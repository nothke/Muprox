using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Item : Interactable
{
    public Transform handPivot;

    public void UseItem()
    {
        if (isServer)
            ActItem();
        else
            CmdUseItem();
    }

    [Command]
    void CmdUseItem()
    {
        Act();
        Debug.Log("This is a test");
    }

    public virtual void ActItem() { }
}
