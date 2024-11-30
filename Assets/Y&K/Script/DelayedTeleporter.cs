using UnityEngine;

public class DelayedTeleporter : MonoBehaviour
{
    [Header("Player Settings")]
    [Tooltip("Tag of the player object to detect")]
    [SerializeField] string playerTag = "Player";

    [Header("Teleport Settings")]
    [Tooltip("The target marker object for teleport location")]
    [SerializeField] Transform targetMarker;

    [Tooltip("The GameObject to teleport")]
    [SerializeField] GameObject objectToTeleport;

    [Tooltip("Delay before teleporting (in seconds)")]
    [SerializeField] float teleportDelay = 3.0f;

    [Header("Effect Settings")]
    [Tooltip("The GameObject to activate temporarily after teleport")]
    [SerializeField] GameObject effectObject;

    [Tooltip("Duration for the effect object to remain active (in seconds)")]
    [SerializeField] float effectDuration = 3.0f;

    private bool isPlayerInZone = false; // 玩家是否在区域内
    private bool teleportScheduled = false; // 是否已经安排了传送
    private float timer = 0f; // 计时器

    private void Update()
    {
        if (isPlayerInZone && !teleportScheduled && targetMarker != null && objectToTeleport != null)
        {
            // 逐步累加计时器
            timer += Time.deltaTime;

            // 检查是否达到了延迟时间
            if (timer >= teleportDelay)
            {
                TeleportObject();
                teleportScheduled = true; // 防止多次传送
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 检测玩家进入触发区
        if (other.CompareTag(playerTag))
        {
            isPlayerInZone = true;
            teleportScheduled = false; // 重置传送状态
            timer = 0f; // 重置计时器
            Debug.Log("Player entered the teleport zone.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 检测玩家离开触发区
        if (other.CompareTag(playerTag))
        {
            isPlayerInZone = false;
            teleportScheduled = false; // 取消传送
            timer = 0f; // 重置计时器
            Debug.Log("Player left the teleport zone. Teleport cancelled.");
        }
    }

    private void TeleportObject()
    {
        if (targetMarker != null && objectToTeleport != null)
        {
            objectToTeleport.transform.position = targetMarker.position;
            Debug.Log("Object teleported to target marker.");
            ActivateEffectObject(); // 激活效果物体
        }
        else
        {
            Debug.LogWarning("Target marker or object to teleport is not set.");
        }
    }

    private void ActivateEffectObject()
    {
        if (effectObject != null)
        {
            effectObject.SetActive(true);
            Debug.Log("Effect object activated.");
            // 启用后一定时间后停用
            Invoke(nameof(DeactivateEffectObject), effectDuration);
        }
        else
        {
            Debug.LogWarning("Effect object is not assigned.");
        }
    }

    private void DeactivateEffectObject()
    {
        if (effectObject != null)
        {
            effectObject.SetActive(false);
            Debug.Log("Effect object deactivated.");
        }
    }
}
