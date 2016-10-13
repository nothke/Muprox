using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ItemManager : NetworkBehaviour
{

    Weapon weapon;

    Weapon hoverWeapon;

    public Transform hand;

    public Transform raycastFrom;

    void Start()
    {
        handStartPos = hand.transform.localPosition;
        handTargetPos = handStartPos;
    }

    public Vector3 handStartPos;
    Vector3 handTargetPos;

    void Update()
    {


        if (!isLocalPlayer) return;

        UpdateHand();

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
                            Take(hoverWeapon);
                        }
                    }

                }
            }
            else hoverWeapon = null;
        }
        else hoverWeapon = null;



        if (Input.GetMouseButton(0))
            Use();

        if (Input.GetMouseButtonDown(1))
            StartAim();

        if (Input.GetMouseButtonUp(1))
            EndAim();

        mouseSpeed = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);

        //lastPos = handTargetPos;
    }

    Vector3 handAimPos = new Vector3(0, -0.1f, 0.5f);

    void Take(Weapon _weapon)
    {
        GameObject go = Instantiate(_weapon.nonNetworkPrefab) as GameObject;

        Weapon n_Weapon = go.GetComponent<Weapon>();

        go.transform.parent = hand;
        go.transform.localPosition = -n_Weapon.handPivot.localPosition;
        go.transform.localRotation = Quaternion.identity;

        handAimPos = n_Weapon.handPivot.localPosition - n_Weapon.aimPivot.localPosition + Vector3.forward * 0.4f;

        CmdDestroyWeapon(_weapon.gameObject);

        weapon = n_Weapon;
    }

    Vector3 mouseSpeed;

    Vector3 handRefVelo;
    //Vector3 lastPos;
    float handSmooth = 0.1f;

    Vector3 handRecoilPos;

    void UpdateHand()
    {
        Vector3 swayOffset = -mouseSpeed * Time.deltaTime;

        hand.localPosition = Vector3.SmoothDamp(hand.localPosition, handTargetPos + handRecoilPos + swayOffset, ref handRefVelo, handSmooth);
    }

    void Use()
    {
        if (weapon is BallisticWeapon)
        {
            BallisticWeapon bw = weapon as BallisticWeapon;

            if (bw.cooldown != 0) return;
        }

        handRecoilPos = new Vector3(0, 0, -0.1f);
        handSmooth = 0.001f;

        StopCoroutine("Jerk");
        StartCoroutine("Jerk");

        if (weapon)
        {
            weapon.Fire();
        }
    }

    IEnumerator Jerk()
    {
        yield return new WaitForSeconds(0.1f);
        handSmooth = 0.1f;
        handRecoilPos = Vector3.zero;
    }

    void StartAim()
    {
        handTargetPos = handAimPos;
    }

    void EndAim()
    {
        handTargetPos = handStartPos;
    }

    [Command]
    void CmdDestroyWeapon(GameObject weaponGO)
    {
        //Destroy(weaponGO.GetComponent<NetworkTransform>());

        //weaponGO.GetComponent<>

        Destroy(weaponGO);
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
