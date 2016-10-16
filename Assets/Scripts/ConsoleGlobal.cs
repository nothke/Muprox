using UnityEngine;
using System.Collections;
//using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.UI;

public class ConsoleGlobal : MonoBehaviour
{
    public static ConsoleGlobal e;
    void Awake() { e = this; }

    string message = "";

    static Queue<string> messageQueue = new Queue<string>();

    //public bool useGUI = true;

    public GameObject consoleUI;
    public InputField inputUI;
    public Text outputUI;


    [HideInInspector]
    public ConsoleController console = new ConsoleController();

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

    ~ConsoleGlobal()
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

        if (FocusButtonPressed())
            Focus();


        if (EnterIsPressed() && !InputIsEmpty())
            ProcessInput(inputUI.text);

        if (UnfocusButtonPressed())
            Unfocus();
    }

    bool UnfocusButtonPressed()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            return true;

        return false;
    }

    bool FocusButtonPressed()
    {
        return NInput.GetKeyDown(KeyCode.T) || NInput.GetKey(KeyCode.Caret);
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



        if (inputString[0] == '\\')
        {
            console.runCommandString(inputString);
            //console.
            //PushMessage("Invalid command");
        }
        else
            // if not a command, it's considered to be public chat
            console.appendLogLine(inputString);

        //CmdSendChat(nick + ": " + message);
        //inputUI.text = inputString;

        ClearInput();

        Focus();
    }

    void ClearInput()
    {
        inputUI.text = "";
    }

    void Focus()
    {
        NInput.bypass = true;

        consoleUI.SetActive(true);

        inputUI.Select();
        inputUI.ActivateInputField();


    }

    void Unfocus()
    {
        NInput.bypass = false;

        inputUI.DeactivateInputField();

        consoleUI.SetActive(false);
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

    /*
    [Command]
    void CmdSendChat(string msg)
    {
        //nickNet = nick;
        RpcSendChat(msg);
    }*/

    /*
    [ClientRpc]
    void RpcSendChat(string msg)
    {
        //if (!isLocalPlayer) return;

        Debug.Log(msg);
        PushMessage(msg);
    }*/

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

    public static void Log(string str)
    {
        e.console.appendLogLine(str);
    }
}