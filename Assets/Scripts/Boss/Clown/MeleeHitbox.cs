using UnityEngine;


public class MeleeHitbox : MonoBehaviour
{
    [SerializeField] float damage;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detected in melee hitbox"); // Verify detection
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                if (player.IsDashing())
                {
                    Debug.Log("Player dodged the attack!");
                    player.GainEnery();
                }
                else
                {
                    Debug.Log("Player hit by the attack!");
                    player.TakeDamage(damage); // Apply damage to the player
                }
            }
        }
    }
}