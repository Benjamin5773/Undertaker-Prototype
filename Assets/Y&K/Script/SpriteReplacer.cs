using UnityEngine;

public class SpriteReplacer : MonoBehaviour
{

    [Header("Sprite Settings")]
    [Tooltip("The new sprite to replace with after collision")]
    [SerializeField] Sprite newSprite;

    private SpriteRenderer spriteRenderer;
    private bool hasTriggered = false; // 标志是否已经触发过

    private void Start()
    {
        // 获取当前物体的 SpriteRenderer 组件
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component not found on the object!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 检查是否已经触发过，以及 SpriteRenderer 和 newSprite 是否有效
        if (!hasTriggered && spriteRenderer != null && newSprite != null)
        {
            hasTriggered = true; // 设置标志为 true，确保只触发一次
            ReplaceSprite();
            RotateObject();
        }
        else if (newSprite == null)
        {
            Debug.LogWarning("No new sprite assigned in the inspector.");
        }
    }

    private void ReplaceSprite()
    {
        spriteRenderer.sprite = newSprite;
        Debug.Log("Sprite replaced successfully!");
    }

    private void RotateObject()
    {
        // 将对象沿 X 轴旋转 90 度
        transform.Rotate(90f, 0f, 0f);
        Debug.Log("Object rotated 90 degrees along the X axis.");
    }
}
