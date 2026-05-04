using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    // InputActions are currently bound in the unity object inspector!
    // We can change this if necessary

    // Cinemachine 2D Camera zoom control
    public InputAction ZoomAction;
    public CinemachineCamera Cam;
    // Input action for player movement
    public InputAction MoveAction;

    // Movement variables
    [SerializeField] private float speed = 10f;
    [SerializeField] private Rigidbody2D rb;
    private Vector2 move;

    // Camera variables
    [SerializeField] private float defaultZoom = 5f;
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxZoom = 20f;

    // Each mouse wheel tick will increase / decrease zoom by 10%
    [SerializeField] private float zoomPercentage = 0.10f;
    private float currentZoom;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        MoveAction.Enable();    
        ZoomAction.Enable();

        // Setup currentZoom and ensure it stays within bounds
        currentZoom = Mathf.Clamp(defaultZoom, minZoom, maxZoom);

        if (Cam != null)
        {
            // Get reference to lens, assign new value, then reasign Camera lens
            // This process is necessary because Cinemachine 3.0 Cam is a struct
            var lens = Cam.Lens;
            lens.OrthographicSize = currentZoom;
            Cam.Lens = lens;
        }
    }

    private void OnEnable()
    {
        MoveAction.Enable();
        ZoomAction.Enable();
    }

    private void OnDisable()
    {
        MoveAction.Disable();
        ZoomAction.Disable();
    }

    private void Update()
    {
        //Get the movement vector from MoveAction InputAction variable
        move = MoveAction.ReadValue<Vector2>();

        // ZoomAction can be bound to mouse scroll (Vector2) and/or keys (float)
        // This allows zoom to be rebound, or have additional binds like ctrl+
        var scrollVector = ZoomAction.ReadValue<Vector2>();
        float scroll = 0;
        if (!Mathf.Approximately(scrollVector.y, 0f)) scroll = scrollVector.y;
        else scroll = ZoomAction.ReadValue<float>(); // for key bindings like ctrl+ / ctrl-
        
        if (!Mathf.Approximately(scroll, 0f))
        {
            var steps = Mathf.Max(1, Mathf.RoundToInt(Mathf.Abs(scroll)));
            if (scroll > 0f)
            {
                // Zoom in if mouse wheel up
                currentZoom *= Mathf.Pow(1f - zoomPercentage, steps); 
            } else
            {
                // Zoom out if mouse wheel down
                currentZoom *= Mathf.Pow(1f + zoomPercentage, steps);                 
            }

            // Prevent zoom from going beyond limits
            currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
        }

        if(Cam != null)
        {
            var lens = Cam.Lens;
            lens.OrthographicSize = currentZoom;
            Cam.Lens = lens;
        }
    }

    private void FixedUpdate()
    { 
        var pos = rb.position + (move * (speed * Time.fixedDeltaTime));
        rb.MovePosition(pos);
    }
}
