using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportPlayer : MonoBehaviour
{
    [Header("Player Settings")]
    [Tooltip("Tag of the player object to detect")]
    [SerializeField] string playerTag = "Player";

    [Header("Teleport Settings")]
    [Tooltip("Name of the target scene to teleport to")]
    [SerializeField] string targetSceneName = "Scene1";

    [Header("Optional Settings")]
    [Tooltip("Delay before teleporting (in seconds)")]
    [SerializeField] float teleportDelay = 0f;

    private bool hasTriggered = false; // 标志是否已经触发过

    private void OnTriggerEnter(Collider other)
    {
        // 检查是否是玩家，并且是否已经触发过
        if (!hasTriggered && other.CompareTag(playerTag))
        {
            hasTriggered = true; // 设置标志为 true
            StartTeleport();
        }
    }

    private void StartTeleport()
    {
        if (teleportDelay > 0)
        {
            Invoke(nameof(TeleportToScene), teleportDelay);
        }
        else
        {
            TeleportToScene();
        }
    }

    private void TeleportToScene()
    {
        if (!string.IsNullOrEmpty(targetSceneName))
        {
            SceneManager.LoadScene(targetSceneName);
        }
        else
        {
            Debug.LogWarning("Target scene name is not set!");
        }
    }
}
