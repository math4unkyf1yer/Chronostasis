using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDoor : MonoBehaviour
{
    public Animator doorAnimation;
    public Collider doorCollider;
    private bool trapActive;

    private void Start()
    {
        doorAnimation.SetBool("isDoorOpen", true);
        doorCollider.enabled = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player" && trapActive == false)
        {
            CloseDoor();
        }
    }

    public void OpenDoor()
    {
        Debug.Log("Open Door");
        doorAnimation.SetBool("isDoorOpen", true);
        doorCollider.enabled = false;
    }

    public void CloseDoor()
    {
        trapActive = true;
        Debug.Log("Close Door");
        doorAnimation.SetBool("isDoorOpen", false);
        doorCollider.enabled = true;
    }
}
