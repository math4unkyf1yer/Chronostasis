using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapDoorOpen : MonoBehaviour
{
    public TrapDoor trapDoorScript;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            trapDoorScript.OpenDoor();
        }
    }
}
