using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class Chat : NetworkBehaviour
{

    public string nick = "playerName";

    public string message;

    public static string[] messages = new string[10];
    public static Queue<string> messageQueue = new Queue<string>();

    void OnGUI()
    {
        if (!isLocalPlayer) return;

        if (Event.current.Equals(Event.KeyboardEvent("return")))
        {
            CmdSendChat(nick + ": " + message);
            message = "";
        }

        nick = GUI.TextField(new Rect(10, 200, 100, 30), nick);
        message = GUI.TextField(new Rect(10, 250, 200, 30), message);

        GUI.Label(new Rect(10, 300, 200, 1000), Convert(messageQueue));


    }

    [Command]
    void CmdSendChat(string msg)
    {
        RpcSendChat(msg);
    }

    [ClientRpc]
    void RpcSendChat(string msg)
    {
        //if (!isLocalPlayer) return;

        Debug.Log(msg);
        PushMessage(msg);
    }

    void PushMessage(string msg)
    {
        messageQueue.Enqueue(msg);

        if (messageQueue.Count == 10)
            messageQueue.Dequeue();

        /*
        for (int i = 0; i < messages.Length; i++)
        {
            if (string.IsNullOrEmpty(messages[i]))

        }*/
    }


    string Convert(Queue<string> lineQueue)
    {
        string str = "";

        foreach (var line in lineQueue)
        {
            str += "\n" + line;
        }

        return str;
    }
}
