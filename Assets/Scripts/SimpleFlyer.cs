using UnityEngine;
using System.Collections;

public class SimpleFlyer : MonoBehaviour
{
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        rb.AddForce(transform.up * -Physics.gravity.y);

    }
}
