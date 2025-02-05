using UnityEngine;

public class Paddle : MonoBehaviour
{
    [SerializeField]
    Rigidbody rb;
    Vector3 m_EulerAngleVelocity;

    public float movementSpeed = 0f;
    private float currentAngle = 0f;
    public float rotationAngle = 10f;
    public float rotationSpeed = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Rotate();
    }

    void Move()
    {
        float movement = 0;

        // if left is pressed, move left
        if (Input.GetKey(KeyCode.A))
        {
            movement = -movementSpeed;
        }

        // if right is pressed, move right
        if (Input.GetKey(KeyCode.D))
        {
            movement = movementSpeed;
        }

        rb.linearVelocity = new Vector3(movement, 0, 0);
    }

    void Rotate()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll > 0)
        {
            currentAngle += rotationAngle;
        }
        else if (scroll < 0)
        {
            currentAngle -= rotationAngle;
        }

        Quaternion targetRotation = Quaternion.Euler(0, 0, currentAngle);
        rb.MoveRotation(Quaternion.Lerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
    }
}
