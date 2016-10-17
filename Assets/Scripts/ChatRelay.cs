using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ChatRelay : NetworkBehaviour
{
    [Command]
    public void CmdSendChat(string what)
    {
        RpcSendChat(what);
    }

    [ClientRpc]
    void RpcSendChat(string what)
    {
        ConsoleGlobal.Log(what);
    }
}
