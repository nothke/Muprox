using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ObjectSpawner : NetworkBehaviour
{

    public GameObject prefab;

    public override void OnStartServer()
    {
        GameObject go = Instantiate(prefab, transform.position, transform.rotation) as GameObject;

        NetworkServer.Spawn(go);
    }


}
