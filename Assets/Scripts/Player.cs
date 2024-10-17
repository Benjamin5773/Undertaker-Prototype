
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Cinemachine.Utility;

public class Player : MonoBehaviour
{
    [SerializeField] private GameObject Cam;
    [SerializeField] private Transform enemy;
    private Rigidbody playerRig;

    [Header("GameInput Script")]
    [SerializeField] private GameInput gameInput;

    // Player movement
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 7.0f;
    [SerializeField] private float distance = 5.0f;
    [SerializeField] private float dashDuration = 0.2f; // Added dash duration

    //Energy bar 
    [Header("Energy")]
    [SerializeField] private Image energyBar, energyBarExtra;

    [SerializeField] private float maxEnergy, dashGain, energyFill, extraEnergyPerDodge;

    [Header("Skill Orbs")]
    [SerializeField] private GameObject[] skillOrbPrefabs; // Array of different skill orb prefabs
    private GameObject[] skillOrbInstances = new GameObject[4]; // Array to store the four instantiated skill orbs
    [SerializeField] private Vector3[] orbOffsets = new Vector3[4]; // Array for position offsets of each orb

    private SkillOrbs currentHoveredOrb; // Keep track of the currently hovered orb

    //Player damage setting
    [Header("Player Damage Setting")]
    public float Damage;


    private float energy;

    private float dashCount;
    private float rotateSpeed = 10f;
    private bool isWalking;
    private bool isDashing;
    public bool isEnergyFull; //To track whether the energy bar is full

    private Color targetColor; // The target color to transition to
    private float colorChangeInterval = 0.5f; // Time between color changes 
    private float colorChangeTimer; // Timer to track color changes


    private void Awake()
    {
        playerRig = GetComponent<Rigidbody>();
        energyBar.fillAmount = energy;
        energyBarExtra.fillAmount = 0;

        // Initialize the skill orbs
        // Define offsets for top, bottom, left, and right orbs relative to the player
        orbOffsets[0] = new Vector3(0.95f, 1.5f, 0);  // Top
        orbOffsets[1] = new Vector3(0.95f, 1, 0); // Bottom
        orbOffsets[2] = new Vector3(0.7f, 1.25f, 0); // Left
        orbOffsets[3] = new Vector3(1.2f, 1.25f, 0);  // Right

        // Instantiate the skill orbs using different prefabs and hide them initially
        for (int i = 0; i < 4; i++)
        {
            Vector3 spawnPosition = transform.position + orbOffsets[i]; // Calculate the spawn position
            skillOrbInstances[i] = Instantiate(skillOrbPrefabs[i], spawnPosition, Quaternion.identity);
            skillOrbInstances[i].transform.parent = transform; // Set each orb as a child of the player
            skillOrbInstances[i].SetActive(false); // Hide the orbs initially
        }
    }

    private void Update()
    {
        // Fill the energy bar over time
        energy += energyFill * Time.deltaTime;
        if (energy > maxEnergy)
        {
            energy = maxEnergy;
            energyBarExtra.fillAmount = dashCount * extraEnergyPerDodge / maxEnergy;
        }
        energyBar.fillAmount = energy / maxEnergy;

        // Assign random color to the extra energy bar
        UpdateEnergyBarExtraColor();

        if (!IsEnergyFull())
        {
            Cam.GetComponent<CameraSwitch>().ShowOverheadView();
            // Check for dash input, only dash if not already dashing
            if (gameInput.IsJumpButtonPressed() && !isDashing)
            {
                Debug.Log("dash");
                StartCoroutine(DashCoroutine()); // Use coroutine for smooth dash
                dashCount++;
                energy += dashGain;
            }

            // Get the movement vector from GameInput script
            Vector2 inputVector = gameInput.GetMovementVectorNormalized();

            // Move the player with the moveSpeed value
            Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
            transform.position += moveDir * moveSpeed * Time.deltaTime;

            // Smooth rotation towards movement direction
            if (moveDir != Vector3.zero)
            {
                transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
            }

            isWalking = moveDir != Vector3.zero;
        }

        else
        {
            LockOnEnemy();
            ShowSkillOrbs();
            HandleMouseHover();

            // Check for normal attack input
            if (gameInput.IsNormalAttackButtonPressed() && !isDashing)
            {
                Debug.Log("Normal Attack");
                ApplyDamage();
                ResetEnergy();
            }

            // Skills
            if(gameInput.IsSkillButtonPressed()&& currentHoveredOrb != null){
                currentHoveredOrb.ActivateSkill(); // Activate the skill of the hovered orb
                ResetEnergy();
            }

        }

    }

    // Coroutine for smooth dash
    private IEnumerator DashCoroutine()
    {
        isDashing = true;

        RaycastHit hit;
        Vector3 startPosition = transform.position;
        Vector3 destination = transform.position + transform.forward * distance;

        // Check if there's an obstacle in the path with the "Wall" tag
        if (Physics.Linecast(transform.position, destination, out hit))
        {
            if (hit.collider.CompareTag("Wall")) // Check if the hit object has the "Wall" tag
            {
                destination = transform.position + transform.forward * (hit.distance - 1f);
            }
        }

        // Ensure the destination is grounded only if the object hit has the "Wall" tag
        if (Physics.Raycast(destination, -Vector3.up, out hit))
        {
            if (hit.collider.CompareTag("Wall")) // Check if the hit object has the "Wall" tag
            {
                destination = hit.point;
            }
        }
        // Smoothly move the player over the dash duration
        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            transform.position = Vector3.Lerp(startPosition, destination, elapsed / dashDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the player is at the final destination
        transform.position = destination;

        isDashing = false;
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    public bool IsEnergyFull()
    {
        // Check if energy is 100 and disable movement
        if (energy >= maxEnergy)
        {
            isEnergyFull = true; // Disable movement
        }
        else
        {
            isEnergyFull = false; // Enable movement if energy is below 100
        }
        return isEnergyFull;
    }

    private void UpdateEnergyBarExtraColor()
    {
        colorChangeTimer += Time.deltaTime;

        // If the timer exceeds the color change interval, generate a new target color
        if (colorChangeTimer >= colorChangeInterval)
        {
            targetColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            colorChangeTimer = 0f; // Reset the timer
        }

        energyBarExtra.color = Color.Lerp(energyBarExtra.color, targetColor, Time.deltaTime * 2.0f);
    }

    private void ResetEnergy()
    {
        //reset the energy bar after player's action
        energy = 0;
        dashCount = 0;
        energyBarExtra.fillAmount = 0;
        HideSkillOrbs();
    }
    private void LockOnEnemy()
    {
        // Rotate the player to face the enemy
        Vector3 directionToEnemy = (enemy.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToEnemy.x, 0, directionToEnemy.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotateSpeed);
        Cam.GetComponent<CameraSwitch>().ShowOvershoulderView();
    }

    private void ShowSkillOrbs()
    {
        // Show all 4 skill orbs
        for (int i = 0; i < 4; i++)
        {
            if (skillOrbInstances[i] != null)
            {
                skillOrbInstances[i].SetActive(true); // Show each orb
            }
        }
    }

    private void HideSkillOrbs()
    {
        // Hide all 4 skill orbs
        for (int i = 0; i < 4; i++)
        {
            if (skillOrbInstances[i] != null)
            {
                skillOrbInstances[i].SetActive(false); // Hide each orb
            }
        }
        Debug.Log("Skill orbs hidden.");
    }

    // https://www.youtube.com/watch?v=qYnAkMGbgwo&t=218s
      private void HandleMouseHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Cast a ray from the camera to the mouse position
        if (Physics.Raycast(ray, out hit))
        {
            // Check if the ray hit a skill orb
            SkillOrbs skillOrb = hit.collider.GetComponent<SkillOrbs>();
            if (skillOrb != null)
            {
                // If the orb is hovered for the first time, call the hover function
                if (currentHoveredOrb != skillOrb)
                {
                    // Unhover the previous orb
                    if (currentHoveredOrb != null)
                    {
                        currentHoveredOrb.OnHover(false);
                    }

                    // Hover the new orb
                    currentHoveredOrb = skillOrb;
                    currentHoveredOrb.OnHover(true);
                }
            }
            else
            {
                // If the ray didn't hit any orb, unhover the currently hovered orb
                if (currentHoveredOrb != null)
                {
                    currentHoveredOrb.OnHover(false);
                    currentHoveredOrb = null;
                }
            }
        }
        else
        {
            // If nothing is hit by the ray, unhover the currently hovered orb
            if (currentHoveredOrb != null)
            {
                currentHoveredOrb.OnHover(false);
                currentHoveredOrb = null;
            }
        }
    }

    public void ApplyDamage() {
        enemy.GetComponent<EnemyTest>().TakeDamage(Damage);
    }

}


// using UnityEditor.Experimental.GraphView;
// using UnityEngine;
// using System.Collections; // Required for IEnumerator if coroutines are used

// public class Player : MonoBehaviour
// {
//     private Rigidbody playerRig;
//     [SerializeField] private GameInput gameInput;

//     [Header("Movement")]
//     [SerializeField] private float moveSpeed = 7.0f;
//     [SerializeField] private float distance = 2.0f;
//     [SerializeField] private float dashDuration = 0.2f; // Keeping dash duration for timing purposes

//     private float rotateSpeed = 10f;
//     private bool isWalking;
//     private bool isDashing; // To ensure dash is executed only once

//     private void Awake()
//     {
//         playerRig = GetComponent<Rigidbody>();
//     }

//     private void Update()
//     {
//         // Check for dash input, only dash if not already dashing
//         if (gameInput.IsJumpButtonPressed() && !isDashing)
//         {
//             Debug.Log("dash");
//             BlinkDash(); // Perform instant blink (teleport)
//         }

//         // Get the movement vector from GameInput script
//         Vector2 inputVector = gameInput.GetMovementVectorNormalized();

//         // Move the player with the moveSpeed value
//         Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);
//         transform.position += moveDir * moveSpeed * Time.deltaTime;

//         // Smooth rotation towards movement direction
//         if (moveDir != Vector3.zero)
//         {
//             transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
//         }

//         isWalking = moveDir != Vector3.zero;
//     }

//     // Instant Blink Dash (Teleport)
//     private void BlinkDash()
//     {
//         isDashing = true;

//         RaycastHit hit;
//         Vector3 destination = transform.position + transform.forward * distance;

//         // Check if there's an obstacle in the path
//         if (Physics.Linecast(transform.position, destination, out hit))
//         {
//             // Adjust destination to stop just before the obstacle
//             destination = transform.position + transform.forward * (hit.distance - 1f);
//         }

//         // Ensure the destination is grounded
//         if (Physics.Raycast(destination, -Vector3.up, out hit))
//         {
//             destination = hit.point;
//         }

//         // Instantly move the player to the destination (blink/teleport)
//         transform.position = destination;

//         // Invoke reset function after a short delay to allow for cooldown
//         Invoke(nameof(ResetDash), dashDuration);
//     }

//     private void ResetDash()
//     {
//         isDashing = false;
//     }

//     public bool IsWalking()
//     {
//         return isWalking;
//     }
// }
