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
        if (platformRewinding)
        {
            if (rewindJustStarted)
            {
                lastPlatformPos = gameObject.transform.position;
                rewindJustStarted = false;
                return; // skip first frame to avoid weird delta
            }
            if (playerOnPlat)
            {
                Vector3 platformDelta = gameObject.transform.position - lastPlatformPos;
                player.transform.position += platformDelta;
            }
            lastPlatformPos = gameObject.transform.position;
        }
    }
}
