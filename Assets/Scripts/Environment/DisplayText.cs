
using System.Collections;
using UnityEngine;
using TMPro;

public class DisplayText : MonoBehaviour
{
    [SerializeField] private GameObject player; 
    [SerializeField] private Collider[] colliders = new Collider[5]; 
    [SerializeField] private TMP_Text displayText; 
    [SerializeField] private string[] messages = { "1", "2", "3", "4", "5" }; 

    private Coroutine displayCoroutine;
     private bool[] hasTriggered; 

    private void Start()
    {
         hasTriggered = new bool[colliders.Length];
        if (displayText != null)
        {
            displayText.text = "";
            displayText.gameObject.SetActive(false);
        }
    }

 private void OnTriggerEnter(Collider other)
{

      Debug.Log("OnTriggerEnter triggered by: " + other.gameObject.name);

     // if (other.gameObject == player)
     // {
        Debug.Log("Player detected!");

       
        for (int i = 0; i < colliders.Length; i++)
        {
            if (other == colliders[i] && !hasTriggered[i])
            {
                Debug.Log("Player triggered Collider " + (i + 1));
                ShowMessage(i);
                hasTriggered[i] = true; 
                break;
            }
       // }
    }
}


    private void ShowMessage(int index)
    {
        if (displayCoroutine != null)
        {
            StopCoroutine(displayCoroutine);
        }

        displayCoroutine = StartCoroutine(DisplayTextForSeconds(messages[index], 2f));
    }

    private IEnumerator DisplayTextForSeconds(string message, float duration)
    {
        displayText.text = message;
        displayText.gameObject.SetActive(true);

        yield return new WaitForSeconds(duration);

        displayText.text = "";
        displayText.gameObject.SetActive(false);
    }
}
