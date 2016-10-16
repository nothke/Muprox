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
    public Text hudOutputUI;


    [HideInInspector]
    public ConsoleController console = new ConsoleController();

    string lastInput = "";

    void Start()
    {
        inputUI = PoolingManager.e.inputUI;
        outputUI = PoolingManager.e.outputUI;

        console.visibilityChanged += Console_visibilityChanged;
        console.logChanged += UpdateConsole;

        console.appendLogLine("-----------------------");
        console.appendLogLine("Open console with 'T' or '\\', close it on 'Esc'");
        console.appendLogLine("Type \\help to see a list of commands");
    }

    void UpdateConsole(string[] newLog)
    {
        if (newLog == null)
            outputUI.text = "";
        else
            outputUI.text = string.Join("\n", newLog);

        if (hudOutputUI)

        {
            int first = Mathf.Clamp(newLog.Length - 10, 0, 100000);
            int last = Mathf.Clamp(newLog.Length, 0, 10);
            hudOutputUI.text = string.Join("\n", newLog, first, last);
        }
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
        {
            lastInput = inputUI.text;
            Unfocus();

        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
            inputUI.text = console.GetPrevCommand();
    }

    bool UnfocusButtonPressed()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            return true;

        return false;
    }

    bool FocusButtonPressed()
    {
        //if (NInput.GetKey(KeyCode.Backslash))
        //lastInput = "\\";

        return NInput.GetKeyDown(KeyCode.T) || NInput.GetKey(KeyCode.Backslash);
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
            console.runCommandString(inputString);
        else
            // if not a command, it's considered to be public chat
            Chat(inputString);

        ClearInput();

        Focus();
    }

    void Chat(string inputString)
    {
        string message = System.Security.SecurityElement.Escape(inputString);

        string nick = "Me";

        if (PlayerController.client)
            nick = PlayerController.client.nick;

        message = ConsoleController.Colorize(nick + ": " + message, HashColor(nick));

        console.appendNetworkLogLine(message);
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

        inputUI.text = lastInput;

        StartCoroutine(MoveToEnd());
        //inputUI.caretPosition = inputUI.text.Length;
    }

    IEnumerator MoveToEnd()
    {
        yield return null;
        inputUI.MoveTextEnd(false);
    }

    void Unfocus()
    {
        NInput.bypass = false;

        lastInput = inputUI.text;
        //inputUI.DeactivateInputField();

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