using UnityEngine;
using UnityEngine.InputSystem;


public class BuildingPlacement : MonoBehaviour
{
    internal static bool IsCreatingGhost { get; private set; } = false;
    private bool isBuildMode = false;                           // Build Mode must be active to place buildings
    private bool isDeleteMode = false;                          // Delete mode removes placed buildings instead of placing new ones
    [SerializeField] private GameObject[] buildPrefabs = new GameObject[7];      // Array of all buildings that can be placed
    [SerializeField] private float gridSize = 1f;
    private int selectedIndex;                                  // Currently selected building
    private int rotationIndex;                                  // Rotation for placed building
    private GameObject ghost;                                   // Ghost for showing where building will be
    private GameObject deleteIndicator;                         // Red outline shown while deleting buildings
    private TilemapChunk tilemapChunk;
    private readonly Collider[] overlapBuffer = new Collider[32];


    // Input Actions for placing buildings if implemented this way
    /*
    public InputAction BuildMode;
    public InputAction BuildingSelection; // 1 - 9 to refer to buildPrefabs array
    public InputAction RotateBuilding;
    public InputAction PlaceBuilding;
    */

    private void Start()
    {
        tilemapChunk = FindObjectsByType<TilemapChunk>(FindObjectsInactive.Exclude)[0];
    }

    private void Update()
    {
        if (Keyboard.current.bKey.wasPressedThisFrame)
        {
            isBuildMode = !isBuildMode;
            if (!isBuildMode)
            {
                ClearPlacementPreview();
            }
            else
            {
                RefreshPlacementPreview();
            }
        }

        if (!isBuildMode) return;

        if (Keyboard.current.deleteKey.wasPressedThisFrame)
        {
            isDeleteMode = !isDeleteMode;
            RefreshPlacementPreview();
        }

        if (isDeleteMode)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                DeleteAt(GetSnappedPosition());
            }

            UpdateDeleteIndicator();
            return;
        }

        var selectionChanged = false;
        if (Keyboard.current.digit1Key.wasPressedThisFrame) { selectedIndex = 0; selectionChanged = true; }
        if (Keyboard.current.digit2Key.wasPressedThisFrame) { selectedIndex = 1; selectionChanged = true; }
        if (Keyboard.current.digit3Key.wasPressedThisFrame) { selectedIndex = 2; selectionChanged = true; }
        if (Keyboard.current.digit4Key.wasPressedThisFrame) { selectedIndex = 3; selectionChanged = true; }
        if (Keyboard.current.digit5Key.wasPressedThisFrame) { selectedIndex = 4; selectionChanged = true; }
        if (Keyboard.current.digit6Key.wasPressedThisFrame) { selectedIndex = 5; selectionChanged = true; }
        if (Keyboard.current.digit7Key.wasPressedThisFrame) { selectedIndex = 6; selectionChanged = true; }
        if (Keyboard.current.digit8Key.wasPressedThisFrame) { selectedIndex = 7; selectionChanged = true; }

        if (selectionChanged)
        {
            RefreshPlacementPreview();
        }

        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            Rotate();
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Place();
        }

        UpdateGhost();
    }

    private void Place()
    {
        var snapped = GetSnappedPosition();
        if (!CanPlaceAt(snapped)) return;
        if (!CanPlaceMinerAt(snapped)) return;

        Instantiate(buildPrefabs[selectedIndex], snapped, GetPlacementRotation());
    }

    private GameObject GetSelectedPrefab()
    {
        if (selectedIndex < 0 || selectedIndex >= buildPrefabs.Length)
        {
            return null;
        }

        return buildPrefabs[selectedIndex];
    }

    private void RefreshPlacementPreview()
    {
        if (!isBuildMode)
        {
            ClearPlacementPreview();
            return;
        }

        if (isDeleteMode)
        {
            ClearGhost();
            CreateDeleteIndicator();
            return;
        }

        ClearDeleteIndicator();

        var selectedPrefab = GetSelectedPrefab();
        if (selectedPrefab == null)
        {
            return;
        }

        if (ghost != null)
        {
            Destroy(ghost);
            ghost = null;
        }

        IsCreatingGhost = true;
        ghost = Instantiate(selectedPrefab, GetSnappedPosition(), GetPlacementRotation());
        IsCreatingGhost = false;
        MakeGhostVisualOnly(ghost);
        ghost.SetActive(true);
    }

    private void ClearPlacementPreview()
    {
        ClearGhost();
        ClearDeleteIndicator();
    }

    private void ClearGhost()
    {
        if (ghost != null)
        {
            Destroy(ghost);
            ghost = null;
        }
    }

    private void ClearDeleteIndicator()
    {
        if (deleteIndicator != null)
        {
            Destroy(deleteIndicator);
            deleteIndicator = null;
        }
    }

    private void MakeGhostVisualOnly(GameObject ghostObject)
    {
        var colliders = ghostObject.GetComponentsInChildren<Collider>(true);
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }

        var scripts = ghostObject.GetComponentsInChildren<MonoBehaviour>(true);
        foreach (var script in scripts)
        {
            if (ReferenceEquals(script, null) || script is BuildingPlacement)
            {
                continue;
            }

            script.enabled = false;
        }

        var spriteRenderers = ghostObject.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var spriteRenderer in spriteRenderers)
        {
            var color = spriteRenderer.color;
            color.r = 0.35f;
            color.g = 0.65f;
            color.b = 1.0f;
            color.a = 0.45f;
            spriteRenderer.color = color;
        }
    }

    private void CreateDeleteIndicator()
    {
        if (deleteIndicator != null)
        {
            return;
        }

        deleteIndicator = new GameObject("DeleteIndicator");
        var lineRenderer = deleteIndicator.AddComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        lineRenderer.positionCount = 5;
        lineRenderer.startWidth = gridSize * 0.08f;
        lineRenderer.endWidth = gridSize * 0.08f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = new Color(1f, 0.2f, 0.2f, 0.65f);
        lineRenderer.endColor = new Color(1f, 0.2f, 0.2f, 0.65f);

        var half = gridSize * 0.45f;
        lineRenderer.SetPosition(0, new Vector3(-half, -half, 0f));
        lineRenderer.SetPosition(1, new Vector3(-half, half, 0f));
        lineRenderer.SetPosition(2, new Vector3(half, half, 0f));
        lineRenderer.SetPosition(3, new Vector3(half, -half, 0f));
        lineRenderer.SetPosition(4, new Vector3(-half, -half, 0f));

        UpdateDeleteIndicator();
    }

    private void UpdateDeleteIndicator()
    {
        if (deleteIndicator == null) return;

        var snapped = GetSnappedPosition();
        deleteIndicator.transform.SetPositionAndRotation(snapped, Quaternion.identity);
    }

    private Vector3 GetSnappedPosition()
    {
        var world = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        world.z = 0f;

        var x = (Mathf.Floor(world.x / gridSize) * gridSize) + (gridSize * 0.5f);
        var y = (Mathf.Floor(world.y / gridSize) * gridSize) + (gridSize * 0.5f);
        return new Vector3(x, y, 0f);
    }

    private Quaternion GetPlacementRotation()
    {
        return Quaternion.Euler(0f, 0f, -90f * rotationIndex);
    }

    private bool CanPlaceAt(Vector3 position)
    {
        var overlapCount = Physics.OverlapBoxNonAlloc(position, new Vector3(gridSize * 0.45f, gridSize * 0.45f, 0.5f), overlapBuffer);
        for (var i = 0; i < overlapCount; i++)
        {
            var overlap = overlapBuffer[i];
            if (overlap == null)
            {
                continue;
            }

            if (overlap.GetComponentInParent<AbstractBuilding>() != null)
            {
                return false;
            }
        }

        if (overlapCount == overlapBuffer.Length)
        {
            return false;
        }

        return true;
    }

    private bool CanPlaceMinerAt(Vector3 position)
    {
        var selectedPrefab = buildPrefabs[selectedIndex];
        if (selectedPrefab == null || selectedPrefab.GetComponent<Miner>() == null)
        {
            return true;
        }

        return tilemapChunk != null && tilemapChunk.GetResourceAt(position) != null;
    }

    private void DeleteAt(Vector3 position)
    {
        var building = GetBuildingAt(position);
        if (building == null)
        {
            return;
        }

        DetachBuilding(building);
        Destroy(building.gameObject);
    }

    private AbstractBuilding GetBuildingAt(Vector3 position)
    {
        var overlapCount = Physics.OverlapBoxNonAlloc(position, new Vector3(gridSize * 0.45f, gridSize * 0.45f, 0.5f), overlapBuffer);
        for (var i = 0; i < overlapCount; i++)
        {
            var overlap = overlapBuffer[i];
            if (overlap == null)
            {
                continue;
            }

            var building = overlap.GetComponentInParent<AbstractBuilding>();
            if (building != null)
            {
                return building;
            }
        }

        return null;
    }

    private static void DetachBuilding(AbstractBuilding building)
    {
        foreach (var sender in building.Senders)
        {
            if (sender != null)
            {
                sender.Receivers.Remove(building);
            }
        }

        foreach (var receiver in building.Receivers)
        {
            if (receiver != null)
            {
                receiver.Senders.Remove(building);
            }
        }

        building.Senders.Clear();
        building.Receivers.Clear();
    }

    private void Rotate()
    {
        rotationIndex = (rotationIndex + 1) % 4;
        if (ghost != null)
        {
            ghost.transform.rotation = GetPlacementRotation();
        }
    }

    private void UpdateGhost()
    {
        if (ghost == null) return;

        var snapped = GetSnappedPosition();
        ghost.transform.SetPositionAndRotation(snapped, GetPlacementRotation());
    }
}