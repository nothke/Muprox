using UnityEngine;
using System.Collections;

public class ScopeItem : Item
{

    public Camera renderCamera;

    public override void OnPickUp()
    {
        base.OnPickUp();

        renderCamera.enabled = true;
    }

    public override void OnDrop()
    {
        base.OnDrop();

        renderCamera.enabled = false;
    }
}
