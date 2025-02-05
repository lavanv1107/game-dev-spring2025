using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class Princess : MonoBehaviour
{
    // Movement settings
    [SerializeField] private Rigidbody rb;
    public float moveSpeed = 5f;
    public float moveDistance = 5f;
    public float rotationSpeed = 180f;
    
    // State variables
    private bool isStunned = false;
    private bool isStanding = false;
    private bool isRotating = true;
    private bool hasInitialRotation = false;
    
    // Charge system and UI
    private TextMeshProUGUI chargesText;
    private TextMeshProUGUI livesText;
    private int currentCharges = 0;
    private int currentLives = 3;
    private const int MAX_CHARGES = 3;
    private const int MAX_LIVES = 3;
    
    // Movement tracking
    private float startingX;
    private int direction;
    private Quaternion targetRotation;
    private Quaternion lastMovementRotation;
    
    // Visual components
    private Renderer princessRenderer;
    private Material princessMaterial;
    private Color originalColor;

    // Standing cycle timer
    private float standingCycleTimer = 0f;
    private const float STANDING_CYCLE_INTERVAL = 3f;
    private GameObject lastRowBrick; // Reference to the currently flashing brick
    private Color blueStandingColor = new Color(0f, 0.5f, 1f, 1f);
    
    void Start()
    {
        InitializeComponents();
        SetInitialRotation();
        UpdateChargesDisplay();
    }

    void Update()
    {
        standingCycleTimer += Time.deltaTime;
        if (standingCycleTimer >= STANDING_CYCLE_INTERVAL)
        {
            standingCycleTimer = 0f;
            ToggleStanding();
        }
    }

    void FixedUpdate()
    {
        // Debug state on seemingly stuck situations
        if (!isRotating && !isStanding && !isStunned && rb.linearVelocity.magnitude <= 0)
        {
            Debug.Log($"State check - Rotation: {transform.rotation.eulerAngles}, Position: {transform.position}, Direction: {direction}");
        }

        if (isStunned || isStanding) return;
        
        if (isRotating)
        {
            HandleRotation();
            return;
        }

        if (hasInitialRotation)
        {
            HandleMovement();
        }
    }

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody>();
        startingX = transform.position.x;
        
        // Setup visual components
        princessRenderer = GetComponent<Renderer>();
        princessMaterial = princessRenderer.material;
        originalColor = princessMaterial.color;
        
        // Find charges text by tag
        GameObject chargesTextObj = GameObject.FindWithTag("charges");
        if (chargesTextObj != null)
        {
            chargesText = chargesTextObj.GetComponent<TextMeshProUGUI>();
            if (chargesText == null)
            {
                Debug.LogError("Found object with 'charges' tag but it doesn't have a TextMeshProUGUI component!");
            }
        }
        else
        {
            Debug.LogError("Could not find object with 'charges' tag!");
        }

        // Find lives text by tag
        GameObject livesTextObj = GameObject.FindWithTag("lives");
        if (livesTextObj != null)
        {
            livesText = livesTextObj.GetComponent<TextMeshProUGUI>();
            if (livesText == null)
            {
                Debug.LogError("Found object with 'lives' tag but it doesn't have a TextMeshProUGUI component!");
            }
        }
        else
        {
            Debug.LogError("Could not find object with 'lives' tag!");
        }

        // Initialize lives display
        UpdateLivesDisplay();
        
        // Set random initial direction
        direction = Random.value > 0.5f ? 1 : -1;
    }

    private void UpdateChargesDisplay()
    {
        if (chargesText != null)
        {
            chargesText.text = $"Charges: {currentCharges}/{MAX_CHARGES}";
        }
    }

    private void UpdateLivesDisplay()
    {
        if (livesText != null)
        {
            livesText.text = $"Lives: {currentLives}/{MAX_LIVES}";
        }
    }

    private void LoseLife()
    {
        currentLives--;
        UpdateLivesDisplay();
        
        if (currentLives <= 0)
        {
            SceneManager.LoadScene("GameOver");
        }
    }

    private void SetInitialRotation()
    {
        transform.rotation = Quaternion.Euler(-89.98f, -180f, 0);
        float targetYRotation = direction == 1 ? -270f : -90f;
        targetRotation = Quaternion.Euler(-89.98f, targetYRotation, 0);
    }

    private void HandleRotation()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        
        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
        {
            isRotating = false;
            transform.rotation = targetRotation;
            hasInitialRotation = true;
        }
    }

    private void HandleMovement()
    {
        // Debug movement state
        if (transform.position == rb.position)
        {
            Debug.Log($"Princess stuck! States: Stunned={isStunned}, Standing={isStanding}, Rotating={isRotating}, HasInitialRotation={hasInitialRotation}");
        }
        
        float distanceFromStart = transform.position.x - startingX;
        
        if ((distanceFromStart >= moveDistance && direction == 1) ||
            (distanceFromStart <= -moveDistance && direction == -1))
        {
            ChangeDirection();
            return;
        }
        
        // Only move if we're in a valid movement state
        if (!isRotating && !isStunned && !isStanding)
        {
            Vector3 movement = new Vector3(direction * moveSpeed * Time.fixedDeltaTime, 0f, 0f);
            rb.MovePosition(rb.position + movement);
        }
    }

    private void ChangeDirection()
    {
        direction *= -1;
        targetRotation = Quaternion.Euler(-89.98f, direction == 1 ? -270f : -90f, 0);
        isRotating = true;
    }

    private void ToggleStanding()
    {
        isStanding = !isStanding;
        if (isStanding)
        {
            lastMovementRotation = transform.rotation;
            StopAllCoroutines();
            StartCoroutine(StandingEffect());
            
            // Increment charges when entering standing state
            if (currentCharges < MAX_CHARGES)
            {
                currentCharges++;
                UpdateChargesDisplay();
            }
            else if (currentCharges >= MAX_CHARGES)
            {
                // Reset charges and trigger brick flash
                currentCharges = 0;
                UpdateChargesDisplay();
                if (BrickSpawner.Instance != null)
                {
                    BrickSpawner.Instance.FlashRandomLastRowBrick(blueStandingColor);
                }
            }
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(ResumeMovementEffect());
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ball") && !isStunned)
        {
            HandleBallCollision(collision.gameObject);
        }
    }

    private void HandleBallCollision(GameObject ball)
    {
        Destroy(ball);
        
        if (BrickSpawner.Instance != null)
        {
            BrickSpawner.Instance.SpawnBallFromLastRow();
        }
        
        if (isStanding)
        {
            isStanding = false;
            StopAllCoroutines();
            StartCoroutine(ResumeMovementEffect());
        }
        
        // Reset charges on ball hit
        currentCharges = 0;
        UpdateChargesDisplay();
        
        // Reduce lives and check for game over
        LoseLife();
        
        StartCoroutine(StunEffect());
    }

    private IEnumerator StunEffect()
    {
        Debug.Log("Starting stun effect");
        isStunned = true;
        isRotating = false; // Ensure we're not stuck in rotation
        Color orangeColor = new Color(1f, 0.5f, 0f, 1f);
        
        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            float lerp = Mathf.PingPong(elapsedTime * 4f, 1f);
            princessMaterial.color = Color.Lerp(originalColor, orangeColor, lerp);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        princessMaterial.color = originalColor;
        isStunned = false;
        hasInitialRotation = true; // Ensure we can move after stun
        Debug.Log("Stun effect complete");
    }

    private IEnumerator StandingEffect()
    {
        Quaternion standingRotation = Quaternion.Euler(-89.98f, -180f, 0);
        while (Quaternion.Angle(transform.rotation, standingRotation) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, standingRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = standingRotation;

        Color blueColor = new Color(0f, 0.5f, 1f, 1f);
        while (isStanding)
        {
            float lerp = Mathf.PingPong(Time.time * 2f, 1f);
            princessMaterial.color = Color.Lerp(originalColor, blueColor, lerp);
            yield return null;
        }
    }

    private IEnumerator ResumeMovementEffect()
    {
        Debug.Log("Resuming movement");
        princessMaterial.color = originalColor;
        
        // Make sure we're not in any blocking states
        isStanding = false;
        isStunned = false;
        
        targetRotation = Quaternion.Euler(-89.98f, direction == 1 ? -270f : -90f, 0);
        isRotating = true; // Set rotating to true while we turn

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }
        transform.rotation = targetRotation;
        
        // Reset all movement states properly
        isStanding = false;
        isRotating = false;
        isStunned = false;
        hasInitialRotation = true;
        
        Debug.Log($"Movement resumed. Direction: {direction}");
    }
}