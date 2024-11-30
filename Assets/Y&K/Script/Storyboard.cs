using UnityEngine;

public class StoryBoard : MonoBehaviour
{
    [Header("Player Setting")]
    [SerializeField] GameObject player;

    [Header("Rotation Settings")]
    [Tooltip("Angle to rotate when triggered (degrees)")]
    [SerializeField] float rotationAngle = -90.0f;

    [Tooltip("Rotation speed")]
    [SerializeField] float rotationSpeed = 90.0f;

    [Header("Optional Object Activation")]
    [Tooltip("GameObject to activate after rotation ends (optional)")]
    [SerializeField] GameObject objectToActivate;

    private bool isRotating = false;
    private bool hasTriggered = false; // 标志是否已经触发过
    private float targetRotationX;
    private float currentRotationX;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if (isRotating)
        {
            RotateTowardsTarget();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.gameObject == player) // 检查是否已经触发过
        {
            hasTriggered = true; // 设置标志为 true
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
            OnRotationComplete(); // 调用旋转完成后的逻辑
        }
    }

    private void OnRotationComplete()
    {
        if (objectToActivate != null) // 如果传入了对象，则启用它
        {
            objectToActivate.SetActive(true);
        }
    }
}
