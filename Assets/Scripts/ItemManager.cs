using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ItemManager : NetworkBehaviour
{

    Weapon weapon;

    Weapon hoverWeapon;

    public Transform raycastFrom;

    void Update()
    {


        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.Q))
            CmdTest();

        RaycastHit hit;
        if (Physics.Raycast(raycastFrom.position, raycastFrom.forward, out hit, Mathf.Infinity))
        {
            if (hit.collider)
            {
                if (!weapon)
                {
                    hoverWeapon = hit.collider.GetComponent<Weapon>();

                    if (hoverWeapon)
                    {
                        if (Input.GetMouseButtonDown(1))
                        {
                            CmdDestroyWeapon(hit.collider.gameObject);
                        }
                    }

                }
            }
            else hoverWeapon = null;
        }
        else hoverWeapon = null;
    }

    [Command]
    void CmdDestroyWeapon(GameObject weaponGO)
    {
        Destroy(weaponGO.GetComponent<NetworkTransform>());

        //weaponGO.GetComponent<>

        //Destroy(weaponGO);
        //RpcDestroyWeapon();
    }

    [ClientRpc]
    void RpcDestroyWeapon()
    {
        Destroy(hoverWeapon.gameObject);
    }

    [ClientRpc]
    void RpcTakeWeapon()
    {

    }

    [Command]
    void CmdTest()
    {

        RpcTest();
    }

    [ClientRpc]
    void RpcTest()
    {
        Debug.Log("Test message sent from: " + gameObject.name);

    }

    void OnGUI()
    {
        if (hoverWeapon)
            GUI.Label(new Rect(Screen.width / 2, 20, 1000, 100), hoverWeapon.name + "\n right click to take");
    }
}
