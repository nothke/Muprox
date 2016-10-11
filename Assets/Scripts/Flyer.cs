using UnityEngine;
using System.Collections;

public class Flyer : MonoBehaviour
{

    public VectorPid pid;
    Rigidbody rb;

    public Transform target;
    public Vector3 targetPos;

    public float maxForce = 1;

    public Renderer LED;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public Transform turret;
    public Transform turretTarget;


    void Update()
    {
        if (turret)
        {
            if (turretTarget)
            {
                turret.LookAt(turretTarget, transform.up);
                float clampedXAngle = Mathf.Clamp(turret.localEulerAngles.x, 1, 80);
                Vector3 localAngle = new Vector3(clampedXAngle, turret.localEulerAngles.y, turret.localEulerAngles.z);

                turret.localEulerAngles = localAngle;
            }
        }

        if (LED)
        {

            Color curCol = Color.Lerp(Color.black, Color.red, Mathf.Sin(Time.time * 10) * 10);

            LED.material.SetColor("_EmissionColor", curCol);

        }
    }

    void FixedUpdate()
    {
        if (target)
            targetPos = target.position;
        else
            targetPos = transform.position;

        Vector3 force = pid.Update(targetPos, transform.position, Time.deltaTime);

        force = Vector3.ClampMagnitude(force, maxForce);

        //if (rb.velocity.magnitude < maxSpeed)
        //rb.AddForce(transform.up * -Physics.gravity.y + force);
        rb.AddRelativeForce(Vector3.up * -Physics.gravity.y + force);


    }
}
