using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SpawnerButton : Interactable
{

    public GameObject prefab;

    public Transform location;

    public override void Act()
    {
        base.Act();

        GameObject go = Instantiate(prefab, location.position, location.rotation) as GameObject;

        NetworkServer.Spawn(go);
    }
}
