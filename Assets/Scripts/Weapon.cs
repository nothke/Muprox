using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Weapon : NetworkBehaviour
{
    public Transform muzzle;

    public Light fireFlash;

    public int ammo = 10;

    public AudioClip fireShot;
    public AudioClip fireNoAmmo;

    Vector3 holdPosition = new Vector3(0.2f, -0.2f, 0.2f);
    Vector3 aimPosition;



    [Command]
    public virtual void CmdFire()
    {

    }
}
