using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class FlyerSpawner : NetworkBehaviour
{

    public GameObject flyerPrefab;

    public override void OnStartServer()
    {
        GameObject go = Instantiate(flyerPrefab, transform.position, Quaternion.identity) as GameObject;

        NetworkServer.Spawn(go);
    }
}
