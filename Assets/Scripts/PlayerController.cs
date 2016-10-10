using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public Transform head;

    CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    public AnimationCurve speedSlope;

    float vSpeed;
    float gravity = 9.8f;
    float speedMult = 10;

    float mouseSensitivity = 1;

    float totalX, totalY;

    void Update()
    {
        float iH = Input.GetAxis("Horizontal");
        float iV = Input.GetAxis("Vertical");

        Vector3 iVec = new Vector3(iH, 0, iV);

        Vector3 move = iVec.normalized * speedSlope.Evaluate(iVec.magnitude) * speedMult * Time.deltaTime;

        move = transform.forward * move.z + transform.right * move.x;

        // vertical speed
        if (controller.isGrounded)
            vSpeed = 0;

        vSpeed -= gravity * Time.deltaTime;
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
}
