using System.Collections;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [Header("Door Objects")]
    [SerializeField] private Transform doorLeft; // Door_L object
    [SerializeField] private Transform doorRight; // Door_R object

    [Header("Door Settings")]
    [SerializeField] private float moveDistance = 3f; // Distance each door will move
    [SerializeField] private float moveSpeed = 2f; // Speed at which the doors move

    private Vector3 doorLeftClosedPosition;
    private Vector3 doorRightClosedPosition;
    private Vector3 doorLeftOpenPosition;
    private Vector3 doorRightOpenPosition;

    private void Start()
    {
        // Store the initial positions of the doors
        doorLeftClosedPosition = doorLeft.position;
        doorRightClosedPosition = doorRight.position;

        // Calculate the target open positions for both doors
        doorLeftOpenPosition = doorLeft.position + Vector3.left * moveDistance;
        doorRightOpenPosition = doorRight.position + Vector3.right * moveDistance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();
            StartCoroutine(OpenDoor());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();
            StartCoroutine(CloseDoor());
        }
    }

    private IEnumerator OpenDoor()
    {
        while (Vector3.Distance(doorLeft.position, doorLeftOpenPosition) > 0.01f &&
               Vector3.Distance(doorRight.position, doorRightOpenPosition) > 0.01f)
        {
            // Move doors towards the open positions
            doorLeft.position = Vector3.MoveTowards(doorLeft.position, doorLeftOpenPosition, moveSpeed * Time.deltaTime);
            doorRight.position = Vector3.MoveTowards(doorRight.position, doorRightOpenPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator CloseDoor()
    {
        while (Vector3.Distance(doorLeft.position, doorLeftClosedPosition) > 0.01f &&
               Vector3.Distance(doorRight.position, doorRightClosedPosition) > 0.01f)
        {
            // Move doors back towards the closed positions
            doorLeft.position = Vector3.MoveTowards(doorLeft.position, doorLeftClosedPosition, moveSpeed * Time.deltaTime);
            doorRight.position = Vector3.MoveTowards(doorRight.position, doorRightClosedPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
