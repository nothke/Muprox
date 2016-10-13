using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    //[SyncVar(hook = "CmdOnNickChange")]
    public string nick;
    string prevNick;

    public Transform head;

    CharacterController controller;

    public Text chat;

    public GameObject bulletPrefab;
    public Transform gunpoint;

    public Weapon weapon;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (!isLocalPlayer)
            DisableCamera();
    }

    public AnimationCurve speedSlope;

    float vSpeed;
    float gravity = 20;
    float speedMult = 5;

    float mouseSensitivity = 1;

    float totalX, totalY;

    public Component[] componentsToDisable;



    //[Command]
    void OnNickChange(string _nick)
    {
        chat.text = _nick;
    }

    [Command]
    void CmdChangeNick()
    {
        chat.text = nick;
    }

    [Command]
    void CmdFire()
    {
        //if (!weapon) return;

        //weapon.CmdFire();


        var bullet = Instantiate(bulletPrefab, gunpoint.position, gunpoint.rotation) as GameObject;

        NetworkServer.Spawn(bullet);

        bullet.GetComponent<Rigidbody>().AddForce(gunpoint.forward * 1000);
    }

    [Command]
    void CmdPush(GameObject go, Vector3 point, Vector3 direction)
    {
        Rigidbody _rigidbody = go.GetComponent<Rigidbody>();

        if (_rigidbody)
            _rigidbody.AddForceAtPosition(direction * 1000, point);



    }

    void Update()
    {
        //if (prevNick != nick)
        //CmdChangeNick();

        prevNick = nick;

        if (!isLocalPlayer)
            return;

        float iH = Input.GetAxis("Horizontal");
        float iV = Input.GetAxis("Vertical");

        Vector3 iVec = new Vector3(iH, 0, iV);

        Vector3 move = iVec.normalized * speedSlope.Evaluate(iVec.magnitude) * speedMult;// * Time.deltaTime;

        move = transform.forward * move.z + transform.right * move.x;

        // vertical speed



        if (controller.isGrounded)
        {
            vSpeed = 0;

            if (Input.GetKeyDown(KeyCode.Space))
                vSpeed = 8;
        }

        vSpeed -= gravity * Time.deltaTime;
        vSpeed = Mathf.Clamp(vSpeed, -54, 54);
        move.y = vSpeed;
        //float x = speedSlope.Evaluate(Mathf.Abs(f)) * Time.deltaTime;
        //float y = speedSlope.Evaluate(Input.GetAxis("Vertical")) * Time.deltaTime;

        //Vector3 move = new Vector3(x, 0, y);

        controller.Move(move * Time.deltaTime);

        // MOUSELOOK
        float rotX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float rotY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        totalX += rotX;
        totalY += rotY;

        Quaternion yQuaternion = Quaternion.AngleAxis(totalY, Vector3.left);
        Quaternion xQuaternion = Quaternion.AngleAxis(totalX, Vector3.up);

        head.localRotation = yQuaternion;
        transform.localRotation = xQuaternion;


        //if (Input.GetMouseButtonDown(0))
            //CmdFire();

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            if (Physics.Raycast(head.position, head.forward, out hit, Mathf.Infinity))
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
                    CmdPush(rb.gameObject, hit.point, head.forward);
                }
            }
        }
    }

    /*
    void OnGUI()
    {
        if (!isLocalPlayer) return;

        nick = GUI.TextField(new Rect(10, 200, 100, 30), nick);
    }*/


    void DisableCamera()
    {
        if (componentsToDisable.Length == 0) return;

        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            if (componentsToDisable[i] != null)
                Destroy(componentsToDisable[i]);
        }
    }
}