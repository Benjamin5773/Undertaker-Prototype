using UnityEngine;

public class SingleTriggerSlowMotionZone : MonoBehaviour
{
    [Header("Time Settings")]
    [Tooltip("Time scale to set when the player enters the zone (0.5 for half speed)")]
    [SerializeField] private float slowMotionScale = 0.5f;

    [Tooltip("Default time scale when leaving slow motion (usually 1.0)")]
    [SerializeField] private float normalTimeScale = 1.0f;

    [Header("Object Settings")]
    [Tooltip("GameObject to activate during slow motion")]
    [SerializeField] private GameObject objectToActivate;

    private bool hasTriggered = false; // 标志是否已经触发过

    private void OnTriggerEnter(Collider other)
    {
        // 检查是否是玩家进入触发区，并且未触发过
        if (other.CompareTag("Player") && !hasTriggered)
        {
            ActivateSlowMotion();
            hasTriggered = true; // 标记为已触发
        }
    }

    private void Update()
    {
        // 当处于慢动作状态时，检测玩家是否按下空格键
        if (Time.timeScale == slowMotionScale && Input.GetKeyDown(KeyCode.Space))
        {
            DeactivateSlowMotion();
        }
    }

    private void ActivateSlowMotion()
    {
        // 设置游戏时间为慢动作
        Time.timeScale = slowMotionScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // 调整物理更新步长

        // 启用指定的对象
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }

        Debug.Log("Slow motion activated.");
    }

    private void DeactivateSlowMotion()
    {
        // 恢复游戏时间为正常速度
        Time.timeScale = normalTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // 恢复物理更新步长

        // 停用指定的对象
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(false);
        }

        Debug.Log("Slow motion deactivated.");
    }
}
