using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleTrigger : MonoBehaviour
{
    private Player player;
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
    }
  private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the player
        if (other.CompareTag("Player"))
        {
            // Call the EnterBattle method on the player script
            if (player != null)
            {
                Debug.Log("Player has entered the battle zone!");
            }
        }
    }
}
