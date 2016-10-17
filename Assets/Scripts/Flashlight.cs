using UnityEngine;
using System.Collections;

public class Flashlight : Item
{

    public GameObject toggleObject;

    public override void ActItem()
    {
        toggleObject.SetActive(!toggleObject.activeSelf);
    }
}
