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

        UpdateRaycast();


        if (Input.GetMouseButton(0))
            Use();

        if (Input.GetMouseButtonDown(1))
            StartAim();

        if (Input.GetMouseButtonUp(1))
            EndAim();

        if (Input.GetKeyDown(KeyCode.Q))
            ParentDrop();

        mouseSpeed = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
    }

    Collider hoverCollider;

    void UpdateRaycast()
    {
        hoverWeapon = null;
        hoverCollider = null;

        RaycastHit hit;
        if (Physics.Raycast(raycastFrom.position, raycastFrom.forward, out hit, Mathf.Infinity))
        {
            if (!hit.collider) return;

            hoverCollider = hit.collider;

            hoverWeapon = hit.collider.GetComponent<Weapon>();

            if (!hoverWeapon) return;

            if (Input.GetMouseButtonDown(1))
                Take(hoverWeapon);
        }

    }

    Vector3 handAimPos = new Vector3(0, -0.1f, 0.5f);

    void Take(Weapon _weapon)
    {
        if (weapon)
        {
            Debug.Log("Already holding a weapon");
            return;
        }

        PositionWeaponAtHand(_weapon);
        SetAimPos(_weapon);
        DisablePhysics(_weapon);

        CmdSetParent(_weapon.gameObject, netId);

        weapon = _weapon;
    }

    void ParentDrop()
    {
        if (!weapon) return;

        weapon.transform.parent = null;
        EnablePhysics(weapon);

        CmdNullifyParent(weapon.gameObject);

        weapon = null;
    }

    [Command]
    void CmdSetParent(GameObject _weapon, NetworkInstanceId id)
    {
        _weapon.GetComponent<Parentable>().parentNetId = id;

        PositionWeaponAtHand(_weapon.GetComponent<Weapon>());
        _weapon.GetComponent<NetworkTransform>().enabled = false;
        DisablePhysics(_weapon.GetComponent<Weapon>());

        Debug.Log("Command Set Parent to " + id);

        RpcSetParent(_weapon);
    }



    [ClientRpc]
    void RpcSetParent(GameObject _weapon)
    {
        PositionWeaponAtHand(_weapon.GetComponent<Weapon>());
        _weapon.GetComponent<NetworkTransform>().enabled = false;
        DisablePhysics(_weapon.GetComponent<Weapon>());
    }

    [Command]
    void CmdNullifyParent(GameObject _weapon)
    {
        _weapon.GetComponent<Parentable>().parentNetId = new NetworkInstanceId(0);
        _weapon.GetComponent<NetworkTransform>().enabled = true;

        RpcNullifyParent(_weapon);
    }

    [ClientRpc]
    void RpcNullifyParent(GameObject _weapon)
    {
        _weapon.transform.parent = null;
        EnablePhysics(_weapon.GetComponent<Weapon>());
        _weapon.GetComponent<NetworkTransform>().enabled = true;
    }


    void PositionWeaponAtHand(Weapon weapon)
    {
        weapon.transform.parent = hand;
        weapon.transform.localPosition = -weapon.handPivot.localPosition;
        weapon.transform.localRotation = Quaternion.identity;
    }

    void SetAimPos(Weapon weapon)
    {
        handAimPos = weapon.handPivot.localPosition - weapon.aimPivot.localPosition + Vector3.forward * 0.4f;
    }

    void DisablePhysics(Weapon weapon)
    {
        weapon.GetComponent<Rigidbody>().isKinematic = true;
        weapon.GetComponent<Collider>().enabled = false;
    }

    void EnablePhysics(Weapon weapon)
    {
        weapon.GetComponent<Rigidbody>().isKinematic = false;
        weapon.GetComponent<Collider>().enabled = true;
    }

    [Command]
    void CmdPush(GameObject go, Vector3 point, Vector3 direction)
    {
        Rigidbody _rigidbody = go.GetComponent<Rigidbody>();

        if (_rigidbody)
            _rigidbody.AddForceAtPosition(direction, point);
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
        if (weapon && weapon.cooldown != 0) return;

        handRecoilPos += new Vector3(0, 0, -0.1f) + Random.onUnitSphere * 0.02f;
        handSmooth = 0.01f;

        StopCoroutine("Jerk");
        StartCoroutine("Jerk");

        if (weapon)
        {
            CmdGunFire(weapon.gameObject);

            weapon.Fire();

            ShootRay(weapon);
        }
    }

    [Command]
    void CmdGunFire(GameObject _weapon)
    {
        RpcGunFire(_weapon);
    }

    [ClientRpc]
    void RpcGunFire(GameObject _weapon)
    {
        _weapon.GetComponent<Weapon>().Fire();
    }

    void ShootRay(Weapon _weapon)
    {
        RaycastHit hit;
        if (Physics.Raycast(_weapon.muzzle.position, _weapon.muzzle.forward, out hit, Mathf.Infinity))
        {
            Rigidbody rb = FindRigidbody(hit.collider);

            if (rb)
            {
                CmdPush(rb.gameObject, hit.point, _weapon.muzzle.forward * 300);
            }

            CmdDoVisual(hit.point, hit.normal);
        }
    }

    Rigidbody FindRigidbody(Collider collider)
    {
        Rigidbody rb = collider.GetComponent<Rigidbody>();

        Transform lastParent = collider.transform;
        lastParent = collider.transform.parent;

        for (int i = 0; i < 100; i++)
        {
            if (rb) break;
            if (!lastParent) break;

            rb = lastParent.GetComponent<Rigidbody>();
            lastParent = lastParent.parent;
        }

        return rb;
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

    [Command]
    void CmdDoVisual(Vector3 atPoint, Vector3 normal)
    {
        RpcDoVisual(atPoint, normal);
    }

    [ClientRpc]
    void RpcDoVisual(Vector3 atPoint, Vector3 normal)
    {
        PoolingManager.e.WallParticle(atPoint, normal);
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

    const string hoverThing = "\n right click to take";

    void OnGUI()
    {
        string displayStr = "";

        if (hoverWeapon)
            displayStr = hoverWeapon.name + (weapon ? "" : hoverThing);

        if (hoverCollider)
            if (hoverCollider.GetComponent<Chat>())
                displayStr = hoverCollider.GetComponent<Chat>().displayNick;



        GUI.Label(new Rect(Screen.width / 2, 20, 1000, 100), displayStr);
    }
}
