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


        GameObject go2 = Instantiate(flyerPrefab, transform.position + Vector3.right * 4, Quaternion.identity) as GameObject;

        NetworkServer.Spawn(go2);
    }


}
