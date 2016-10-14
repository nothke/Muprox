using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Interactable : NetworkBehaviour
{
    new public string name;

    public virtual void Use()
    {
        if (isServer)
            Act();
        else
            CmdUse();
    }

    [Command]
    void CmdUse()
    {
        Act();
        Debug.Log("This is a test");
    }

    public virtual void Act()
    {

    }
}
