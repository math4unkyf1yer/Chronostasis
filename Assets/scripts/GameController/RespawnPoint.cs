using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    public Transform respawnPoint;
    private GameObject player;
    private GameObject effects;
    

    private void Start()
    {

        player = GameObject.Find("Player");
    }

    public void SetRespawn(Transform respawnObj,GameObject effect)
    {
        if(effects != null && effects != effect)
        {
            effects.gameObject.SetActive(false);
        }
        respawnPoint = respawnObj;
        effects = effect;
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
