using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.UI;

public class ConsoleView : NetworkBehaviour
{
    //string message = "";

    static Queue<string> messageQueue = new Queue<string>();

    //public bool useGUI = true;

    public InputField inputUI;
    public Text outputUI;

    public override void OnStartClient()
    {
        base.OnStartClient();

    }

    ConsoleController console = new ConsoleController();

    void Start()
    {
        inputUI = PoolingManager.e.inputUI;
        outputUI = PoolingManager.e.outputUI;

        console.visibilityChanged += Console_visibilityChanged;
        console.logChanged += UpdateConsole;
    }

    void UpdateConsole(string[] newLog)
    {
        if (newLog == null)
            outputUI.text = "";
        else
            outputUI.text = string.Join("\n", newLog);
    }

    ~ConsoleView()
    {
        console.visibilityChanged -= Console_visibilityChanged;
        console.logChanged -= UpdateConsole;
    }

    private void Console_visibilityChanged(bool visible)
    {
        throw new System.NotImplementedException();
    }

    void Update()
    {
        if (!UIExists()) return;

        if (EnterIsPressed() && !InputIsEmpty())
            ProcessInput(inputUI.text);


    }

    bool UIExists()
    {
        return inputUI && outputUI;
    }

    bool EnterIsPressed()
    {
        // Doesn't work outside of OnGUI():
        //return Event.current.Equals(Event.KeyboardEvent("return"));

        return Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);
    }

    bool InputIsEmpty()
    {
        return string.IsNullOrEmpty(inputUI.text);
    }

    void ProcessInput(string inputString)
    {
        if (string.IsNullOrEmpty(inputString)) return;

        console.runCommandString(inputString);

        if (inputString[0] == '\\')
        {

            //console.
            //PushMessage("Invalid command");
        }

        // if not a command, it's considered to be public chat
        //CmdSendChat(nick + ": " + message);
        //inputUI.text = inputString;

        ClearInput();
    }

    void ClearInput()
    {
        inputUI.text = "";
    }

    void ActivateInputUI()
    {
        inputUI.Select();
        inputUI.ActivateInputField();
    }

    void UpdateMessage()
    {
        outputUI.text = Convert(messageQueue);
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
        //nickNet = nick;
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

        UpdateMessage();
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
