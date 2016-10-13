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
    public Light LEDLight;

    public float lookAheadDistance = 4;
    public float immediateDistance = 1;

    public LayerMask raycastMask;

    public AudioSource propAudio;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        StartCoroutine(Patrol());
    }

    IEnumerator Patrol()
    {
        while (true)
        {
            seekPos = Random.insideUnitSphere * 100;

            seekPos.y = Random.Range(1, 10);



            turretTarget = GetRandomBox();

            yield return new WaitForSeconds(10);

        }
    }

    Transform GetRandomBox()
    {
        Transform[] all = FindObjectsOfType(typeof(Transform)) as Transform[];

        Transform t = null;

        int tst = 0;

        do
        {
            t = all[Random.Range(0, all.Length)];

            tst++;

            if (tst > 100) break;

        } while (!t.name.StartsWith("Canonball") && !t.name.StartsWith("Player"));


        return t;
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

        if (propAudio)
        {
            propAudio.pitch = totalForce.magnitude / 9.8f;
        }
    }

    void UpdateLEDColor()
    {
        Color curCol = Color.Lerp(Color.black, Color.red, Mathf.Sin(Time.time * 10) * 10);

        LED.material.SetColor("_EmissionColor", curCol);

        if (LEDLight)
            LEDLight.color = curCol;
    }

    bool GunIsReady()
    {
        return cooldown < 0;
    }

    bool TargetIsInView()
    {
        if (!turretTarget) return false;

        RaycastHit hit;
        if (Physics.Raycast(turret.position, turret.forward, out hit, Mathf.Infinity, raycastMask))
        {
            if (hit.collider.gameObject == turretTarget.gameObject) return true;
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
        Vector3 targetDir = Vector3.ClampMagnitude(seekDir, immediateDistance);
        float targetDist = Vector3.ClampMagnitude(seekDir, lookAheadDistance).magnitude;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, seekDir, out hit, targetDist, raycastMask))
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

    Vector3 totalForce;

    void FixedUpdate()
    {
        if (target)
            seekPos = target.position;

        CalculateImmediateTarget();

        Vector3 force = pid.Update(targetPos, transform.position, Time.deltaTime);

        force = Vector3.ClampMagnitude(force, maxForce);

        //if (rb.velocity.magnitude < maxSpeed)
        //rb.AddForce(transform.up * -Physics.gravity.y + force);
        //rb.AddRelativeForce(Vector3.up * -Physics.gravity.y + force);
        if (rb.velocity.magnitude > 5)
            force = Vector3.zero;

        totalForce = transform.up * -Physics.gravity.y + force;

        rb.AddForce(totalForce);

    }
}
