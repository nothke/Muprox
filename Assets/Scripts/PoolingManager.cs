using UnityEngine;
using System.Collections;

public class PoolingManager : MonoBehaviour
{
    public static PoolingManager e;
    void Awake() { e = this; }


    public ParticleSystem shotParticle;

    public void WallParticle(Vector3 point, Vector3 normal)
    {
        shotParticle.transform.position = point;
        shotParticle.transform.rotation = Quaternion.LookRotation(normal);

        shotParticle.Play();
    }

}
