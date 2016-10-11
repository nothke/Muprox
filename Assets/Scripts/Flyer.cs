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

    public RayWeapon weapon;

    float cooldown;

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
            UpdateLEDColor();

        if (weapon)
        {

            cooldown -= Time.deltaTime;

            if (TargetIsInView() && GunIsReady())
                TryShoot();
        }
    }

    void UpdateLEDColor()
    {
        Color curCol = Color.Lerp(Color.black, Color.red, Mathf.Sin(Time.time * 10) * 10);

        LED.material.SetColor("_EmissionColor", curCol);
    }

    bool GunIsReady()
    {
        return cooldown < 0;
    }

    bool TargetIsInView()
    {
        RaycastHit hit;
        if (Physics.Raycast(turret.position, turret.forward, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject == turretTarget.gameObject) return true;

            Debug.Log(hit.collider.name);
        }

        return false;
    }

    void TryShoot()
    {
        weapon.Shoot();
        cooldown = 1;
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
