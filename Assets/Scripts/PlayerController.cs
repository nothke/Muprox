using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerController : NetworkBehaviour
{
    [SyncVar]
    public string nick;


    public Transform head;

    CharacterController controller;

    public Text chat;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (!isLocalPlayer)
            DisableCamera();
    }

    public AnimationCurve speedSlope;

    float vSpeed;
    float gravity = 9.8f;
    float speedMult = 5;

    float mouseSensitivity = 1;

    float totalX, totalY;

    public Component[] componentsToDisable;

    void Update()
    {
        chat.text = nick;

        if (!isLocalPlayer)
            return;

        float iH = Input.GetAxis("Horizontal");
        float iV = Input.GetAxis("Vertical");

        Vector3 iVec = new Vector3(iH, 0, iV);

        Vector3 move = iVec.normalized * speedSlope.Evaluate(iVec.magnitude) * speedMult * Time.deltaTime;

        move = transform.forward * move.z + transform.right * move.x;

        // vertical speed
        if (controller.isGrounded)
            vSpeed = 0;

        vSpeed -= gravity * Time.deltaTime * 0.1f;
        vSpeed = Mathf.Clamp(vSpeed, -54, 54);
        move.y = vSpeed;
        //float x = speedSlope.Evaluate(Mathf.Abs(f)) * Time.deltaTime;
        //float y = speedSlope.Evaluate(Input.GetAxis("Vertical")) * Time.deltaTime;

        //Vector3 move = new Vector3(x, 0, y);

        controller.Move(move);

        // MOUSELOOK
        float rotX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float rotY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        totalX += rotX;
        totalY += rotY;

        Quaternion yQuaternion = Quaternion.AngleAxis(totalY, Vector3.left);
        Quaternion xQuaternion = Quaternion.AngleAxis(totalX, Vector3.up);

        head.localRotation = yQuaternion;
        transform.localRotation = xQuaternion;
    }

    void OnGUI()
    {
        if (!isLocalPlayer) return;

        nick = GUI.TextField(new Rect(10, 200, 100, 30), nick);
    }


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