using UnityEngine;
using UnityEngine.SceneManagement;

public class Ball : MonoBehaviour
{
    [SerializeField]
    Rigidbody rb;

    public float speed = 10f;
    public float speedIncrease = 1.05f;

    private float currentSpeed;

    public Paddle paddle;
    public float sizeIncrease = 1.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb.linearVelocity = new Vector3(speed, -speed, 0);
    }

    // Update is called once per frame
    void Update()
    {
        currentSpeed = rb.linearVelocity.magnitude;
        paddle.movementSpeed = currentSpeed * 1.5f;

        Vector3 viewportPosition = Camera.main.WorldToViewportPoint(transform.position);
  
        if (viewportPosition.x < 0 || viewportPosition.x > 1 || 
        viewportPosition.y < 0 || viewportPosition.y > 1) 
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    void OnCollisionEnter(Collision collision)
    {      
        if (collision.gameObject.CompareTag("paddle")){
            rb.linearVelocity = rb.linearVelocity.normalized * currentSpeed;
        }

        if (collision.gameObject.CompareTag("brick") || collision.gameObject.CompareTag("sizeIncreaseBrick"))
        {
            Destroy(collision.gameObject);
            rb.linearVelocity = rb.linearVelocity * speedIncrease;
        }

        if (collision.gameObject.CompareTag("sizeIncreaseBrick"))
        {
            float paddleSize = paddle.transform.localScale.x * sizeIncrease;
            paddle.transform.localScale = new Vector3(paddleSize , 0.5f, 1f);
        }
    }
}
