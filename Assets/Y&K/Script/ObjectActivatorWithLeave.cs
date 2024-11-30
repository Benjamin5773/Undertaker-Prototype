using UnityEngine;

public class ObjectActivatorwithLeave : MonoBehaviour
{
    [Header("Player Setting")]
    [Tooltip("Tag of the player object to detect")]
    [SerializeField] string playerTag = "Player";

    [Header("Object to Activate")]
    [Tooltip("The GameObject to activate when the player enters the trigger")]
    [SerializeField] GameObject objectToActivate;

    private void OnTriggerEnter(Collider other)
    {
        // 当玩家进入检测区时，启用物体
        if (other.CompareTag(playerTag))
        {
            ActivateObject();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 当玩家离开检测区时，关闭物体
        if (other.CompareTag(playerTag))
        {
            DeactivateObject();
        }
    }

    private void ActivateObject()
    {
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
            Debug.Log("Object activated!");
        }
        else
        {
            Debug.LogWarning("No object assigned to activate!");
        }
    }

    private void DeactivateObject()
    {
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(false);
            Debug.Log("Object deactivated!");
        }
        else
        {
            Debug.LogWarning("No object assigned to deactivate!");
        }
    }
}
