using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Ball : MonoBehaviour
{
    [SerializeField]
    Rigidbody rb;

    private TextMeshProUGUI scoreText; // We'll find this by tag
    private int score = 0;
    public float speed = 10f;
    public float speedIncrease = 1.05f;
    private float currentSpeed;
    public float sizeIncrease = 1.5f;

    void Start()
    {
        rb.linearVelocity = new Vector3(speed, -speed, 0);
        // Find the score text object by tag
        scoreText = GameObject.FindGameObjectWithTag("score").GetComponent<TextMeshProUGUI>();
        UpdateScoreText();
    }

    void Update()
    {
        currentSpeed = rb.linearVelocity.magnitude;
        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);
    }

    void OnCollisionEnter(Collision collision)
    {      
        if (collision.gameObject.CompareTag("brick") || collision.gameObject.CompareTag("sizeIncreaseBrick"))
        {
            Destroy(collision.gameObject);
            rb.linearVelocity = rb.linearVelocity * speedIncrease;
            
            score += 50;
            UpdateScoreText();
        }

        if (collision.gameObject.CompareTag("sizeIncreaseBrick"))
        {
            // Size increase logic here
        }
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }
}