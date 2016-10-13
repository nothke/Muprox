using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Parentable : NetworkBehaviour
{
    [SyncVar]
    public NetworkInstanceId parentNetId;

    public override void OnStartClient()
    {

        if (parentNetId.IsEmpty()) return;

        GameObject parentObject = ClientScene.FindLocalObject(parentNetId);

        Transform parent = parentObject.transform;

        if (parentObject.GetComponent<ItemManager>())
            parent = parentObject.GetComponent<ItemManager>().hand;

        // Set parent and position
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        DisablePhysics();
    }

    void DisablePhysics()
    {
        Collider col = GetComponent<Collider>();
        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb) rb.isKinematic = true;
        if (col) col.enabled = false;
    }
}
