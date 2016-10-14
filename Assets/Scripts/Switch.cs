using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Switch : Interactable
{
    public GameObject objectToSwitch;

    [SyncVar(hook = "Set")]
    public bool isOn;

    public override void OnStartClient()
    {
        base.OnStartClient();

        Set(isOn);
    }

    void Set(bool isOn)
    {
        objectToSwitch.SetActive(isOn);
    }

    public override void Act()
    {
        base.Act();

        isOn = !isOn;
        Set(isOn);
    }
}
