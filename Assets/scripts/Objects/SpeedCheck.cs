using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedCheck : MonoBehaviour
{
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(rb != null)
        {
            if(rb.velocity.magnitude > 27)
            {
                rb.velocity = rb.velocity.normalized * 27f;
            }
        }
    }
}
