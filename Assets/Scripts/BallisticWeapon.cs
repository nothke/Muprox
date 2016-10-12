using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class BallisticWeapon : Weapon
{

    public GameObject bulletPrefab;

    public bool repeat;
    public float cooldownSeconds = 0.1f;
    public float recoil = 1;

    [Command]
    public override void CmdFire()
    {
        base.CmdFire();

        if (ammo <= 0)
        {
            if (fireNoAmmo)
                AudioSource.PlayClipAtPoint(fireNoAmmo, transform.position);

            return;
        }

        ammo--;


        if (fireShot)
            AudioSource.PlayClipAtPoint(fireShot, transform.position);

        GameObject bullet = Instantiate(bulletPrefab, muzzle.position, muzzle.rotation) as GameObject;
        NetworkServer.Spawn(bullet);

        bullet.GetComponent<Rigidbody>().AddForce(muzzle.forward * 200);
    }


}
