using System.Collections;
using UnityEngine;

public class BattleTrigger : MonoBehaviour
{
    [SerializeField] private Player player;

    [Header("Enemy Settings")]
    [SerializeField] private GameObject enemy;

    [Header("Object Settings")]
    [SerializeField] private GameObject obj1; // 第一个传入对象
    [SerializeField] private GameObject obj2; // 第二个传入对象

    [Header("Timing Settings")]
    [Tooltip("Time delay before enabling obj1 (in seconds)")]
    [SerializeField] private float obj1EnableDelay = 3.0f;

    [Tooltip("Time delay after enabling obj1 to disable it (in seconds)")]
    [SerializeField] private float obj1DisableDelay = 2.0f;

    [Tooltip("Time delay after disabling obj1 to enable obj2 and enemy (in seconds)")]
    [SerializeField] private float obj2AndEnemyEnableDelay = 1.0f;

    private bool hasTriggeredObj1 = false; // 标志 obj1 是否已经触发过

    private void Start()
    {
        // 自动查找 Player 对象并获取 Player 脚本组件
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.GetComponent<Player>();
        }
        else
        {
            Debug.LogError("Player object not found. Please ensure the player is tagged as 'Player'.");
        }

        // 自动查找 Enemy 对象并设置初始状态
        if (enemy == null)
        {
            enemy = GameObject.FindWithTag("Enemy");
        }

        if (enemy != null)
        {
            enemy.SetActive(false); // 初始禁用敌人
        }
        else
        {
            Debug.LogError("Enemy object not found. Please ensure the enemy is tagged as 'Enemy'.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 检查是否是玩家进入触发区
        if (other.CompareTag("Player"))
        {
            if (player != null)
            {
                player.DisableNarrativeCamera();
                player.StartBattle();
                Debug.Log("Player has entered the battle zone!");

                if (!hasTriggeredObj1 && obj1 != null)
                {
                    hasTriggeredObj1 = true; // 确保 obj1 只触发一次
                    StartCoroutine(HandleBattleSequence());
                }
            }
        }
    }

    private IEnumerator HandleBattleSequence()
    {
        // 延迟启用 obj1
        yield return new WaitForSeconds(obj1EnableDelay);
        if (obj1 != null)
        {
            obj1.SetActive(true);
            Debug.Log("Obj1 enabled after delay.");
        }

        // 延迟停用 obj1
        yield return new WaitForSeconds(obj1DisableDelay);
        if (obj1 != null)
        {
            obj1.SetActive(false);
            Debug.Log("Obj1 disabled after delay.");
        }

        // 延迟启用 obj2 和 enemy
        yield return new WaitForSeconds(obj2AndEnemyEnableDelay);
        if (obj2 != null)
        {
            obj2.SetActive(true);
            Debug.Log("Obj2 enabled.");
        }
        if (enemy != null)
        {
            enemy.SetActive(true);
            Debug.Log("Enemy enabled.");
        }
    }
}
