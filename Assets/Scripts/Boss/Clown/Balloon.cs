using System.Collections;
using UnityEngine;
using UnityEngine.Formats.Alembic.Importer;

public class Balloon : MonoBehaviour
{
    [SerializeField]
    private float floatSpeed = 3f; // Speed of floating toward the player

    [SerializeField]
    private float explosionThreshold = 2f; // Distance at which the balloon explodes

    [SerializeField]
    private float animationSpeed = 0.75f; // Speed factor for the Alembic animation

    [SerializeField]
    private AlembicStreamPlayer alembicStreamPlayer; // Reference to the Alembic Stream Player

    [SerializeField]
    private GameObject balloonHitbox;

    private Transform playerTransform; // Player reference
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Find the Alembic Stream Player
        if (alembicStreamPlayer == null)
        {
            alembicStreamPlayer = GetComponent<AlembicStreamPlayer>();
        }

        if (balloonHitbox != null)
        {
            balloonHitbox.SetActive(false);
        }
    }

    private void OnEnable()
    {
        // Stop any existing coroutines
        StopAllCoroutines();

        // Find the player if not already assigned
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }

        // Start floating toward the player
        StartCoroutine(FloatTowardPlayer());
    }

    private IEnumerator FloatTowardPlayer()
    {
        while (gameObject.activeSelf)
        {
            if (playerTransform != null)
            {
                // Calculate the direction to the player, ignoring the Y-axis
                Vector3 targetPosition = new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z);
                Vector3 direction = (targetPosition - transform.position).normalized;

                // Move toward the player on a flat plane
                rb.velocity = direction * floatSpeed;

                // Check if within the explosion threshold
                if (Vector3.Distance(transform.position, targetPosition) <= explosionThreshold)
                {
                    Explode();
                    yield break; // Stop the coroutine after exploding
                }
            }

            yield return null; // Wait for the next frame
        }
    }

    private void Explode()
    {
        Debug.Log("Balloon exploded!");

        // Stop movement
        rb.velocity = Vector3.zero;

        // balloonHitbox.SetActive(true);

        // Play the Alembic animation
        if (alembicStreamPlayer != null)
        {
            StartCoroutine(PlayExplosionAnimation());
        }
    }

 private IEnumerator PlayExplosionAnimation()
{
    // Reset and play Alembic animation
    alembicStreamPlayer.CurrentTime = alembicStreamPlayer.StartTime; // Reset to the beginning
        bool hitboxEnabled = false; // Track if the hitbox has been enabled

    while (alembicStreamPlayer.CurrentTime < alembicStreamPlayer.EndTime-0.2f)
    {
        // Progress animation with adjusted speed
        alembicStreamPlayer.CurrentTime += Time.deltaTime * animationSpeed;
        alembicStreamPlayer.UpdateImmediately(alembicStreamPlayer.CurrentTime);

        if (!hitboxEnabled && alembicStreamPlayer.CurrentTime >= 0.55f)
        {
            if (balloonHitbox != null)
            {
                balloonHitbox.SetActive(true);
                Debug.Log("Hitbox activated at 0.55 in the Alembic timeline.");
            }
            hitboxEnabled = true; // Ensure it only activates once
        }

        // Wait for the next frame
        yield return null;
    }
    balloonHitbox.SetActive(false);
    // Destroy the balloon after the animation completes
    Debug.Log("Destroying balloon object after animation.");
    Destroy(gameObject);
}

    private void OnDisable()
    {
        // Reset Rigidbody velocities
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Reset Alembic animation
        if (alembicStreamPlayer != null)
        {
            alembicStreamPlayer.CurrentTime = alembicStreamPlayer.StartTime;
        }
    }
}
