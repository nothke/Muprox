using UnityEngine;
using System.Collections;

public class Flyer : MonoBehaviour
{

    public VectorPid pid;
    Rigidbody rb;

    public Transform target;
    public Vector3 targetPos;

    public Vector3 seekPos;

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

    void CalculateImmediateTarget()
    {

        Vector3 seekDir = seekPos - transform.position;
        Vector3 targetDir = Vector3.ClampMagnitude(seekDir, 8);
        float targetDist = targetDir.magnitude;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, seekDir, out hit, targetDist))
        {
            Vector3 reflectedDir = Vector3.Reflect(targetDir, hit.normal);

            Vector3 reflectedPoint = hit.point + reflectedDir;

            targetPos = reflectedPoint;
            Debug.DrawLine(transform.position, targetPos, Color.red);
            //transform.position - targetDir;
        }
        else
        {
            targetPos = transform.position + targetDir;
            Debug.DrawLine(transform.position, targetPos, Color.white);
        }

    }

    void FixedUpdate()
    {
        if (target)
            seekPos = target.position;
        else
            seekPos = transform.position;

        CalculateImmediateTarget();

        Vector3 force = pid.Update(targetPos, transform.position, Time.deltaTime);

        force = Vector3.ClampMagnitude(force, maxForce);

        //if (rb.velocity.magnitude < maxSpeed)
        //rb.AddForce(transform.up * -Physics.gravity.y + force);
        rb.AddRelativeForce(Vector3.up * -Physics.gravity.y + force);


    }
}
