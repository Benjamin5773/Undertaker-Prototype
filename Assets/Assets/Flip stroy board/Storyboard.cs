using UnityEngine;

public class StoryBoard : MonoBehaviour
{
    [Header("Player  Setting")]
    [SerializeField] GameObject player;  

    [Header("Rotation Settings")]
    [Tooltip("Angle to rotate when triggered (degrees)")]
    [SerializeField] float rotationAngle = -90.0f;  

    [Tooltip("Rotation speed")]
    [SerializeField] float rotationSpeed = 90.0f;  

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
    
    //Ö´ÐÐÐý×ª
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
