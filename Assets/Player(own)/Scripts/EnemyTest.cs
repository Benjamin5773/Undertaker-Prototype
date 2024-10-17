using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections;

public class EnemyTest : MonoBehaviour
{
    public NavMeshAgent agent;

    [Header("Movement Settings")]
    public float normalMoveSpeed = 5.0f; // Normal movement speed
    public float slowFactor = 1000.0f; // The factor by which the speed is reduced
    private float currentMoveSpeed; // The current movement speed
    private Vector3 startPosition;
    [SerializeField] private Player player; // Reference to the player

    [Header("Health")]
    [SerializeField] private Canvas healthBarCanvas;
    [SerializeField] private Image healthBar;
    [SerializeField] private float maxHealth;
    private float currentHealth;

    [Header("Face Health Bar to Camera")]
    private Camera mainCamera;

    [Header("Damage Settings")]
    [SerializeField] private float frontDamage = 1f; // Damage taken from the front
    [SerializeField] private float sideDamage = 1.1f;  // Damage taken from the sides
    [SerializeField] private float backDamage = 1.5f;  // Damage taken from the back

    [Header("Chase Settings")]
    [SerializeField] private float chaseRange = 10f;  // Distance within which the enemy starts chasing the player
    [SerializeField] private float stoppingDistance = 3f;  // Distance at which the enemy stops moving toward the player

    [Header("Rotation Settings")]
    [SerializeField] private float normalRotationSpeed = 5f; // Normal rotation speed
    [SerializeField] private float currentRotationSpeed; // The current rotation speed

    [Header("Attack Settings")]
    [SerializeField] private float timeBetweenAttacks = 2.0f; // Time between consecutive attacks
    [SerializeField] private float meleeDamage = 10f;  // Damage for melee attacks
    [SerializeField] private float chargePrepareTime = 1f;  // Time before charging towards the player
    [SerializeField] private float chargeSpeed = 20f;  // Speed of the charge
    [SerializeField] private float chargeOffset = 3f;  // Distance behind the player to charge to
    [SerializeField] private int numberOfCharges = 2;
    [SerializeField] private float rangedDamage = 5f;  // Damage for ranged attacks
    private float currentChargeSpeed;
    private float currentChargePrepareTime;
    private bool alreadyAttacked;
    private bool isAttacking = false; // This flag will ensure only one attack at a time
    [Header("Teleport")]
    Vector3 playerDirection;
    [SerializeField] float teleportDistance = 3f;

    private void Start()
    {
        currentChargeSpeed = chargeSpeed;
        currentChargePrepareTime = chargePrepareTime;
        agent = GetComponent<NavMeshAgent>();
        startPosition = transform.position;
        currentMoveSpeed = normalMoveSpeed; // Start with normal movement speed
        currentRotationSpeed = normalRotationSpeed; // Start with normal rotation speed
        currentHealth = maxHealth; // Initialize enemy's health
        UpdateHealthBar(); // Set the health bar to full at the start

        // Disable NavMeshAgent's automatic rotation so we can manually control it
        agent.updateRotation = false;

        // Automatically find the player object by tag
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.GetComponent<Player>();
        }
        else
        {
            Debug.LogError("Player object not found. Ensure the player has the tag 'Player'.");
        }

        // Find the Camera
        mainCamera = Camera.main;

        // Initially hide the health bar
        if (healthBarCanvas != null)
        {
            healthBarCanvas.enabled = false;
        }
    }

    private void Update()
    {
        // Adjust the movement and rotation speed based on the player's energy
        currentMoveSpeed = player.IsEnergyFull() ? normalMoveSpeed / slowFactor : normalMoveSpeed;
        currentRotationSpeed = player.IsEnergyFull() ? normalRotationSpeed / slowFactor : normalRotationSpeed;
        currentChargeSpeed = player.IsEnergyFull() ? chargeSpeed / slowFactor : chargeSpeed;
        currentChargePrepareTime = player.IsEnergyFull() ? chargePrepareTime / slowFactor : chargePrepareTime;

        // Display the health bar only when the player's energy is full
        if (player.IsEnergyFull())
        {
            if (healthBarCanvas != null)
            {
                healthBarCanvas.enabled = true; // Show the health bar
            }

            // Make the health bar face the camera
            if (healthBarCanvas != null)
            {
                AlignHealthBarWithCamera();
            }
        }
        else
        {
            if (healthBarCanvas != null)
            {
                healthBarCanvas.enabled = false; // Hide the health bar
            }
        }

        // Move the enemy toward the player
        ChasePlayer();
    }

     IEnumerator StopMoving(float stopTime)
    {
        float savedSpeed = currentMoveSpeed;
        currentMoveSpeed = 0;
        yield return new WaitForSeconds(stopTime);
        currentMoveSpeed= savedSpeed;
    }
    private void ChasePlayer()
    {
        if (agent != null && player != null)
        {
            // Set the player's position as the destination for the agent
            agent.SetDestination(player.transform.position);

            // Stop the enemy a short distance away from the player
            agent.stoppingDistance = stoppingDistance;

            if (agent.remainingDistance <= stoppingDistance && !isAttacking) // Ensure no attack in progress
            {
                agent.speed = 0; // Stop movement when within stopping distance
                PerformAttackDecision(); // Decide and perform an attack when close to the player
            }
            else
            {
                agent.speed = currentMoveSpeed; // Continue chasing at normal speed
            }

            // Handle rotation towards the player
            RotateTowardsPlayer();
        }
    }

    // Attack decision logic after reaching stopping distance
    private void PerformAttackDecision()
    {
        // If the enemy hasn't already attacked and is not attacking, choose an attack action
        if (!alreadyAttacked && !isAttacking)
        {
            isAttacking = true; // Set the attack flag

            // Randomly choose between melee and ranged attack
            int attackType = Random.Range(0, 2); // 0 for melee, 1 for ranged

            if (attackType == 0)
            {
                PerformMeleeAttack(); // Perform melee attack
            }
            else
            {
                PerformRangedAttack(); // Perform ranged attack
            }

            // Set the attack cooldown
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void PerformMeleeAttack()
    {
        StartCoroutine(MeleeChargeAttack(numberOfCharges));
    }

    private IEnumerator MeleeChargeAttack(int times)
    {
        while (times > 0)
        {
            Vector3 playerDirectionXZ = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z); 
            transform.forward = playerDirectionXZ - transform.position;

            Debug.Log("Charging...");
            StartCoroutine(StopMoving(chargePrepareTime));
            yield return new WaitForSeconds(chargePrepareTime);

            Vector3 targetPos = player.transform.position + playerDirection * chargeOffset;

            while (Vector3.Distance(transform.position, targetPos) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, currentChargeSpeed * Time.deltaTime);
                yield return null;
            }

            times--;
        }

        Debug.Log("Finished all charges.");
        isAttacking = false; // Reset attack flag after the attack
    }

    private void PerformRangedAttack()
    {
        Debug.Log("Performing ranged attack!");
        StartCoroutine(MeleeAttack2(1));
    }

    IEnumerator MeleeAttack2(int attackType)
    {
        // Save original position to maintain reference for enemy's starting point
        Vector3 originalPosition = transform.position;

        // Calculate the teleport position near the player
        Vector3 teleportPosition = player.transform.position + player.transform.forward * teleportDistance;

        // Teleport to the calculated position near the player
        transform.position = teleportPosition;

        // Face the player after teleporting
        Vector3 playerDirectionXZ = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.forward = playerDirectionXZ - transform.position; // Face the player

        // Pause for 1.5 seconds to simulate the enemy preparing an attack (adjust as needed)
        Debug.Log("Enemy paused for melee attack...");
        yield return new WaitForSeconds(1.5f);

        if (attackType == 1)
        {
            // Melee attack
        }
        else if (attackType == 2)
        {
            StartCoroutine(MeleeChargeAttack(numberOfCharges-1));
        }

        // Reset attack flag after the attack
        isAttacking = false;
    }

    private void ResetAttack()
    {
        alreadyAttacked = false; // Reset the attack flag
    }

    // Rotate the enemy towards the player smoothly
    private void RotateTowardsPlayer()
    {
        if (player != null)
        {
            // Calculate the direction to the player
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));

            // Smoothly rotate the enemy towards the player
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * currentRotationSpeed);
        }
    }

    // Function to calculate damage based on the player's relative position (front, side, or back)
    private float CalculateDamageBasedOnPosition()
    {
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        float dotProduct = Vector3.Dot(transform.forward, directionToPlayer); // Dot product between enemy forward and direction to player

        if (dotProduct > 0.5f) // Front (dot product near 1)
        {
            Debug.Log("Hit from the front");
            return frontDamage;
        }
        else if (dotProduct < -0.7f) // Back (dot product near -1)
        {
            Debug.Log("Hit from the back");
            return backDamage;
        }
        else // Sides (dot product near 0)
        {
            Debug.Log("Hit from the side");
            return sideDamage;
        }
    }

    // Function to update the health bar based on the current health
    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.fillAmount = currentHealth / maxHealth; // Update the fill amount of the health bar
        }
    }

    // Function to handle the enemy's death
    private void Die()
    {
        Debug.Log("Enemy has died");
        Destroy(gameObject);
    }

    // Rotate the health bar to face the camera
    private void AlignHealthBarWithCamera()
    {
        if (mainCamera != null && healthBarCanvas != null)
        {
            healthBarCanvas.transform.LookAt(mainCamera.transform);
            healthBarCanvas.transform.Rotate(0, 180, 0); // Correct the rotation
        }
    }

    // Apply damage and calculate the damage based on position
    public void TakeDamage(float damageAmount)
    {
        float actualDamage = CalculateDamageBasedOnPosition() * damageAmount;
        Debug.Log("Damage taken: " + actualDamage);
        currentHealth -= actualDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Clamp health

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die(); // Handle death
        }
    }

    public void TakeDebuff(float Debuff)
    {
    }
}
