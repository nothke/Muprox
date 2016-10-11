using UnityEngine;
using System.Collections;

public class RayWeapon : MonoBehaviour
{
    public float recoilForce = 0;
    public float hitForce = 200;

    public ParticleSystem nozzleParticles;
    public int emitParticles = 30;

    public int ammo = 64;

    public void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
        {
            if (hit.collider.GetComponent<Rigidbody>())
                hit.collider.GetComponent<Rigidbody>().AddForceAtPosition(transform.forward * hitForce, hit.point);

            Debug.Log("Shot: " + hit.collider.name);
        }

        if (nozzleParticles)
            nozzleParticles.Emit(emitParticles);

        ammo--;
    }
}
