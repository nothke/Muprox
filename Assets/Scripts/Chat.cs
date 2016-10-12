using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Chat : NetworkBehaviour
{
    public string nick = "playerName";

    void Start()
    {

    }

    void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.Return))
            CmdSendChat();
    }

    public string message;

    [SyncVar]
    public string messages;

    void OnGUI()
    {
        if (!isLocalPlayer) return;

        nick = GUI.TextField(new Rect(10, 200, 100, 30), nick);
        message = GUI.TextField(new Rect(10, 250, 200, 30), message);

        GUI.Label(new Rect(10, 300, 200, 1000), messages);
    }

    [Command]
    void CmdSendChat()
    {
        RpcSendChat(nick + ": " + message);
    }

    [ClientRpc]
    void RpcSendChat(string msg)
    {
        Debug.Log(msg);
        messages += "\n" + msg;
    }
}
