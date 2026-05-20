using UnityEngine;          // For functionality.



// Brief description.
public class Belt : AbstractBuilding
{


    // ##### MEMBER VARIABLE OVERRIDES #####
    // ##### METHODS #####
    // ##### Method Overrides #####
    /**
      * @brief The function called during the onUpdate() override.
      * @returns A boolean of whether or not the send action succeeded.
      */
    internal override void Act()
    {
        if (!isInitialized) return;

        ActTimer -= Time.deltaTime;
        if (ActTimer > 0f) return;

        // Push the item at the front into the first receiver if possible.
        if (Receivers.Count > 0 && slots.Length > 0)
        {
            var frontItem = slots[0];
            if (frontItem != emptySlot)
            {
                var receiver = Receivers[0];
                if (receiver != null && Send(frontItem, receiver))
                {
                    slots[0] = emptySlot;
                    DestroySlotVisual(0);
                }
            }

            // Move every item one step toward the output.
            for (var i = 1; i < slots.Length; i++)
            {
                if (slots[i - 1] == emptySlot && slots[i] != emptySlot)
                {
                    slots[i - 1] = slots[i];
                    slots[i] = emptySlot;
                    MoveSlotVisual(i, i - 1);
                }
            }
        }

        SyncVisuals();
        ResetProgress();
        return;
    }

    // Unity Methods
    private void Awake()
    {
        OnCreate();
    }

    private void OnCreate()
    {
        if (isInitialized) return;
        isInitialized = true;

        Cooldown = 0.25f;
        ActTimer = Cooldown;

        for (var i = 0; i < AcceptedResources.Length; i++)
        {
            AcceptedResources[i] = true;
        }

        var slotCount = Mathf.Max(1, Tiles * ItemsPerTile);
        slots = new int[slotCount];
        slotVisuals = new GameObject[slotCount];
        for (var i = 0; i < slotCount; i++)
        {
            slots[i] = emptySlot;
        }

        EnsureVisualRoot();

        // Attempt to attach to Receiver building in front of belt.
        if (Physics.Raycast(transform.position, transform.right, out var potentialReceiver, ConnectionRange))
        {
            if (potentialReceiver.transform.gameObject.TryGetComponent(out AbstractBuilding toBeReceiver))
            {
                Receivers.Add(toBeReceiver);
                toBeReceiver.Senders.Add(this);
            }
        }

        // Attempt to attach to Sender behind the belt.
        if (Physics.Raycast(transform.position, -transform.right, out var potentialSender, ConnectionRange))
        {
            if (potentialSender.transform.gameObject.TryGetComponent(out AbstractBuilding toBeSender))
            {
                Senders.Add(toBeSender);
                toBeSender.Receivers.Add(this);
            }
        }
    }

    private void OnDestroy()
    {
        if (slotVisuals != null)
        {
            for (var i = 0; i < slotVisuals.Length; i++)
            {
                DestroySlotVisual(i);
            }
        }

        foreach (var sender in Senders)
        {
            if (sender != null && sender.Receivers.Contains(this)) sender.Receivers.Remove(this);
        }

        foreach (var receiver in Receivers)
        {
            if (receiver != null && receiver.Senders.Contains(this)) receiver.Senders.Remove(this);
        }
    }

    // Variables
    // Number of tiles this belt section occupies (straight section length).
    public int Tiles = 1;
    // Items per tile; Creates steps in belt, not perfect yet
    // TODO MUST CHANGE TO MAKE SMOOTH BELT MOVEMENT INSTEAD OF THIS SYSTEM
    public int ItemsPerTile = 8;
    // Reference to the item library for rendering.
    [SerializeField]
    private ItemLibrarySO itemLibrary = null;
    // Internal representation: slots[0] is output slot, slots[last] input slot.
    [SerializeField]
    private int[] slots = null;
    [SerializeField]
    private GameObject[] slotVisuals = null;
    [SerializeField]
    private Transform visualRoot = null;
    private bool isInitialized = false;
    private const int emptySlot = -1;
    private static Sprite sharedItemSprite = null;

    // Belt speed is controlled through Cooldown
    // Use the inherited cooldown field and set it during initialization to not have double serialization

    // Receive/Send Overrides
    internal override bool Receive(in int resourceID, AbstractBuilding inputSender)
    {
        if (!isInitialized || slots == null || slots.Length == 0) return false;

        var validSender = inputSender != null && Senders.Contains(inputSender);
        if (!validSender) return false;
        if (!AcceptedResources[resourceID]) return false;

        var tailIndex = slots.Length - 1;
        if (slots[tailIndex] == emptySlot)
        {
            slots[tailIndex] = resourceID;
            CreateOrUpdateSlotVisual(tailIndex, resourceID);
            return true;
        }

        return false;
    }

    protected override bool Send(in int resourceID, AbstractBuilding inputReceiver)
    {
        if (inputReceiver == null) return false;
        if (!Receivers.Contains(inputReceiver)) return false;
        if (!inputReceiver.AcceptedResources[resourceID]) return false;

        if (inputReceiver.Receive(resourceID, this))
        {
            return true;
        }

        return false;
    }

    private void EnsureVisualRoot()
    {
        if (visualRoot != null) return;

        var rootObject = new GameObject("BeltItems");
        rootObject.transform.SetParent(transform, false);
        visualRoot = rootObject.transform;
    }

    private void SyncVisuals()
    {
        if (slots == null || slotVisuals == null) return;

        for (var i = 0; i < slots.Length; i++)
        {
            if (slots[i] == emptySlot)
            {
                DestroySlotVisual(i);
            }
            else
            {
                CreateOrUpdateSlotVisual(i, slots[i]);
            }
        }
    }

    private void MoveSlotVisual(int fromIndex, int toIndex)
    {
        if (slotVisuals == null) return;
        if (fromIndex < 0 || fromIndex >= slotVisuals.Length) return;
        if (toIndex < 0 || toIndex >= slotVisuals.Length) return;

        var movingVisual = slotVisuals[fromIndex];
        slotVisuals[fromIndex] = null;
        slotVisuals[toIndex] = movingVisual;

        if (movingVisual != null)
        {
            movingVisual.transform.localPosition = GetSlotLocalPosition(toIndex);
        }
    }

    private void DestroySlotVisual(int slotIndex)
    {
        if (slotVisuals == null) return;
        if (slotIndex < 0 || slotIndex >= slotVisuals.Length) return;

        var slotVisual = slotVisuals[slotIndex];
        slotVisuals[slotIndex] = null;
        if (slotVisual != null)
        {
            Destroy(slotVisual);
        }
    }

    private void CreateOrUpdateSlotVisual(int slotIndex, int resourceID)
    {
        if (slotVisuals == null) return;
        if (slotIndex < 0 || slotIndex >= slotVisuals.Length) return;

        var slotVisual = slotVisuals[slotIndex];
        if (slotVisual == null)
        {
            slotVisual = new GameObject($"Item_{slotIndex}");
            slotVisual.transform.SetParent(visualRoot, false);
            slotVisual.transform.localScale = Vector3.one * 0.22f;

            var spriteRenderer = slotVisual.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = GetItemSprite(resourceID);
            spriteRenderer.sortingOrder = 10;

            slotVisuals[slotIndex] = slotVisual;
        }

        var existingRenderer = slotVisual.GetComponent<SpriteRenderer>();
        if (existingRenderer != null)
        {
            existingRenderer.sprite = GetItemSprite(resourceID);
            existingRenderer.color = Color.white;
        }

        slotVisual.transform.localPosition = GetSlotLocalPosition(slotIndex);
    }

    private Vector3 GetSlotLocalPosition(int slotIndex)
    {
        var totalSlots = Mathf.Max(1, slots.Length);
        var t = totalSlots <= 1 ? 0.5f : 1f - ((float)slotIndex / (totalSlots - 1));
        var beltLength = Mathf.Max(1f, Tiles);
        var localX = Mathf.Lerp(-beltLength * 0.5f, beltLength * 0.5f, t);
        return new Vector3(localX, 0.15f, 0f);
    }

    private static Sprite GetSharedItemSprite()
    {
        if (sharedItemSprite != null) return sharedItemSprite;

        var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        texture.SetPixels32(new[] { new Color32(255, 255, 255, 255), new Color32(255, 255, 255, 255), new Color32(255, 255, 255, 255), new Color32(255, 255, 255, 255) });
        texture.Apply();
        sharedItemSprite = Sprite.Create(texture, new Rect(0f, 0f, 2f, 2f), new Vector2(0.5f, 0.5f), 2f);
        return sharedItemSprite;
    }

    private Sprite GetItemSprite(int resourceID)
    {
        if (itemLibrary != null && itemLibrary.Items != null &&
            resourceID >= 0 && resourceID < itemLibrary.Items.Length)
        {
            var item = itemLibrary.Items[resourceID];
            if (item != null && item.Icon != null)
            {
                return item.Icon;
            }
        }

        return GetSharedItemSprite();
    }
}
