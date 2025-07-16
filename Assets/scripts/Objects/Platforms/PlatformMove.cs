using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMove : MonoBehaviour
{
    Vector3 lastPlatformPos;
    public bool playerOnPlat;
    public bool platformRewinding;
    private GameObject player;
    private bool rewindJustStarted = false;

    public float smoothingSpeed = 10f; // adjust for smoothness


    public void PlayerOnObject()
    {
        playerOnPlat = true;
       
    }

    public void PlayerOffObject()
    {
        
        playerOnPlat = false;
    }

    private void Start()
    {
        player = GameObject.Find("Player");
        
    }
    public void StartRewind()
    {
        platformRewinding = true;
        rewindJustStarted = true;
    }
    private void FixedUpdate()
    {
        if (playerOnPlat)
        {
            Vector3 platformDelta = transform.position - lastPlatformPos;
            Vector3 targetPos = player.transform.position + platformDelta;

            // compute platform speed this frame
            float platformSpeed = platformDelta.magnitude / Time.fixedDeltaTime;

            // move player toward targetPos at platform speed
            player.transform.position = Vector3.MoveTowards(
                player.transform.position,
                targetPos,
                platformSpeed * Time.fixedDeltaTime
            );
        }
        lastPlatformPos = gameObject.transform.position;
      /*  if (platformRewinding) keep to see if I want to change it back to only when I rewind
        {
            if (rewindJustStarted)
            {
                lastPlatformPos = gameObject.transform.position;
                rewindJustStarted = false;
                return; // skip first frame to avoid weird delta
            }
        }*/
    }
}
