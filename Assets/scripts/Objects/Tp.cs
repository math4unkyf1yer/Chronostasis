using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tp : MonoBehaviour
{
    public Transform positionB;
    private float rangex = 2f,rangez = 7f;
    public float maxRangeZ, minRangeZ;
    private float x, z,y;
    public LayerMask layer;
    public bool verticalTp;
    public bool notRandom;
    private void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            x = other.transform.position.x;
            z = other.transform.position.z;
            y = other.transform.position.y;
            if (!verticalTp && !notRandom)
            {
                Rigidbody rb = other.GetComponent<Rigidbody>();
                rb.velocity = Vector3.zero;

                // Generate random X and Z offsets
                float randomZ = Random.Range(z - rangez, z + rangez);
                if (randomZ > maxRangeZ)
                {
                    randomZ = maxRangeZ;
                }
                else if (randomZ < minRangeZ)
                {
                    randomZ = minRangeZ;
                }

                other.gameObject.transform.position = new Vector3(x, positionB.position.y, randomZ);
            }
            else if(verticalTp)
            {
                other.gameObject.transform.position = new Vector3(positionB.position.x, y, z);
            }
            else
            {
                other.gameObject.transform.position = new Vector3(x, positionB.position.y, z);
            }
        }
    }
}
