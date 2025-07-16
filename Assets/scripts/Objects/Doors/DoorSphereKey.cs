using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSphereKey : MonoBehaviour
{
    public int key;
    public int enoughKey;
    private Collider doorCollider;
    private Animator doorAnimation;

    private void Start()
    {
        doorCollider = GetComponent<Collider>();
        doorAnimation = GetComponent<Animator>();
    }

    private void Update()
    {
        if(enoughKey == key)
        {
            doorCollider.enabled = false; 
            doorAnimation.SetBool("isDoorOpen", true);
        }
    }
}
