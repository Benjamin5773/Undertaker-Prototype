using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleEnter : MonoBehaviour
{
    [SerializeField] private Player player;

    [Header("Enemy Settings")]
    [SerializeField] private GameObject enemy;
    private void Start()
    {
        // Automatically find the player object by tag and get the Player script component
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.GetComponent<Player>();
        }
        else
        {
            Debug.LogError("Player object not found. Please ensure the player is tagged as 'Player'.");
        }

        if (enemy == null)
        {
            enemy = GameObject.FindWithTag("Enemy");
        }

        if (enemy != null)
        {
            enemy.SetActive(false); // Initially disable the enemy
        }
        else
        {
            Debug.LogError("Enemy object not found. Please ensure the enemy is tagged as 'Enemy'.");
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the player
        if (other.CompareTag("Player"))
        {
            // Call the EnterBattle method on the player script
            if (player != null)
            {
                player.DisableNarrativeCamera();
               // player.StartBattle();
               // Debug.Log("Player has entered the battle zone!");
                //if (enemy != null)
                //{
                   // enemy.SetActive(true); // Enable the enemy when entering battle
               // }
            }
        }
    }
}
