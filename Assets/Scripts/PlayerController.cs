﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    [SyncVar]
    public string nick = "Unnamed";

    public Transform head;

    CharacterController controller;

    public GameObject bulletPrefab;

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
    float walkSpeedMult = 5;
    float runSpeedMult = 10;

    float mouseSensitivity = 1;

    float totalX, totalY;

    public Component[] componentsToDisable;

    public static PlayerController client;

    public override void OnStartClient()
    {
        base.OnStartClient();


        ConsoleGlobal.Log("Client started!");

        if (!isLocalPlayer) return;

        ConsoleGlobal.Log("Client started and is local player!");
        //ConsoleGlobal.Log("Local player started!");

        client = this;
    }

    public override void OnStartLocalPlayer()
    {
        ConsoleGlobal.Log("Welcome! Please, set you nick by using \\nick [yourName]");

        client = this;
    }

    void Update()
    {
        if (!isLocalPlayer)
            return;

        float iH = NInput.GetAxis("Horizontal");
        float iV = NInput.GetAxis("Vertical");

        speedMult = NInput.GetKey(KeyCode.LeftShift) ? runSpeedMult : walkSpeedMult;

        Vector3 iVec = new Vector3(iH, 0, iV);

        Vector3 move = iVec.normalized * speedSlope.Evaluate(iVec.magnitude) * speedMult;// * Time.deltaTime;

        move = transform.forward * move.z + transform.right * move.x;

        // vertical speed
        if (controller.isGrounded)
        {
            vSpeed = 0;

            if (NInput.GetKeyDown(KeyCode.Space))
                vSpeed = 8;
        }

        vSpeed -= gravity * Time.deltaTime;
        vSpeed = Mathf.Clamp(vSpeed, -54, 54);
        move.y = vSpeed;
        //float x = speedSlope.Evaluate(Mathf.Abs(f)) * Time.deltaTime;
        //float y = speedSlope.Evaluate(Input.GetAxis("Vertical")) * Time.deltaTime;

        controller.Move(move * Time.deltaTime);

        // MOUSELOOK
        float rotX = NInput.GetAxis("Mouse X") * mouseSensitivity;
        float rotY = NInput.GetAxis("Mouse Y") * mouseSensitivity;

        totalX += rotX;
        totalY += rotY;

        totalY = Mathf.Clamp(totalY, -85, 85);

        Quaternion yQuaternion = Quaternion.AngleAxis(totalY, Vector3.left);
        Quaternion xQuaternion = Quaternion.AngleAxis(totalX, Vector3.up);

        head.localRotation = yQuaternion;
        transform.localRotation = xQuaternion;

        if (NInput.GetKeyDown(KeyCode.B))
            CmdBoxSpawn();

    }

    [Command]
    void CmdBoxSpawn()
    {
        var bullet = Instantiate(bulletPrefab, head.position + head.forward, head.rotation) as GameObject;

        NetworkServer.Spawn(bullet);

        bullet.GetComponent<Rigidbody>().AddForce(head.forward * 1000);
    }

    [Client]
    public void SpawnInFront(GameObject prefab)
    {
        Debug.Log("SpawnInFront begin");

        CmdSpawnInFront(prefab);

        Debug.Log("SpawnInFront end");
    }

    [Command]
    void CmdSpawnInFront(GameObject prefab)
    {
        Debug.Log("CmdSpawnInFront begin");

        // TODO: check if it has authority

        Vector3 position = head.position + head.forward * 2;

        var go = Instantiate(prefab, position, head.rotation) as GameObject;
        NetworkServer.Spawn(go);

        ConsoleGlobal.Log(nick + " has spawned " + prefab.name, true);
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