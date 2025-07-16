using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseLaser : MonoBehaviour
{
    public GameObject close;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            close.SetActive(false);
        }
    }
}
