using UnityEngine;
using System.Collections;

public class PoolingManager : MonoBehaviour
{
    public static PoolingManager e;
    void Awake() { e = this; }

    public AudioClip tail;

    public ParticleSystem shotConcreteParticle;
    public ParticleSystem shotMetalParticle;
    public ParticleSystem shotBloodParticle;
    public ParticleSystem explosionParticle;

    public UnityEngine.UI.Text outputUI;
    public UnityEngine.UI.InputField inputUI;

    public enum SurfaceType { Concrete, Metal, Blood };

    public void DoSurfaceShotParticle(SurfaceType surface, Vector3 point, Vector3 normal)
    {
        ParticleSystem ps = null;

        switch (surface)
        {
            case SurfaceType.Concrete:
                ps = shotConcreteParticle;
                break;
            case SurfaceType.Metal:
                ps = shotMetalParticle;
                break;
            case SurfaceType.Blood:
                ps = shotBloodParticle;
                break;
        }

        ps.transform.position = point;
        ps.transform.rotation = Quaternion.LookRotation(normal);

        ps.Emit(25);
    }

    public int GetSurfaceType(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            return 2;

        if (collider.sharedMaterial == null)
            return 0;

        if (collider.sharedMaterial.name == "Metal")
            return 1;

        Debug.Log(collider.gameObject.layer);



        return 0;
    }

    public void PlayTail(Vector3 point)
    {
        tail.PlayOnce(point, 0.7f, 1, 150, 4);
    }
}
