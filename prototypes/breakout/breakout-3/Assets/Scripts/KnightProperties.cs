using UnityEngine;

public class Knight : MonoBehaviour
{
    [SerializeField]
    Rigidbody rb;

    public float movementSpeed = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
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
}
