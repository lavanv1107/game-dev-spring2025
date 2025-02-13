using UnityEngine;

public class SwordProperties : MonoBehaviour
{
    // Configuration variables
    [SerializeField] Rigidbody rb;
    public float moveSpeed = 50f;
    public float baseXRotation = 52f;
    public float whackAngle = 30f;
    public float whackSpeed = 400f;
    public float returnSpeed = 400f;
    public float minX = -22.75f;
    public float maxX = 7f;
    
    // State tracking
    private bool isWhacking = false;
    private bool isReturning = false;
    private float currentWhackAngle = 0f;
    private bool whackingLeft = true;
    private Camera mainCamera;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        // Set initial sword rotation and main camera
        transform.rotation = Quaternion.Euler(baseXRotation, -90f, 90f);
        mainCamera = Camera.main;
    }

    void Update()
    {
        HandleMovement();
        HandleWhacking();
        HandleReturn();
    }

    // Updates sword position based on mouse position
    private void HandleMovement()
    {
        // Get mouse position in screen coordinates
        Vector3 mousePosition = Input.mousePosition;
        
        // Convert screen point to world point
        mousePosition.z = -mainCamera.transform.position.z; // Set this to the distance you want from camera
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        
        // Clamp the x position between your desired limits while keeping y and z unchanged
        float clampedX = Mathf.Clamp(worldPosition.x, minX, maxX);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }

    // Handles sword whacking animation when mouse buttons are pressed
    private void HandleWhacking()
    {
        // Only whack if mouse button is pressed and not currently returning
        if ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && !isReturning)
        {
            isWhacking = true;
            whackingLeft = Input.GetMouseButton(0); // Left click = whack left, Right click = whack right
            
            // Increment whack angle up to max
            currentWhackAngle += whackSpeed * Time.deltaTime;
            currentWhackAngle = Mathf.Min(currentWhackAngle, whackAngle);
            
            // Apply rotation
            float newRotX = baseXRotation + (whackingLeft ? -currentWhackAngle : currentWhackAngle);
            transform.rotation = Quaternion.Euler(newRotX, -90f, 90f);
        }

        // Start return animation when mouse button is released
        if ((Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1)) && isWhacking)
        {
            isWhacking = false;
            isReturning = true;
        }
    }

    // Handles sword return animation after whacking
    private void HandleReturn()
    {
        if (!isReturning) return;

        // Decrease whack angle
        currentWhackAngle -= returnSpeed * Time.deltaTime;
        
        // Check if return is complete
        if (currentWhackAngle <= 0)
        {
            // Reset to initial state
            currentWhackAngle = 0;
            isReturning = false;
            transform.rotation = Quaternion.Euler(baseXRotation, -90f, 90f);
            return;
        }

        // Apply return rotation
        float newRotX = baseXRotation + (whackingLeft ? -currentWhackAngle : currentWhackAngle);
        transform.rotation = Quaternion.Euler(newRotX, -90f, 90f);
    }
}