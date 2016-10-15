using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ItemManager : NetworkBehaviour
{

    Weapon weapon;
    Item itemInHands;

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


    bool triggerDown = false;

    void Update()
    {
        if (!isLocalPlayer) return;

        UpdateHand();

        UpdateRaycast();


        if (NInput.GetMouseButton(0))
            Use();

        if (NInput.GetMouseButtonUp(0))
            triggerDown = false;

        if (NInput.GetMouseButtonDown(1))
            StartAim();

        if (NInput.GetMouseButtonUp(1))
            EndAim();

        if (NInput.GetKeyDown(KeyCode.Q))
        {
            ParentDrop();
            Drop();
        }

        mouseSpeed = new Vector3(NInput.GetAxis("Mouse X"), NInput.GetAxis("Mouse Y"), 0);
    }

    Collider hoverCollider;

    void UpdateRaycast()
    {
        hoverWeapon = null;
        hoverCollider = null;

        RaycastHit hit;

        if (!Physics.Raycast(raycastFrom.position, raycastFrom.forward, out hit, Mathf.Infinity))
            return;

        if (!hit.collider) return;

        hoverCollider = hit.collider;

        // hover weapon

        hoverWeapon = hit.collider.GetComponent<Weapon>();

        if (hoverWeapon)
        {

            if (NInput.GetMouseButtonDown(1))
                Take(hoverWeapon);
        }



        // interactable

        Interactable interactable = hit.collider.GetComponent<Interactable>();

        if (interactable)
        {
            if (NInput.GetMouseButtonDown(1))
                if (isServer)
                    interactable.Act();
                else
                    CmdInteract(interactable.gameObject);

        }

        if (interactable is Item)
        {
            if (NInput.GetMouseButtonDown(1))
            {
                Item item = interactable as Item;
                Take(item);
            }
        }
    }

    [Command]
    void CmdInteract(GameObject interactableObject)
    {
        interactableObject.GetComponent<Interactable>().Act();
    }

    Vector3 handAimPos = new Vector3(0, -0.1f, 0.5f);

    void Take(Item item)
    {
        if (itemInHands)
        {
            Debug.Log("Already holding an item");
            return;
        }

        PositionItemAtHand(item);
        //SetAimPos(item); // no aim for items??
        DisablePhysics(item);

        CmdSetItemParent(item.gameObject, netId);

        itemInHands = item;
    }

    // TBO
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

    // TBO
    void ParentDrop()
    {
        if (!weapon) return;

        weapon.transform.parent = null;
        EnablePhysics(weapon);

        CmdNullifyParent(weapon.gameObject);

        weapon = null;
    }

    void Drop()
    {
        if (!itemInHands) return;

        itemInHands.transform.parent = null;
        EnablePhysics(itemInHands);

        CmdNullifyParent(itemInHands.gameObject);

        itemInHands = null;
    }

    [Command]
    void CmdSetItemParent(GameObject itemObject, NetworkInstanceId parentId)
    {
        // integrate parentable into item?
        itemObject.GetComponent<Parentable>().parentNetId = parentId;

        Item item = itemObject.GetComponent<Item>();


        PositionItemAtHand(item);
        itemObject.GetComponent<NetworkTransform>().enabled = false;
        DisablePhysics(item);

        Debug.Log("Command Set Parent to " + parentId);

        RpcSetItemParent(itemObject);
    }

    [ClientRpc]
    void RpcSetItemParent(GameObject itemObject)
    {
        Item item = itemObject.GetComponent<Item>();

        PositionItemAtHand(item);
        itemObject.GetComponent<NetworkTransform>().enabled = false;
        DisablePhysics(item);
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
    void CmdNullifyParent(GameObject itemObject)
    {
        itemObject.GetComponent<Parentable>().parentNetId = new NetworkInstanceId(0);
        itemObject.GetComponent<NetworkTransform>().enabled = true;

        RpcNullifyParent(itemObject);
    }

    [ClientRpc]
    void RpcNullifyParent(GameObject itemObject)
    {
        itemObject.transform.parent = null;

        // TBO
        if (itemObject.GetComponent<Weapon>())
            EnablePhysics(itemObject.GetComponent<Weapon>());

        if (itemObject.GetComponent<Item>())
            EnablePhysics(itemObject.GetComponent<Item>());

        itemObject.GetComponent<NetworkTransform>().enabled = true;
    }

    // TBO
    void PositionWeaponAtHand(Weapon weapon)
    {
        weapon.transform.parent = hand;
        weapon.transform.localPosition = -weapon.handPivot.localPosition;
        weapon.transform.localRotation = Quaternion.identity;
    }

    void PositionItemAtHand(Item item)
    {
        item.transform.parent = hand;
        item.transform.localPosition = -item.handPivot.localPosition;
        item.transform.localRotation = Quaternion.identity;
    }

    void SetAimPos(Weapon weapon)
    {
        handAimPos = weapon.handPivot.localPosition - weapon.aimPivot.localPosition + Vector3.forward * 0.4f;
    }

    // TBO
    void DisablePhysics(Weapon weapon)
    {
        weapon.GetComponent<Rigidbody>().isKinematic = true;
        weapon.GetComponent<Collider>().enabled = false;
    }

    // TBO
    void EnablePhysics(Weapon weapon)
    {
        weapon.GetComponent<Rigidbody>().isKinematic = false;
        weapon.GetComponent<Collider>().enabled = true;
    }

    void DisablePhysics(Item item)
    {
        item.GetComponent<Rigidbody>().isKinematic = true;
        item.GetComponent<Collider>().enabled = false;
    }

    void EnablePhysics(Item item)
    {
        item.GetComponent<Rigidbody>().isKinematic = false;
        item.GetComponent<Collider>().enabled = true;
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
        if (weapon && !weapon.CanBeFired()) return;

        if (weapon && !weapon.repeat && triggerDown) return;

        triggerDown = true;

        DoRecoil();

        if (weapon)
        {
            CmdGunFire(weapon.gameObject);

            weapon.Fire();

            ShootRay(weapon);
        }
    }

    void DoRecoil()
    {
        handRecoilPos += new Vector3(0, 0, -0.1f) + Random.onUnitSphere * 0.02f;
        handSmooth = 0.01f;

        StopCoroutine("Jerk");
        StartCoroutine("Jerk");
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
        for (int i = 0; i < _weapon.buck; i++)
        {
            Vector3 rayDirection = _weapon.muzzle.forward + Random.insideUnitSphere * _weapon.spread;

            RaycastHit hit;
            if (Physics.Raycast(_weapon.muzzle.position, rayDirection, out hit, Mathf.Infinity))
            {


                Rigidbody rb = FindRigidbody(hit.collider);

                if (rb)
                {
                    CmdPush(rb.gameObject, hit.point, _weapon.muzzle.forward * 300);
                }

                Health health = hit.collider.GetComponent<Health>();

                if (!health && rb)
                    health = rb.GetComponent<Health>();

                if (health)
                    CmdTakeHealth(health.gameObject, _weapon.damage);

                CmdDoVisual(PoolingManager.e.GetSurfaceType(hit.collider), hit.point, hit.normal);
            }
        }
    }


    [Command]
    void CmdTakeHealth(GameObject playerObject, int amount)
    {
        playerObject.GetComponent<Health>().TakeDamage(amount);
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
    void CmdDoVisual(int surfaceType, Vector3 atPoint, Vector3 normal)
    {
        RpcDoVisual(surfaceType, atPoint, normal);
    }

    [ClientRpc]
    void RpcDoVisual(int surfaceType, Vector3 atPoint, Vector3 normal)
    {
        PoolingManager.e.DoSurfaceShotParticle((PoolingManager.SurfaceType)surfaceType, atPoint, normal);
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

    const string hoverItem = "\n right click to take";
    const string hoverInteractable = "\n right click to use";

    void OnGUI()
    {
        string displayStr = "";

        if (hoverWeapon)
            displayStr = hoverWeapon.name + (weapon ? "" : hoverItem);

        if (hoverCollider)
        {
            if (hoverCollider.GetComponent<Chat>())
                displayStr = hoverCollider.GetComponent<Chat>().displayNick;

            Interactable interactable = hoverCollider.GetComponent<Interactable>();

            if (interactable)
            {
                if (interactable is Item)
                    displayStr = hoverCollider.GetComponent<Interactable>().name + hoverItem;
                else
                    displayStr = hoverCollider.GetComponent<Interactable>().name + hoverInteractable;
            }
        }



        GUI.Label(new Rect(Screen.width / 2, 20, 1000, 100), displayStr);
    }
}
