using UnityEngine;
using System.Collections;
//using UnityEngine.Networking;

public class BallisticWeapon : Weapon
{
    public bool ray;

    public GameObject bulletPrefab;





    //[Command]
    public override void Fire()
    {
        base.Fire();

        if (ray)
            return;

        GameObject bullet = Instantiate(bulletPrefab, muzzle.position, muzzle.rotation) as GameObject;
        //NetworkServer.Spawn(bullet);

        bullet.GetComponent<Rigidbody>().AddForce(muzzle.forward * 100, ForceMode.VelocityChange);
    }
}
