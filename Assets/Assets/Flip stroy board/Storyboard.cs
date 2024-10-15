using UnityEngine;

public class StoryBoard : MonoBehaviour
{
    [Header("Player Settings")]
    public GameObject player;  

    [Header("Rotation Settings")]
    [Tooltip("Angle to rotate when triggered (degrees)")]
    public float rotationAngle = -90.0f;  

    [Tooltip("Rotation speed (degrees per second)")]
    public float rotationSpeed = 90.0f;  

    private bool isRotating = false;
    private float targetRotationX;
    private float currentRotationX;

    void Update()
    {
        if (isRotating)
        {
            RotateTowardsTarget();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject == player)
        {
            StartRotation();
        }
    }

    private void StartRotation()
    {
        currentRotationX = transform.eulerAngles.x;
        targetRotationX = currentRotationX + rotationAngle;
        isRotating = true;
    }

    private void RotateTowardsTarget()
    {
        float step = rotationSpeed * Time.deltaTime;  
        float newX = Mathf.MoveTowardsAngle(transform.eulerAngles.x, targetRotationX, step);
        transform.eulerAngles = new Vector3(newX, transform.eulerAngles.y, transform.eulerAngles.z);

        if (Mathf.Approximately(newX, targetRotationX))
        {
            isRotating = false;  
        }
    }
}
