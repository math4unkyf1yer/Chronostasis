using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightNeeded : MonoBehaviour
{
    public int weightNeeded;
    private int currentWeight;
    private bool doorOpen;
    
    public GameObject doors;
    private Animator doorAnimator;
    private Collider doorCollider;

    public float moveDuration = 1f; // Duration of door movement in seconds
    private Coroutine moveCoroutine;

    private Vector3 originalDoorPosition;
    private Vector3 targetOpenPosition;

    private bool isMoving = false;
    private void Start()
    {
        doorAnimator = doors.GetComponent<Animator>();
        doorCollider = doors.GetComponent<Collider>();
        originalDoorPosition = doors.transform.position;
        targetOpenPosition = originalDoorPosition + new Vector3(0, 6f, 0);
    }
    private void OnCollisionEnter(Collision collision)
    {
        Weight weightScript = collision.gameObject.GetComponent<Weight>();
        if(weightScript != null)
        {
            currentWeight += weightScript.weight;
            if(currentWeight >= weightNeeded)
            {
                MaxWeightAchieve();
            }
        }
    }
    private void OnCollisionExit(Collision collision)
    {

        Weight weightScript = collision.gameObject.GetComponent<Weight>();
        if (weightScript != null)
        {
            currentWeight -= weightScript.weight;
        }
    }

    private void Update()
    {

        // Only try to close if weight is below threshold
        if ((doorOpen && currentWeight < weightNeeded) ||
            (!doorOpen && currentWeight < weightNeeded && doors.transform.position.y > originalDoorPosition.y + 0.01f))
        {
            CloseDoor();
        }
    }


    void CloseDoor()
    {
        Debug.Log("Closing door. Door open? " + doorOpen + ", Current weight: " + currentWeight);

        doorOpen = false;
        if (doors.tag == "notGrab")
        {
            Debug.Log("Letting gravity pull door down.");
            Rigidbody rb = doors.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
                rb.isKinematic = false; // Let gravity take over
            }
        }
        else if (doors.tag == "Laser")
        {
            doors.gameObject.SetActive(true);
        }
        else
        {
            doorAnimator.SetBool("isDoorOpen", false);
            doorCollider.enabled = true;
        }
    }
    void MaxWeightAchieve()
    {
        doorOpen = true;
        if (doors.tag == "notGrab")
        {
            Rigidbody rb = doors.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                rb.isKinematic = true; // Temporarily disable physics so we can Lerp upward
            }
            if (doors.transform.position.y < targetOpenPosition.y - 0.01f)
            {
                StartMoveDoor(targetOpenPosition);
            }
        }
        else if (doors.tag == "Laser")
        {
            doors.gameObject.SetActive(false);
        }
        else
        {
            doorAnimator.SetBool("isDoorOpen", true);
            doorCollider.enabled = false;
        }
    }
    void StartMoveDoor(Vector3 targetPosition)
    {
        if (isMoving) return;

        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(MoveDoorSmoothly(targetPosition));
    }

    IEnumerator MoveDoorSmoothly(Vector3 targetPos)
    {
        isMoving = true;
        Vector3 startPos = doors.transform.position;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            doors.transform.position = Vector3.Lerp(startPos, targetPos, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        doors.transform.position = targetPos; // Ensure final position is exact
        isMoving = false;
    }
}
