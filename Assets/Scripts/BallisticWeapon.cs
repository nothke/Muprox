using UnityEngine;
using System.Collections;
//using UnityEngine.Networking;

public class BallisticWeapon : Weapon
{

    public GameObject bulletPrefab;

    public bool repeat;
    public float cooldownSeconds = 1;
    public float cooldown = 0;
    public float recoil = 1;

    void Update()
    {
        cooldown -= Time.deltaTime / cooldownSeconds;

        cooldown = Mathf.Clamp01(cooldown);

    }

    //[Command]
    public override void Fire()
    {

        if (cooldown > 0)
            return;

        cooldown = 1;

        base.Fire();





        GameObject bullet = Instantiate(bulletPrefab, muzzle.position, muzzle.rotation) as GameObject;
        //NetworkServer.Spawn(bullet);

        bullet.GetComponent<Rigidbody>().AddForce(muzzle.forward * 100, ForceMode.VelocityChange);
    }
}
