using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleLeave : MonoBehaviour
{
    [SerializeField] private Player player;

    [Header("Enemy Settings")]
    [Tooltip("The enemy GameObject to disable")]
    [SerializeField] private GameObject enemy;

    [Header("Object Settings")]
    [Tooltip("An additional GameObject to disable when the player leaves")]
    [SerializeField] private GameObject objToDisable;

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
                player.EnableNarrativeCamera();
                Debug.Log("Player has left the battle zone.");

                // 禁用敌人
                if (enemy != null)
                {
                    enemy.SetActive(false);
                    Debug.Log("Enemy has been disabled.");
                }

                // 禁用额外传入的对象
                if (objToDisable != null)
                {
                    objToDisable.SetActive(false);
                    Debug.Log($"{objToDisable.name} has been disabled.");
                }
            }
        }
    }
}
