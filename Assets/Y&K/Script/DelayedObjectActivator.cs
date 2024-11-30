using UnityEngine;

public class DelayedObjectActivator : MonoBehaviour
{
    [Header("Player Setting")]
    [Tooltip("Tag of the player object to detect")]
    [SerializeField] private string playerTag = "Player";

    [Header("Object to Activate")]
    [Tooltip("The GameObject to activate when the player enters the trigger")]
    [SerializeField] private GameObject objectToActivate;

    [Header("Activation Delay")]
    [Tooltip("The delay time in seconds before activating the object")]
    [SerializeField] private float activationDelay = 1.0f;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag(playerTag))
        {
            hasTriggered = true;
            Invoke(nameof(ActivateObject), activationDelay); // Call ActivateObject after the delay
        }
    }

    private void ActivateObject()
    {
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No object assigned to activate!");
        }
    }
}
