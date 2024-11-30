// using System.Collections;
// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.AI;

// public class EnemyTest : MonoBehaviour
// {
//     public NavMeshAgent agent;

//     [Header("Player Reference")]
//     [SerializeField] private Player player; // Reference to the player

//     [Header("Movement Settings")]
//     public float normalMoveSpeed = 5.0f; // Normal movement speed
//     public float slowFactor = 1000.0f; // The factor by which the speed is reduced
//     private float currentMoveSpeed; // The current movement speed
//     private float currentRotationSpeed; // The current rotation speed

//     [Header("Health")]
//     [SerializeField] private Canvas healthBarCanvas;
//     [SerializeField] private Image healthBar;
//     [SerializeField] private float maxHealth = 100f;
//     private float currentHealth;

//     [Header("Damage Settings")]
//     [SerializeField] private float frontDamage = 1f; // Damage taken from the front
//     [SerializeField] private float sideDamage = 1.1f;  // Damage taken from the sides
//     [SerializeField] private float backDamage = 1.5f;  // Damage taken from the back

//     [Header("Chase Settings")]
//     [SerializeField] private float chaseRange = 100f;  // Distance within which the enemy starts chasing the player
//     [SerializeField] private float stoppingDistance = 3f;  // Distance at which the enemy stops moving toward the player

//     [Header("Attack Settings")]
//     [SerializeField] private float timeBetweenAttacks = 2.0f;
//     [SerializeField] private float meleeDamage = 10f;
//     [SerializeField] private float chargePrepareTime = 1f;
//     [SerializeField] private float chargeSpeed = 20f;
//     [SerializeField] private float chargeOffset = 3f;
//     [SerializeField] private int numberOfCharges = 2;
//     [SerializeField] private float rangedDamage = 5f;



//     private float currentChargeSpeed;
//     private float currentChargePrepareTime;
//     private bool alreadyAttacked;
//     private bool isAttacking = false;
//     private Camera mainCamera;

 
   

//     private void Start()
//     {
//         currentHealth = maxHealth;
//         currentChargeSpeed = chargeSpeed;
//         currentChargePrepareTime = chargePrepareTime;
//         currentRotationSpeed = normalMoveSpeed;

//        //  agent = GetComponent<NavMeshAgent>();
//        // agent.updateRotation = false; // Manual rotation control

//         // Automatically find player by tag
//         GameObject playerObject = GameObject.FindWithTag("Player");
//         if (playerObject != null) player = playerObject.GetComponent<Player>();
        
//         mainCamera = Camera.main;

//         // Initially hide the health bar
//         if (healthBarCanvas != null) healthBarCanvas.enabled = false;
//     }

//     private void Update()
//     {
//         currentMoveSpeed = player.IsEnergyFull() ? normalMoveSpeed / slowFactor : normalMoveSpeed;
//         currentChargeSpeed = player.IsEnergyFull() ? chargeSpeed / slowFactor : chargeSpeed;
//         currentChargePrepareTime = player.IsEnergyFull() ? chargePrepareTime / slowFactor : chargePrepareTime;

//         // Display or hide health bar based on player's energy
//         healthBarCanvas.enabled = player.IsEnergyFull();

//         if (healthBarCanvas != null && mainCamera != null) AlignHealthBarWithCamera();


//     }

   
    
//     private void RotateTowardsPlayer()
//     {
//         Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
//         Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
//         transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * currentRotationSpeed);
//     }

//     private void AlignHealthBarWithCamera()
//     {
//         healthBarCanvas.transform.LookAt(mainCamera.transform);
//         healthBarCanvas.transform.Rotate(0, 180, 0);
//     }

//     public void TakeDamage(float damageAmount)
//     {
//         float actualDamage = CalculateDamageBasedOnPosition() * damageAmount;
//         currentHealth -= actualDamage;
//         currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
//         UpdateHealthBar();

//         if (currentHealth <= 0) Die();
//     }
//     public void TakeDebuff(float damageAmount) {}

//     private float CalculateDamageBasedOnPosition()
//     {
//         Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
//         float dotProduct = Vector3.Dot(transform.forward, directionToPlayer);

//         if (dotProduct > 0.5f) return frontDamage;
//         else if (dotProduct < -0.7f) return backDamage;
//         else return sideDamage;
//     }

//     private void UpdateHealthBar()
//     {
//         if (healthBar != null) healthBar.fillAmount = currentHealth / maxHealth;
//     }

//     private void Die()
//     {
//         Debug.Log("Enemy has died");
//         Destroy(gameObject);
//     }
// }
