using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tp : MonoBehaviour
{
    public Transform positionB;
    private float rangex = 2f,rangez = 7f;
    public float maxRangeZ, minRangeZ;
    private float x, z;
    public LayerMask layer;
    private void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            x = other.transform.position.x;
            z = other.transform.position.z;
            // Generate random X and Z offsets
            float randomZ = Random.Range(z-rangez, z+rangez);
            if(randomZ > maxRangeZ)
            {
                randomZ = maxRangeZ;
            }else if(randomZ < minRangeZ)
            {
                randomZ = minRangeZ;
            }

            other.gameObject.transform.position = new Vector3(x,positionB.position.y,randomZ);
            Rigidbody rb = other.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
        }
    }
}
