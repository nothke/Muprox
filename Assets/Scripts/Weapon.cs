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

    Vector3 holdPosition = new Vector3(0.2f, -0.2f, 0.2f);
    public Transform aimPivot;
    public Transform handPivot;

    public GameObject nonNetworkPrefab;

    [HideInInspector]
    public float cooldown = 0;
    public float cooldownSeconds = 0.1f;

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
            AudioSource.PlayClipAtPoint(fireShot, transform.position);

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
}
