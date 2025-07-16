using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorKey : MonoBehaviour
{
    public Animator doorAnimation;
    public Collider doorCollider;
    public Selecting selectingScript;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Key")
        {
            doorCollider.enabled = false;
            doorAnimation.SetBool("isDoorOpen", true);
            if (selectingScript.isGrabbing)
                selectingScript.isGrabbing = false;
            Destroy(collision.gameObject);
        }
    }
}
