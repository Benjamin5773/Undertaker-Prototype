using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopupTrigger : MonoBehaviour
{
    [Header("Popup UI")]
    [SerializeField] private Image popupImage; // Reference to the Image component
    [SerializeField] private float displayDuration = 3.0f; // Duration for the popup to be visible

    private Coroutine hideCoroutine; // Coroutine to handle hiding

    void Start()
    {
        if (popupImage != null)
        {
            popupImage.gameObject.SetActive(false); // Make sure the image is initially hidden
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the player enters the trigger
        {
            ShowPopup();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the player exits the trigger
        {
            HidePopup();
        }
    }

    private void ShowPopup()
    {
        if (popupImage != null)
        {
            popupImage.gameObject.SetActive(true); // Show the popup image
            
            // Cancel any ongoing hide coroutine to keep it visible as long as the player stays in the trigger
            if (hideCoroutine != null) 
            {
                StopCoroutine(hideCoroutine);
            }
        }
    }

    private void HidePopup()
    {
        if (popupImage != null)
        {
            // Start a coroutine to hide the image after the specified duration
            hideCoroutine = StartCoroutine(HideAfterDelay());
        }
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration); // Wait for the display duration
        popupImage.gameObject.SetActive(false); // Hide the popup image
        hideCoroutine = null; // Clear the coroutine reference
    }
}
