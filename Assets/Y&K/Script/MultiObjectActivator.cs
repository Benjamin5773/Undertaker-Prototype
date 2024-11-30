using UnityEngine;

public class MultiObjectActivator : MonoBehaviour
{
    [Header("Player Setting")]
    [Tooltip("Tag of the player object to detect")]
    [SerializeField] private string playerTag = "Player";

    [Header("Objects to Activate")]
    [Tooltip("The GameObjects to activate when the player enters the trigger")]
    [SerializeField] private GameObject[] objectsToActivate;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered && other.CompareTag(playerTag))
        {
            hasTriggered = true;
            ActivateObjects();
        }
    }

    private void ActivateObjects()
    {
        if (objectsToActivate != null && objectsToActivate.Length > 0)
        {
            foreach (GameObject obj in objectsToActivate)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                }
                else
                {
                    Debug.LogWarning("One of the objects to activate is null!");
                }
            }
        }
        else
        {
            Debug.LogWarning("No objects assigned to activate!");
        }
    }
}
