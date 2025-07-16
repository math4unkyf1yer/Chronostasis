using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakePlatMove : MonoBehaviour
{
    public bool isMovingRight = true;
    public float speed = 2f;
    void Update()
    {
        Vector3 direction = isMovingRight ? Vector3.right : Vector3.left;
        transform.position += direction * speed * Time.deltaTime;
    }
}
