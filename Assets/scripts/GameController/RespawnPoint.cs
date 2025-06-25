using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    public Transform respawnPoint;
    private GameObject player;
    

    private void start()
    {
        Debug.Log("Start");
        player = GameObject.Find("Player");
    }

    public void SetRespawn(Transform respawnObj)
    {
        respawnPoint = respawnObj;
    }

    public void Respawn()
    {
        if(respawnPoint != null)
        {
            player.transform.position = respawnPoint.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Respawn();
        }
    }

}
