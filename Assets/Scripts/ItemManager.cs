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
        CmdInstItemInHand(_weapon.gameObject);

        Weapon takenWeapon = go.GetComponent<Weapon>();

        PositionWeaponAtHand(takenWeapon);

        handAimPos = takenWeapon.handPivot.localPosition - takenWeapon.aimPivot.localPosition + Vector3.forward * 0.4f;

        //CmdDestroyWeapon(_weapon.gameObject);

        weapon = takenWeapon;
    }

    void PositionWeaponAtHand(Weapon weapon)
    {
        weapon.transform.parent = hand;
        weapon.transform.localPosition = -weapon.handPivot.localPosition;
        weapon.transform.localRotation = Quaternion.identity;
    }

    [Command]
    void CmdInstItemInHand(GameObject worldObject)
    {
        RpcInstItemInHand(worldObject);

        Destroy(worldObject);

        Debug.Log("Took " + worldObject.name);
    }

    [ClientRpc]
    void RpcInstItemInHand(GameObject worldObject)
    {
        if (isClient) return;

        if (!worldObject) Debug.Log("no network prefab");

        GameObject go = Instantiate(worldObject.GetComponent<Weapon>().nonNetworkPrefab) as GameObject;

        Weapon _weapon = go.GetComponent<Weapon>();

        PositionWeaponAtHand(_weapon);

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
            weapon.Fire();

            ShootRay(weapon);
        }
    }

    void ShootRay(Weapon _weapon)
    {
        RaycastHit hit;
        if (Physics.Raycast(_weapon.muzzle.position, _weapon.muzzle.forward, out hit, Mathf.Infinity))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();

            Transform lastParent = hit.collider.transform;
            lastParent = hit.collider.transform.parent;

            for (int i = 0; i < 100; i++)
            {
                if (rb) break;
                if (!lastParent) break;

                rb = lastParent.GetComponent<Rigidbody>();
                lastParent = lastParent.parent;
            }


            if (rb)
            {
                Debug.Log("Pushing: " + rb.gameObject.name);
                CmdPush(rb.gameObject, hit.point, _weapon.muzzle.forward * 300);
            }

            CmdDoVisual(hit.point, hit.normal);
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
