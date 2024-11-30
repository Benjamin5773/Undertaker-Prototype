using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Boss : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject chargeDestinationMarkerPrefab;
    [SerializeField] private GameObject balloonPrefab; // Projectile prefab for ranged attack
    [SerializeField] private Transform firePoint; // Position to spawn the projectile

    [Header("Basic")]
    [SerializeField] private float bossMaxHealth = 100f;
    [SerializeField] private float bossMoveSpeed = 5f;
    private float currentMoveSpeed;
    [SerializeField] private float bossHealth;

    [Header("Slowdown Factor")]
    [SerializeField] private float slowFactor = 1000.0f;

    [Header("Charge Attack")]
    [SerializeField] private float chargePrepareTime = 1f;
    [SerializeField] private float chargeSpeed = 25f;
    private float currentChargeSpeed;
    private float currentChargePrepareTime;
    [SerializeField] private float chargeOffset = 3f;

    [Header("Ranged Attack")]
    [SerializeField] private float rangedAttackCooldown = 8f;
    [SerializeField] private float rangedAttackRange = 15f;
    private float rangedAttackTimer;

    [Header("Action Cooldown")]
    [SerializeField] private float minActionCooldown = 5f;
    [SerializeField] private float maxActionCooldown = 10f;
    [SerializeField] private float closeRangeThreshold = 10f;
    private float actionCooldownTimer;

    [Header("Damage Settings")]
    [SerializeField] private float frontDamage = 1f;
    [SerializeField] private float sideDamage = 1.1f;
    [SerializeField] private float backDamage = 1.5f;
    public float damageMultiplier = 1.0f;

    [Header("UI and Effects")]
    [SerializeField] private Canvas healthBarCanvas;
    [SerializeField] private Image healthBar;
    [SerializeField] private ParticleSystem chargeEffect;
    [SerializeField] private ParticleSystem meleeEffect;

    [Header("Attack Hitboxes")]
    [SerializeField] private GameObject meleeHitbox;

    private Animator animator;
    private NavMeshAgent agent;
    private Camera mainCamera;
    private bool isAttacking = false;

    private float playerDistance;
    private Vector3 playerDirection;

    private bool isPerformingChargeAttack = false;

    public enum State
    {
        cooldown,
        makeDecision,
        closeRangeAttack,
        chargeAttack,
        rangedAttack,
        paused
    }
    public State currentState = State.makeDecision;

    void Start()
    {
        bossHealth = bossMaxHealth;
        actionCooldownTimer = 5f;
        rangedAttackTimer = rangedAttackCooldown;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;

        currentMoveSpeed = bossMoveSpeed;
        currentChargeSpeed = chargeSpeed;
        currentChargePrepareTime = chargePrepareTime;

        //if (healthBarCanvas != null) healthBarCanvas.enabled = false;
    }

    private void Update()
    {
        UpdateState();
        DisplayHealthBarBasedOnPlayerEnergy();
    }

    private void UpdateState()
    {
        Debug.Log($"Current State: {currentState}, Ranged Attack Timer: {rangedAttackTimer:F2}");

        AdjustSpeedsBasedOnPlayerEnergy();

        // Paused state if the player's energy is full
        if (player.GetComponent<Player>().IsEnergyFull())
        {
            if (currentState != State.paused)
            {
                StartCoroutine(PauseBossWhilePlayerEnergyFull());
            }
            return;
        }

        switch (currentState)
        {
            case State.cooldown:
                actionCooldownTimer -= Time.deltaTime;
                rangedAttackTimer -= Time.deltaTime;

                if (actionCooldownTimer <= 0 || rangedAttackTimer <= 0)
                {
                    currentState = State.makeDecision;
                }
                break;

            case State.makeDecision:
                if (!isAttacking)
                    MakeDecision();
                break;

            case State.closeRangeAttack:
                if (!isAttacking)
                    PerformCloseRangeAttack();
                break;

            case State.chargeAttack:
                if (!isAttacking)
                    StartCoroutine(PerformChargeAttack());
                break;

            case State.rangedAttack:
                if (!isAttacking)
                    RangedAttack();
                break;
        }
    }

    private void FixedUpdate()
    {
        if (currentState != State.paused)
        {
            GetDistanceDirection();
            SetAgentMovement();
        }
    }

    private IEnumerator PauseBossWhilePlayerEnergyFull()
    {
        currentState = State.paused;
        float savedMoveSpeed = currentMoveSpeed;
        currentMoveSpeed = 0f;

        animator.SetBool("Run", false);
        agent.isStopped = true;

        while (player.GetComponent<Player>().IsEnergyFull())
        {
            yield return null;
        }

        currentMoveSpeed = savedMoveSpeed;
        currentState = State.cooldown;
        animator.SetBool("Run", true);
        agent.isStopped = false;
    }

    private void AdjustSpeedsBasedOnPlayerEnergy()
    {
        bool isPlayerEnergyFull = player.GetComponent<Player>().IsEnergyFull();
        currentMoveSpeed = isPlayerEnergyFull ? bossMoveSpeed / slowFactor : bossMoveSpeed;
        currentChargeSpeed = isPlayerEnergyFull ? chargeSpeed / slowFactor : chargeSpeed;
        currentChargePrepareTime = isPlayerEnergyFull ? chargePrepareTime / slowFactor : chargePrepareTime;
    }

    private void DisplayHealthBarBasedOnPlayerEnergy()
    {
 //       healthBarCanvas.enabled = player.GetComponent<Player>().IsEnergyFull();
  //      if (healthBarCanvas.enabled) AlignHealthBarWithCamera();
    }

    private void SetAgentMovement()
    {
        agent.destination = player.transform.position;
        agent.speed = currentMoveSpeed;
        animator.SetBool("Run", agent.velocity.magnitude > 0.1f);
    }

    private void GetDistanceDirection()
    {
        Vector3 playerXZ = new Vector3(player.transform.position.x, 0, player.transform.position.z);
        Vector3 selfXZ = new Vector3(transform.position.x, 0, transform.position.z);
        playerDistance = Vector3.Distance(playerXZ, selfXZ);
        playerDirection = (playerXZ - selfXZ).normalized;
    }

    private void MakeDecision()
    {
        Debug.Log($"Boss is making a decision... Ranged Timer: {rangedAttackTimer:F2}, Action Timer: {actionCooldownTimer:F2}");

        // If the ranged attack is available, prioritize it
        if (playerDistance <= rangedAttackRange && rangedAttackTimer <= 0)
        {
            currentState = State.rangedAttack;
            Debug.Log("Boss chose to perform a ranged attack.");
            return;
        }

        // Prioritize charge attack if health is low and player is not in close range
        if (bossHealth < (0.5f * bossMaxHealth) && playerDistance > closeRangeThreshold)
        {
            currentState = State.chargeAttack;
            Debug.Log("Boss chose to perform a charge attack.");
            return;
        }

        // If player is within close range, perform close-range attack
        if (playerDistance <= closeRangeThreshold)
        {
            currentState = State.closeRangeAttack;
            Debug.Log("Boss chose to perform a close-range attack.");
            return;
        }

        // Default to cooldown state
        currentState = State.cooldown;
        Debug.Log("Boss is cooling down.");
    }
    //Call player.GetComponent<Player>().TakeDamage(x); when player takes damage
    private void PerformCloseRangeAttack()
    {
        if (isAttacking) return; // Prevent multiple attacks
        isAttacking = true; // Set flag to indicate attacking

        Debug.Log("Boss is preparing for a close-range attack.");
        agent.isStopped = true;
        animator.SetBool("Run", false);
        animator.SetTrigger("MeleeAttack"); // Trigger melee attack animation

        // Use a coroutine to delay the activation of the hitbox and melee effect
        StartCoroutine(ActivateMeleeHitboxWithDelay());
        ResetActionCooldown();
    }

    private IEnumerator ActivateMeleeHitboxWithDelay()
    {
        yield return new WaitForSeconds(0.6f); // Wait 0.6 seconds before activating the hitbox and effect

        meleeHitbox.SetActive(true); // Enable the hitbox
        meleeEffect.Play(); // Play the melee effect
        Debug.Log("Melee hitbox and effect activated.");

        // Disable the hitbox after a short duration
        yield return new WaitForSeconds(0.5f); // Duration the melee hitbox remains active
        meleeHitbox.SetActive(false); // Deactivate hitbox
        agent.isStopped = false;
        animator.SetBool("Run", true);
        Debug.Log("Melee hitbox deactivated.");
    }
    private IEnumerator PerformChargeAttack()
    {
        if (isAttacking) yield break; // Prevent multiple attacks
        isAttacking = true; // Set flag to indicate attacking
        if (isPerformingChargeAttack)
        {
            yield break; // Exit if the charge attack is already in progress
        }

        isPerformingChargeAttack = true; // Set the flag
        Debug.Log("Boss is preparing for a charge attack.");

        // Calculate the direction and initial target position
        Vector3 playerPosition = player.transform.position;
        Vector3 playerDirectionXZ = new Vector3(playerPosition.x, transform.position.y, playerPosition.z); // Ignore Y axis
        transform.forward = (playerDirectionXZ - transform.position).normalized; // Face the player

        Vector3 targetPos = playerPosition + transform.forward * chargeOffset;

        // Create a LayerMask to include walls and exclude the player
        LayerMask layerMask = LayerMask.GetMask("Wall", "Character"); // Adjust layers as necessary

        // Debug the raycast direction
        Debug.DrawRay(transform.position, transform.forward * chargeOffset, Color.red, 2f);

        // Perform a raycast with the LayerMask
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, chargeOffset, layerMask))
        {
            Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.yellow, 2f);
            Debug.Log($"Raycast hit: {hit.collider.name} at {hit.point}"); // Debug the hit object

            if (hit.collider.CompareTag("Wall")) // Check if it's a wall
            {
                Debug.Log("Wall detected during charge. Adjusting target position.");
                // Set the target position slightly in front of the wall
                targetPos = hit.point - transform.forward * 0.5f; // Offset by 0.5 units to prevent overlap
            }
        }
        else
        {
            Debug.DrawRay(transform.position, transform.forward * chargeOffset, Color.white, 2f);
            Debug.Log("Raycast did not hit any object.");
        }

        // Place the shadow marker at the adjusted target position
        if (chargeDestinationMarkerPrefab != null)
        {
            GameObject chargeMarker = Instantiate(chargeDestinationMarkerPrefab, targetPos, Quaternion.LookRotation(transform.forward));
            Debug.Log($"Placing shadow marker at {targetPos}.");
            Destroy(chargeMarker, chargePrepareTime); // Auto-destroy after preparation time
        }

        // Pause for preparation
        yield return StartCoroutine(StopMoving(currentChargePrepareTime));

        // Activate the hitbox and play charge effect
        meleeHitbox.SetActive(true); // Enable the hitbox during charge
        chargeEffect.Play();
        Debug.Log("Boss started charging. Hitbox activated.");

        float chargeStartTime = Time.time;

        // Charge movement
        while (Vector3.Distance(transform.position, targetPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, currentChargeSpeed * Time.deltaTime);

            // Timeout to prevent infinite loop
            if (Time.time - chargeStartTime > 5f) // 5 seconds max for charge
            {
                Debug.LogWarning("Charge attack timeout reached.");
                break;
            }

            yield return null;
        }

        // Stop charge effect and deactivate the hitbox
        chargeEffect.Stop();
        meleeHitbox.SetActive(false); // Disable the hitbox after charge
        Debug.Log("Boss charge attack completed. Hitbox deactivated.");

        // Reset cooldown for next action
        ResetActionCooldown();

        isPerformingChargeAttack = false; // Reset the flag
    }



    private void RangedAttack()
    {
        if (isAttacking) return; // Prevent multiple attacks
        isAttacking = true; // Set flag to indicate attacking

        Debug.Log("Boss is performing a ranged attack.");

        // Play the ranged attack animation
        animator.SetBool("IsRanged", true);

        // Wait for the animation event to spawn the projectile
        StartCoroutine(SpawnBalloonWithDelay());
    }
    private IEnumerator SpawnBalloonWithDelay()
    {
        yield return new WaitForSeconds(0.5f); // Delay to sync with animation (adjust as needed)

        if (balloonPrefab != null)
        {
            // Instantiate the balloon prefab with a Y offset
            Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z); // Adjust Y offset
            Instantiate(balloonPrefab, spawnPosition, Quaternion.identity);
            Debug.Log("Balloon projectile spawned.");
        }

        // Reset action cooldown with ranged attack cooldown
        ResetActionCooldown(true);
        animator.SetBool("IsRanged", false);
        isAttacking = false;
    }
    private void ResetActionCooldown(bool resetRangedAttack = false)
    {
        actionCooldownTimer = UnityEngine.Random.Range(minActionCooldown, maxActionCooldown);

        if (resetRangedAttack)
        {
            rangedAttackTimer = rangedAttackCooldown; // Reset only for ranged attacks
        }

        currentState = State.cooldown;
        isAttacking = false; // Reset the attacking flag
    }
    private void AlignHealthBarWithCamera()
    {
        healthBarCanvas.transform.LookAt(mainCamera.transform);
        healthBarCanvas.transform.Rotate(0, 180, 0);
    }

    public void TakeDamage(float damageAmount)
    {
        float actualDamage = CalculateDamageBasedOnPosition() * damageAmount * damageMultiplier;
        bossHealth -= actualDamage;
        bossHealth = Mathf.Clamp(bossHealth, 0, bossMaxHealth);
        UpdateHealthBar();

        if (bossHealth <= 0) Die();
    }

    public void SetDamageMultiplier(float multiplier)
    {
        damageMultiplier = multiplier;
        Debug.Log($"Boss damage multiplier set to {damageMultiplier}");
    }

    private float CalculateDamageBasedOnPosition()
    {
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
        float dotProduct = Vector3.Dot(transform.forward, directionToPlayer);

        if (dotProduct > 0.5f) return frontDamage;
        else if (dotProduct < -0.7f) return backDamage;
        else return sideDamage;
    }

    private void UpdateHealthBar()
    {
        if (healthBar != null) healthBar.fillAmount = bossHealth / bossMaxHealth;
    }

    private void Die()
    {
        Debug.Log("Boss has died.");
        Destroy(gameObject);
    }

    private IEnumerator StopMoving(float stopTime)
    {
        agent.isStopped = true;
        animator.SetBool("Run", false);
        yield return new WaitForSeconds(stopTime);
        agent.isStopped = false;
        animator.SetBool("Run", true);
    }

}





// using System.Collections;
// using TMPro;
// using UnityEngine;
// using UnityEngine.AI;
// using UnityEngine.UI;

// public class Boss : MonoBehaviour
// {
//     [Header("Prefabs")]
//     [SerializeField] private GameObject player;
//     [SerializeField] private GameObject bossShadowPrefab;

//     [Header("Basic")]
//     [SerializeField] private float bossMaxHealth = 100f;
//     [SerializeField] private float bossMoveSpeed = 5f;
//     private float currentMoveSpeed;
//     private float bossHealth;

//     [Header("Slowdown Factor")]
//     [SerializeField] private float slowFactor = 1000.0f;

//     [Header("Teleport")]
//     [SerializeField] private float teleportDistance = 3f;
//     [SerializeField] private float teleportDelay = 2f;
//     private float currentTeleportDelay;

//     [Header("Charge Attack")]
//     [SerializeField] private float chargePrepareTime = 1f;
//     [SerializeField] private float chargeSpeed = 25f;
//     private float currentChargeSpeed;
//     private float currentChargePrepareTime;
//     [SerializeField] private float chargeOffset = 3f;

//     [Header("Action Cooldown")]
//     [SerializeField] private float minActionCooldown = 5f;
//     [SerializeField] private float maxActionCooldown = 10f;
//     [SerializeField] private float closeRangeThreshold = 10f;
//     private float actionCooldownTimer;

//     [Header("Damage Settings")]
//     [SerializeField] private float frontDamage = 1f;
//     [SerializeField] private float sideDamage = 1.1f;
//     [SerializeField] private float backDamage = 1.5f;

//     [Header("UI and Effects")]
//     [SerializeField] private Canvas healthBarCanvas;
//     [SerializeField] private Image healthBar;
//     [SerializeField] private ParticleSystem teleportEffect;
//     [SerializeField] private ParticleSystem chargeEffect;

//     private Animator animator;
//     private NavMeshAgent agent;
//     private Camera mainCamera;
//     private bool isAttacking = false;
//     private bool alreadyAttacked = false;

//     private float playerDistance;
//     private Vector3 playerDirection;

//     public enum State
//     {
//         idle,
//         cooldown,
//         makeDecision,
//         doAction,
//         paused
//     }
//     public State currentState = State.idle;

//     void Start()
//     {
//         bossHealth = bossMaxHealth;
//         actionCooldownTimer = 5f;
//         agent = GetComponent<NavMeshAgent>();
//         animator = GetComponent<Animator>();
//         mainCamera = Camera.main;

//         currentMoveSpeed = bossMoveSpeed;
//         currentChargeSpeed = chargeSpeed;
//         currentChargePrepareTime = chargePrepareTime;
//         currentTeleportDelay = teleportDelay;

//         if (healthBarCanvas != null) healthBarCanvas.enabled = false;
//     }

//     private void Update()
//     {
//         UpdateState(); 
//         DisplayHealthBarBasedOnPlayerEnergy();
//     }

//     private void UpdateState()
//     {
//         AdjustSpeedsBasedOnPlayerEnergy();

//         // Paused state if the player's energy is full
//         if (player.GetComponent<Player>().IsEnergyFull())
//         {
//             if (currentState != State.paused)
//             {
//                 StartCoroutine(PauseBossWhilePlayerEnergyFull());
//             }
//             return;
//         }

//         // If not paused, check and handle state transitions
//         switch (currentState)
//         {
//             case State.cooldown:
//                 actionCooldownTimer -= Time.deltaTime;
//                 if (actionCooldownTimer <= 0)
//                 {
//                     currentState = State.makeDecision;
//                 }
//                 break;

//             case State.makeDecision:
//                 MakeDecision();
//                 currentState = State.doAction;
//                 break;

//             case State.doAction:
//                 PerformAction();
//                 break;

//             case State.idle:
//                 // Transition from idle to cooldown as default
//                 currentState = State.cooldown;
//                 break;
//         }
//     }

//     private void FixedUpdate()
//     {
//         if (currentState != State.paused)
//         {
//             GetDistanceDirection();
//             SetAgentMovement();
//         }
//     }

//     private IEnumerator PauseBossWhilePlayerEnergyFull()
//     {
//         currentState = State.paused;
//         float savedMoveSpeed = currentMoveSpeed;
//         currentMoveSpeed = 0f;

//         animator.SetBool("Run", false);

//         while (player.GetComponent<Player>().IsEnergyFull())
//         {
//             yield return null;
//         }

//         currentMoveSpeed = savedMoveSpeed;
//         currentState = State.cooldown;
//         animator.SetBool("Run", true);
//     }

//     private void AdjustSpeedsBasedOnPlayerEnergy()
//     {
//         bool isPlayerEnergyFull = player.GetComponent<Player>().IsEnergyFull();
//         currentMoveSpeed = isPlayerEnergyFull ? bossMoveSpeed / slowFactor : bossMoveSpeed;
//         currentChargeSpeed = isPlayerEnergyFull ? chargeSpeed / slowFactor : chargeSpeed;
//         currentChargePrepareTime = isPlayerEnergyFull ? chargePrepareTime / slowFactor : chargePrepareTime;
//         currentTeleportDelay = isPlayerEnergyFull ? teleportDelay * slowFactor : teleportDelay;
//     }

//     private void DisplayHealthBarBasedOnPlayerEnergy()
//     {
//         healthBarCanvas.enabled = player.GetComponent<Player>().IsEnergyFull();
//         if (healthBarCanvas.enabled) AlignHealthBarWithCamera();
//     }

//     private void SetAgentMovement()
//     {
//         agent.destination = player.transform.position;
//         agent.speed = currentMoveSpeed;
//         animator.SetBool("Run", agent.velocity.magnitude > 0.1f);
//     }

//     private void GetDistanceDirection()
//     {
//         Vector3 playerXZ = new Vector3(player.transform.position.x, 0, player.transform.position.z);
//         Vector3 selfXZ = new Vector3(transform.position.x, 0, transform.position.z);
//         playerDistance = Vector3.Distance(playerXZ, selfXZ);
//         playerDirection = (playerXZ - selfXZ).normalized;
//     }

//     private void MakeDecision()
//     {
//         if (bossHealth >= (2.0f / 3.0f) * bossMaxHealth)
//         {
//             if (playerDistance > closeRangeThreshold) StartCoroutine(PrepareTeleport(1));
//             else CloseRangeAttack();
//         }
//         else if (bossHealth >= (1.0f / 3.0f) * bossMaxHealth)
//         {
//             if (playerDistance > closeRangeThreshold) StartCoroutine(PrepareTeleport(2));
//             else StartCoroutine(ChargeAttack());
//         }
//         else
//         {
//             StartCoroutine(ChargeAttack());
//         }
//     }

//     private void PerformAction()
//     {
//         ResetActionCooldown();
//         currentState = State.cooldown;
//     }

//     private void ResetActionCooldown()
//     {
//         actionCooldownTimer = Random.Range(minActionCooldown, maxActionCooldown);
//         currentState = State.cooldown;
//     }

//     private IEnumerator PrepareTeleport(int attackType)
//     {
//         yield return new WaitForSeconds(currentTeleportDelay);
//         StartCoroutine(TeleportAttack(attackType));
//     }

//     private IEnumerator TeleportAttack(int attackType)
//     {
//         Vector3 teleportPosition = player.transform.position + player.transform.forward * teleportDistance;
//         transform.position = teleportPosition;

//         Vector3 playerDirectionXZ = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
//         transform.forward = playerDirectionXZ - transform.position;

//         if (attackType == 1)
//         {
//             yield return new WaitForSeconds(1f);
//             CloseRangeAttack();
//         }
//         else if (attackType == 2)
//         {
//             StartCoroutine(ChargeAttack(3));
//         }
//     }

//  private IEnumerator ChargeAttack()
//     {
//         Vector3 playerDirectionXZ = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
//         transform.forward = playerDirectionXZ - transform.position;
//         Vector3 targetPos = player.transform.position + playerDirection * chargeOffset;

//         // Prepare for charge attack
//         yield return StartCoroutine(StopMoving(currentChargePrepareTime));

//         // Play charge effect and move towards the target position once
//         chargeEffect.Play();
//         while (Vector3.Distance(transform.position, targetPos) > 0.1f)
//         {
//             transform.position = Vector3.MoveTowards(transform.position, targetPos, currentChargeSpeed * Time.deltaTime);
//             yield return null;
//         }
//         chargeEffect.Stop();

//         // After the charge, go back to cooldown
//         ResetActionCooldown();
//     }

//     private void CloseRangeAttack()
//     {
//         animator.SetTrigger("MelleAttack");
//         teleportEffect.Play();
//         ResetActionCooldown();
//     }

//     private void AlignHealthBarWithCamera()
//     {
//         healthBarCanvas.transform.LookAt(mainCamera.transform);
//         healthBarCanvas.transform.Rotate(0, 180, 0);
//     }

//     public void TakeDamage(float damageAmount)
//     {
//         float actualDamage = CalculateDamageBasedOnPosition() * damageAmount;
//         bossHealth -= actualDamage;
//         bossHealth = Mathf.Clamp(bossHealth, 0, bossMaxHealth);
//         UpdateHealthBar();

//         if (bossHealth <= 0) Die();
//     }

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
//         if (healthBar != null) healthBar.fillAmount = bossHealth / bossMaxHealth;
//     }

//     private void Die()
//     {
//         Destroy(gameObject);
//     }

//     private IEnumerator StopMoving(float stopTime)
//     {
//         agent.isStopped = true;
//         animator.SetBool("Run", false);
//         yield return new WaitForSeconds(stopTime);
//         agent.isStopped = false;
//         animator.SetBool("Run", true);
//     }
// }
