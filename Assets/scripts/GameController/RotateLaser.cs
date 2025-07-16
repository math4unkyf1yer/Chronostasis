using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLaser : MonoBehaviour
{
    public float rotationSpeed = 90f; // degrees per second
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    void Update()
    {
        // Calculate incremental rotation
        Quaternion deltaRotation = Quaternion.Euler(0f, rotationSpeed * Time.deltaTime, 0f);

        // Apply it to the rigidbody
        rb.MoveRotation(rb.rotation * deltaRotation);
    }
}
