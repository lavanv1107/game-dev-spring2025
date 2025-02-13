using UnityEngine;

public class BrickMovement : MonoBehaviour
{
    private float moveSpeed;
    private float moveDistance;
    private float rotationSpeed;
    
    private float startingX;
    private int direction;
    private bool isRotating = true;
    private bool hasInitialRotation = false;
    private Quaternion targetRotation;
    private Vector3 initialRotation;

    public void Initialize(float speed, float distance, float rotSpeed, int initialDirection)
    {
        moveSpeed = speed;
        moveDistance = distance;
        rotationSpeed = rotSpeed;
        direction = initialDirection;
        SetupInitialMovement();
    }

    private void SetupInitialMovement()
    {
        startingX = transform.position.x;
        initialRotation = transform.rotation.eulerAngles;
        
        float targetYRotation = initialRotation.y + (direction == 1 ? -90f : 90f);
        targetRotation = Quaternion.Euler(initialRotation.x, targetYRotation, initialRotation.z);
    }

    private void FixedUpdate()
    {
        if (isRotating)
        {
            HandleRotation();
        }
        else if (hasInitialRotation)
        {
            HandleMovement();
        }
    }

    private void HandleRotation()
    {
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation, 
            targetRotation, 
            rotationSpeed * Time.fixedDeltaTime
        );
        
        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
        {
            CompleteRotation();
        }
    }

    private void HandleMovement()
    {
        float distanceFromStart = transform.position.x - startingX;
        
        if (ShouldChangeDirection(distanceFromStart))
        {
            ChangeDirection(distanceFromStart);
        }
        else
        {
            Move();
        }
    }

    private bool ShouldChangeDirection(float distanceFromStart)
    {
        return (distanceFromStart >= moveDistance && direction == 1) || 
               (distanceFromStart <= -moveDistance && direction == -1);
    }

    private void ChangeDirection(float distanceFromStart)
    {
        direction *= -1;
        
        Vector3 currentEuler = transform.rotation.eulerAngles;
        float newYRotation = (direction == 1) ? currentEuler.y - 180f : currentEuler.y + 180f;
        
        targetRotation = Quaternion.Euler(currentEuler.x, newYRotation, currentEuler.z);
        isRotating = true;
    }

    private void Move()
    {
        Vector3 movement = new Vector3(direction * moveSpeed * Time.fixedDeltaTime, 0f, 0f);
        transform.position += movement;
    }

    private void CompleteRotation()
    {
        isRotating = false;
        transform.rotation = targetRotation;
        hasInitialRotation = true;
    }
}