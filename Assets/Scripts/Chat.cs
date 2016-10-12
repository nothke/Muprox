using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class Chat : NetworkBehaviour
{

    public string nick = "playerName";

    string message = "";

    static Queue<string> messageQueue = new Queue<string>();

    void OnGUI()
    {
        if (!isLocalPlayer) return;

        GUILayout.BeginArea(new Rect(10, 200, 300, 1000));

        GUI.color = Color.white;

        if (Event.current.Equals(Event.KeyboardEvent("return")) && !string.IsNullOrEmpty(message))
        {
            CmdSendChat(nick + ": " + message);
            message = "";
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("Nick");
        nick = GUILayout.TextField(nick, GUILayout.Width(200));
        GUILayout.EndHorizontal();

        message = GUILayout.TextField(message);

        foreach (var line in messageQueue)
        {
            GUI.color = HashColor(line.Split(':')[0]);
            GUILayout.Label(line);
        }

        GUILayout.EndArea();

    }

    Color HashColor(string str)
    {
        System.Random rnd = new System.Random(str.GetHashCode());
        float hue = (float)rnd.NextDouble();

        return Color.HSVToRGB(Mathf.Clamp01(hue), 0.7f, 1);
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
