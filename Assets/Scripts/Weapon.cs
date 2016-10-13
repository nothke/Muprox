using UnityEngine;
using System.Collections;
//using UnityEngine.Networking;

public class Weapon : MonoBehaviour
{
    public Transform muzzle;

    public Light fireFlash;
    public ParticleSystem fireParticles;

    public int ammo = 10;

    public AudioClip fireShot;
    public AudioClip fireNoAmmo;

    public Transform aimPivot;
    public Transform handPivot;

    [HideInInspector]
    public float cooldown = 0;
    public float cooldownSeconds = 0.1f;

    public int damage = 10;

    public bool repeat;
    public float recoil = 1;

    public float spread = 0;
    public int buck = 1;

    public virtual void Update()
    {
        cooldown -= Time.deltaTime / cooldownSeconds;

        cooldown = Mathf.Clamp01(cooldown);
    }

    public virtual void Fire()
    {
        if (cooldown > 0)
            return;

        cooldown = 1;

        if (ammo <= 0)
        {
            if (fireNoAmmo)
                AudioSource.PlayClipAtPoint(fireNoAmmo, transform.position);

            return;
        }

        ammo--;

        if (fireFlash)
            fireFlash.enabled = true;

        StartCoroutine(Flash());

        if (fireShot)
        {
            fireShot.PlayOnce(transform.position);
            //AudioSource.PlayClipAtPoint(fireShot, transform.position);

            PoolingManager.e.PlayTail(transform.position);
        }

        if (fireParticles)
            fireParticles.Emit(10);
    }

    IEnumerator Flash()
    {
        yield return null;
        yield return null;
        yield return null;

        fireFlash.enabled = false;
    }

    public bool CanBeFired()
    {
        if (cooldown <= 0) return true;

        return false;
    }

    public bool CanBeShot()
    {
        if (CanBeFired() && ammo > 0)
            return true;

        return false;
    }
}
