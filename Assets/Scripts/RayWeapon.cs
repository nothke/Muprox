using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class RayWeapon : NetworkBehaviour
{
    public float recoilForce = 0;
    public float hitForce = 200;

    public ParticleSystem nozzleParticles;
    public int emitParticles = 30;

    [SyncVar]
    public int ammo = 64;

    public void Shoot()
    {
        if (nozzleParticles)
            nozzleParticles.Emit(emitParticles);

        if (Network.isServer)
            RpcShoot();
    }

    [ClientRpc]
    public void RpcShoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
        {
            if (hit.collider.GetComponent<Rigidbody>())
                hit.collider.GetComponent<Rigidbody>().AddForceAtPosition(transform.forward * hitForce, hit.point);

            Debug.Log("Shot: " + hit.collider.name);
        }

        ammo--;
    }
}
