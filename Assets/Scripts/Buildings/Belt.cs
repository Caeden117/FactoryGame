using UnityEngine;
using System.Collections.Generic;

public class Belt : AbstractBuilding
{
  public enum BeltPathType
  {
    Straight,
    CornerQuarter
  }

  [Header("Path")]
  [SerializeField] private BeltPathType pathType = BeltPathType.Straight;
  [SerializeField] private Transform entryAnchor;
  [SerializeField] private Transform exitAnchor;
  [SerializeField] private Transform cornerCenter;
  using UnityEngine;
  using System.Collections.Generic;

  public class Belt : AbstractBuilding
  {
    public enum BeltPathType
    {
      Straight,
      CornerQuarter
    }

    [Header("Path")]
    [SerializeField] private BeltPathType pathType = BeltPathType.Straight;
    [SerializeField] private Transform entryAnchor;
    [SerializeField] private Transform exitAnchor;
    [SerializeField] private Transform cornerCenter;
    [SerializeField] private float beltLength = 1.0f;

    [Header("Movement")]
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private bool startPowered = true;
    [SerializeField] private bool sleepWhenIdle = true;
    [SerializeField] private bool acceptAllResources = true;

    [Header("Auto Connect")]
    [SerializeField] private bool autoConnect = true;
    [SerializeField] private float connectionRadius = 0.3f;
    [SerializeField] private float directionDotThreshold = 0.6f;

    private static readonly List<Belt> ActiveBelts = new List<Belt>(128);

    private Vector3 entryLocal;
    private Vector3 exitLocal;
    private Vector3 cornerCenterLocal;
    private Vector3 cornerNormalLocal;
    private float cornerRadius;
    private float cornerAngleDeg;
    private bool cornerValid;
    private bool initialized;

    private bool hasItem;
    private int itemResourceId = -1;
    private float itemDistance;

    public bool HasItem => hasItem;
    public int ItemResourceId => itemResourceId;
    public float ItemDistance => itemDistance;

    private void Awake()
    {
      InitializeDefaults();
      CacheAnchors();
      TogglePower(startPowered);
    }

    private void OnEnable()
    {
      if (!ActiveBelts.Contains(this))
      {
        ActiveBelts.Add(this);
      }
      if (autoConnect)
      {
        RefreshConnections();
      }
    }

    private void OnDisable()
    {
      if (autoConnect)
      {
        ClearConnectionsToSelf();
      }
      ActiveBelts.Remove(this);
    }

    private void OnValidate()
    {
      beltLength = Mathf.Max(0.01f, beltLength);
      speed = Mathf.Max(0.0f, speed);
      connectionRadius = Mathf.Max(0.01f, connectionRadius);
      directionDotThreshold = Mathf.Clamp(directionDotThreshold, 0.0f, 1.0f);
    }

    private void Update()
    {
      if (!isRunning)
      {
        return;
      }

      if (sleepWhenIdle && !hasItem)
      {
        return;
      }

      Act();
    }

    override protected bool Receive(in int resourceID, in AbstractBuilding inputSender)
    {
      EnsureInitialized();

      if (!IsValidInputSender(inputSender) || hasItem || !IsResourceAccepted(resourceID))
      {
        return false;
      }

      hasItem = true;
      itemResourceId = resourceID;
      itemDistance = 0.0f;
      return true;
    }

    override protected bool Act()
    {
      if (!hasItem)
      {
        return false;
      }

      float length = GetBeltLength();
      float newDistance = Mathf.Min(itemDistance + speed * Time.deltaTime, length);
      bool didWork = !Mathf.Approximately(newDistance, itemDistance);
      itemDistance = newDistance;

      if (itemDistance >= length - 0.0001f && TrySendItem())
      {
        hasItem = false;
        itemResourceId = -1;
        itemDistance = 0.0f;
        didWork = true;
      }

      return didWork;
    }

    public Vector3 GetItemWorldPosition()
    {
      return GetWorldPosition(itemDistance);
    }

    public Vector3 GetWorldPosition(float distance)
    {
      EnsureInitialized();

      float length = GetBeltLength();
      if (pathType == BeltPathType.CornerQuarter && cornerValid)
      {
        return transform.TransformPoint(GetCornerLocalPoint(distance, out _));
      }

      float t = length <= 0.0f ? 0.0f : Mathf.Clamp01(distance / length);
      return transform.TransformPoint(Vector3.Lerp(entryLocal, exitLocal, t));
    }

    public Vector3 GetEntryWorld()
    {
      EnsureInitialized();
      return transform.TransformPoint(entryLocal);
    }

    public Vector3 GetExitWorld()
    {
      EnsureInitialized();
      return transform.TransformPoint(exitLocal);
    }

    public Vector3 GetEntryForwardWorld()
    {
      EnsureInitialized();
      return transform.TransformDirection(GetEntryForwardLocal());
    }

    public Vector3 GetExitForwardWorld()
    {
      EnsureInitialized();
      return transform.TransformDirection(GetExitForwardLocal());
    }

    public void RefreshConnections()
    {
      if (!autoConnect)
      {
        return;
      }

      EnsureInitialized();

      Vector3 entryWorld = GetEntryWorld();
      Vector3 exitWorld = GetExitWorld();
      Vector3 entryForward = GetEntryForwardWorld();
      Vector3 exitForward = GetExitForwardWorld();

      AbstractBuilding senderCandidate = null;
      AbstractBuilding receiverCandidate = null;
      float bestSenderDist = float.MaxValue;
      float bestReceiverDist = float.MaxValue;

      for (int i = 0; i < ActiveBelts.Count; i++)
      {
        Belt other = ActiveBelts[i];
        if (other == null || other == this || !other.isActiveAndEnabled)
        {
          continue;
        }

        float senderDist = Vector3.Distance(entryWorld, other.GetExitWorld());
        if (senderDist <= connectionRadius && senderDist < bestSenderDist)
        {
          float dot = Vector3.Dot(entryForward, other.GetExitForwardWorld());
          if (dot >= directionDotThreshold)
          {
            bestSenderDist = senderDist;
            senderCandidate = other;
          }
        }

        float receiverDist = Vector3.Distance(exitWorld, other.GetEntryWorld());
        if (receiverDist <= connectionRadius && receiverDist < bestReceiverDist)
        {
          float dot = Vector3.Dot(exitForward, other.GetEntryForwardWorld());
          if (dot >= directionDotThreshold)
          {
            bestReceiverDist = receiverDist;
            receiverCandidate = other;
          }
        }
      }

      ApplyConnections(senderCandidate, receiverCandidate);
    }

    private void InitializeDefaults()
    {
      for (int i = minResourceID; i <= maxResourceID; i++)
      {
        acceptedResources[i] = acceptAllResources;
      }
    }

    private void CacheAnchors()
    {
      entryLocal = entryAnchor != null ? entryAnchor.localPosition : Vector3.zero;
      exitLocal = exitAnchor != null ? exitAnchor.localPosition : Vector3.forward * beltLength;
      UpdateCornerCache();
    }

    private void EnsureInitialized()
    {
      if (!initialized)
      {
        initialized = true;
      }
      CacheAnchors();
    }

    private bool IsResourceAccepted(int resourceId)
    {
      return resourceId >= minResourceID && resourceId <= maxResourceID && acceptedResources[resourceId];
    }

    private bool IsValidInputSender(AbstractBuilding inputSender)
    {
      if (sender == null || !sender.IsAlive || sender.Target == null)
      {
        return false;
      }

      AbstractBuilding senderProxy = sender.Target as AbstractBuilding;
      return senderProxy != null && inputSender != null && inputSender == senderProxy;
    }

    private bool TrySendItem()
    {
      if (receiver == null || !receiver.IsAlive || receiver.Target == null)
      {
        return false;
      }

      AbstractBuilding receiverProxy = receiver.Target as AbstractBuilding;
      return receiverProxy != null && receiverProxy.Receive(itemResourceId, this);
    }

    private float GetBeltLength()
    {
      if (pathType == BeltPathType.CornerQuarter && cornerValid)
      {
        return cornerRadius * Mathf.Abs(cornerAngleDeg) * Mathf.Deg2Rad;
      }

      float anchorLength = (exitLocal - entryLocal).magnitude;
      return anchorLength > 0.0001f ? anchorLength : Mathf.Max(0.01f, beltLength);
    }

    private Vector3 GetEntryForwardLocal()
    {
      if (pathType == BeltPathType.CornerQuarter && cornerValid)
      {
        Vector3 radial = (entryLocal - cornerCenterLocal).normalized;
        float sign = Mathf.Sign(cornerAngleDeg);
        if (sign == 0.0f)
        {
          sign = 1.0f;
        }
        return Vector3.Cross(cornerNormalLocal * sign, radial).normalized;
      }

      Vector3 forward = exitLocal - entryLocal;
      return forward.sqrMagnitude < 0.0001f ? Vector3.forward : forward.normalized;
    }

    private Vector3 GetExitForwardLocal()
    {
      if (pathType == BeltPathType.CornerQuarter && cornerValid)
      {
        Vector3 radial = (exitLocal - cornerCenterLocal).normalized;
        float sign = Mathf.Sign(cornerAngleDeg);
        if (sign == 0.0f)
        {
          sign = 1.0f;
        }
        return Vector3.Cross(cornerNormalLocal * sign, radial).normalized;
      }

      Vector3 forward = exitLocal - entryLocal;
      return forward.sqrMagnitude < 0.0001f ? Vector3.forward : forward.normalized;
    }

    private Vector3 GetCornerLocalPoint(float distance, out Vector3 tangentLocal)
    {
      float length = GetBeltLength();
      float t = length <= 0.0f ? 0.0f : Mathf.Clamp01(distance / length);
      float angle = cornerAngleDeg * t;

      Vector3 entryOffset = entryLocal - cornerCenterLocal;
      Vector3 radial = Quaternion.AngleAxis(angle, cornerNormalLocal) * entryOffset;
      Vector3 localPoint = cornerCenterLocal + radial;

      float sign = Mathf.Sign(cornerAngleDeg);
      if (sign == 0.0f)
      {
        sign = 1.0f;
      }

      tangentLocal = Vector3.Cross(cornerNormalLocal * sign, radial).normalized;
      return localPoint;
    }

    private void UpdateCornerCache()
    {
      cornerValid = false;
      if (pathType != BeltPathType.CornerQuarter || cornerCenter == null)
      {
        return;
      }

      cornerCenterLocal = cornerCenter.localPosition;
      Vector3 entryOffset = entryLocal - cornerCenterLocal;
      Vector3 exitOffset = exitLocal - cornerCenterLocal;
      float entryRadius = entryOffset.magnitude;
      float exitRadius = exitOffset.magnitude;

      if (entryRadius < 0.0001f || exitRadius < 0.0001f)
      {
        return;
      }

      Vector3 normal = Vector3.Cross(entryOffset, exitOffset);
      if (normal.sqrMagnitude < 0.0001f)
      {
        return;
      }

      cornerNormalLocal = normal.normalized;
      cornerRadius = (entryRadius + exitRadius) * 0.5f;
      cornerAngleDeg = Vector3.SignedAngle(entryOffset, exitOffset, cornerNormalLocal);
      cornerValid = Mathf.Abs(cornerAngleDeg) >= 0.01f;
    }

    private void ApplyConnections(AbstractBuilding senderCandidate, AbstractBuilding receiverCandidate)
    {
      if (senderCandidate != null && (GetSender() == null || GetSender() is Belt))
      {
        SetSender(senderCandidate);
        if (senderCandidate.GetReceiver() == null || senderCandidate.GetReceiver() is Belt)
        {
          senderCandidate.SetReceiver(this);
        }
      }

      if (receiverCandidate != null && (GetReceiver() == null || GetReceiver() is Belt))
      {
        SetReceiver(receiverCandidate);
        if (receiverCandidate.GetSender() == null || receiverCandidate.GetSender() is Belt)
        {
          receiverCandidate.SetSender(this);
        }
      }
    }

    private void ClearConnectionsToSelf()
    {
      for (int i = 0; i < ActiveBelts.Count; i++)
      {
        Belt other = ActiveBelts[i];
        if (other == null || other == this)
        {
          continue;
        }

        if (other.GetSender() == this)
        {
          other.SetSender(null);
        }

        if (other.GetReceiver() == this)
        {
          other.SetReceiver(null);
        }
      }
    }
  }
    cornerAngleDeg = Vector3.SignedAngle(entryOffset, exitOffset, cornerNormalLocal);
    cornerValid = Mathf.Abs(cornerAngleDeg) >= 0.01f;
  }

  private void ApplyConnections(AbstractBuilding senderCandidate, AbstractBuilding receiverCandidate)
  {
    if (senderCandidate != null)
    {
      SetSender(senderCandidate);
      if (senderCandidate.GetReceiver() != this)
      {
        senderCandidate.SetReceiver(this);
      }
    }

    if (receiverCandidate != null)
    {
      SetReceiver(receiverCandidate);
      if (receiverCandidate.GetSender() != this)
      {
        receiverCandidate.SetSender(this);
      }
    }
  }

  private void ClearConnectionsToSelf()
  {
    for (int i = 0; i < ActiveBelts.Count; i++)
    {
      Belt other = ActiveBelts[i];
      if (other == null || other == this)
      {
        continue;
      }

      if (other.GetSender() == this)
      {
        other.SetSender(null);
      }

      if (other.GetReceiver() == this)
      {
        other.SetReceiver(null);
      }
    }
  }
}
