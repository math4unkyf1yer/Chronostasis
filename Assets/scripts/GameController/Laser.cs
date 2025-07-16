using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    private RespawnPoint respawnScript;
    public Transform[] patrolPoints;
    public float speed = 3;
    private int currentTargetIndex = 1;
    public GameObject laserHolder;
    public float maxDistance = 43f;
    public bool OnAndOff;
    public GameObject laserEffect;
    private float timer = 0f;
    private bool laserActive = true;
    private void Start()
    {
        respawnScript = GameObject.Find("GameManager").GetComponent<RespawnPoint>();
    }
    private void Update()
    {
        if (OnAndOff)
        {
            HandleOnAndOff();
        }

        if (laserActive) // only do laser when active
        {
            if (patrolPoints != null && patrolPoints.Length >= 2)
            {
                Patrol();
            }
            UpdateLaser();
        }
    }

    private void HandleOnAndOff()
    {
        timer += Time.deltaTime;

        if (laserActive && timer >= 5f)
        {
            // turn off laser
            laserActive = false;
            timer = 0f;
            if (laserEffect != null) laserEffect.SetActive(false);
        }
        else if (!laserActive && timer >= 2f)
        {
            // turn on laser
            laserActive = true;
            timer = 0f;
            if (laserEffect != null) laserEffect.SetActive(true);
        }
    }

    private void UpdateLaser()
    {
        Vector3 direction = transform.forward;
        float laserLength = maxDistance;

        // Define the half extents of the box (adjust to control width & height)
        Vector3 halfExtents = new Vector3(0.7f, 1f, 0f); // width & height of laser beam

        RaycastHit hit;

        // Start the box slightly in front of the laser to avoid hitting itself
        Vector3 origin = transform.position;

        if (Physics.BoxCast(origin, halfExtents, direction, out hit, transform.rotation, laserLength))
        {
          //  Debug.Log($"BoxCast hit: {hit.collider.name} at distance: {hit.distance}");
            if (hit.collider.CompareTag("Player"))
            {
                respawnScript.Respawn();
            }
        }

        // Optional: for visualization in editor
        Debug.DrawRay(origin, direction * laserLength, Color.red);
        Debug.DrawLine(origin - transform.up * halfExtents.y, origin + transform.up * halfExtents.y, Color.yellow);
    }
    private void Patrol()
    {
        // Move towards the current target point
        Transform targetPoint = patrolPoints[currentTargetIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        // Check if reached the target point
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.1f)
        {
            // Switch to the other point
            currentTargetIndex = (currentTargetIndex == 0) ? 1 : 0;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * maxDistance);
    }
}
