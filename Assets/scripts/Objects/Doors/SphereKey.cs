using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereKey : MonoBehaviour
{
    public Selecting selectingScript;
    private Transform spherePosition;
    private Outline outlineScript;
    private OutlineHighlighter outline2Script;
    private PositionTracker positionTrackerScript;
    private Rigidbody rb;
    public DoorSphereKey doorScript;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        outline2Script = GetComponent<OutlineHighlighter>();
        outlineScript = GetComponent<Outline>();
        positionTrackerScript = GetComponent<PositionTracker>();
        spherePosition = GameObject.Find("Assign").GetComponent<Transform>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "KeyHold")
        {
            rb.isKinematic = true;
            doorScript.key++;
            if (selectingScript.isGrabbing)
                selectingScript.isGrabbing = false;
            DisableScrip();
            spherePosition.position = collision.transform.position;
            float sphereZPosition = spherePosition.transform.position.y + 1;
            Vector3 spherePlacement = new Vector3(spherePosition.transform.position.x, sphereZPosition, spherePosition.transform.position.z);
            transform.position = spherePlacement;

            gameObject.tag = "notGrab";
        }
    }
    void DisableScrip()
    {
        outlineScript.enabled = false;
        outline2Script.enabled = false;
        positionTrackerScript.enabled = false;
    }
}
