using UnityEngine;
using System.Collections;

public class LookAtController : MonoBehaviour
{
    //private readonly VectorPid angularVelocityController = new VectorPid(33.7766f, 0, 0.2553191f);
    //private readonly VectorPid headingController = new VectorPid(9.244681f, 0, 0.06382979f);

    public VectorPid angularVelocityController;
    public VectorPid headingController;

    //public Transform target;

    Rigidbody rb;

    public void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void FixedUpdate()
    {
        /*
        var angularVelocityError = rb.angularVelocity * -1;
        Debug.DrawRay(transform.position, rb.angularVelocity * 10, Color.black);

        var angularVelocityCorrection = angularVelocityController.Update(angularVelocityError, Time.deltaTime);
        Debug.DrawRay(transform.position, angularVelocityCorrection, Color.green);

        rb.AddTorque(angularVelocityCorrection);*/

        //Vector3 upTGT;

        var desiredHeading = Vector3.up;
        Debug.DrawRay(transform.position, desiredHeading, Color.magenta);

        var currentHeading = transform.up;
        Debug.DrawRay(transform.position, currentHeading * 15, Color.blue);

        var headingError = Vector3.Cross(currentHeading, desiredHeading);
        var headingCorrection = headingController.Update(headingError, Time.deltaTime);

        rb.AddTorque(headingCorrection);
    }
}
