using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnBeacon : MonoBehaviour
{
    private RespawnPoint respawnPointScript;
    public Transform positionBack;
    public GameObject beaconEffects;
    

    private void Start()
    {
        //Get the respawn point 
        respawnPointScript = GameObject.Find("GameManager").GetComponent<RespawnPoint>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            //play effect 
            //set respawn
            if(respawnPointScript != null)
            {
                beaconEffects.gameObject.SetActive(true);
                respawnPointScript.SetRespawn(positionBack,beaconEffects);
            }
        }
    }
}
