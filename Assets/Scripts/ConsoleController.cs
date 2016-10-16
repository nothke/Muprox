/// <summary>
/// Handles parsing and execution of console commands, as well as collecting log output.
/// Copyright (c) 2014-2015 Eliot Lash
/// </summary>
using UnityEngine;
using UnityEngine.Networking;

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

public delegate void CommandHandler(string[] args);

public class ConsoleController
{

    #region Event declarations
    // Used to communicate with ConsoleView
    public delegate void LogChangedHandler(string[] log);
    public event LogChangedHandler logChanged;

    public delegate void VisibilityChangedHandler(bool visible);
    public event VisibilityChangedHandler visibilityChanged;
    #endregion

    /// <summary>
    /// Object to hold information about each command
    /// </summary>
    class CommandRegistration
    {
        public string command { get; private set; }
        public CommandHandler handler { get; private set; }
        public string help { get; private set; }

        public CommandRegistration(string command, CommandHandler handler, string help)
        {
            this.command = command;
            this.handler = handler;
            this.help = help;
        }
    }

    /// <summary>
    /// How many log lines should be retained?
    /// Note that strings submitted to appendLogLine with embedded newlines will be counted as a single line.
    /// </summary>
    const int scrollbackSize = 500;

    Queue<string> scrollback = new Queue<string>(scrollbackSize);
    List<string> commandHistory = new List<string>();
    Dictionary<string, CommandRegistration> commands = new Dictionary<string, CommandRegistration>();

    public string[] log { get; private set; } //Copy of scrollback as an array for easier use by ConsoleView

    const string repeatCmdName = "\\!"; //Name of the repeat command, constant since it needs to skip these if they are in the command history

    public ConsoleController()
    {
        //When adding commands, you must add a call below to registerCommand() with its name, implementation method, and help text.
        registerCommand("\\babble", babble, "Example command that demonstrates how to parse arguments. babble [word] [# of times to repeat]");
        registerCommand("\\echo", echo, "echoes arguments back as array (for testing argument parser)");
        registerCommand("\\help", help, "Print this help.");
        //registerCommand("\\hide", hide, "Hide the console.");
        registerCommand(repeatCmdName, repeatCommand, "Repeat last command.");
        //registerCommand("reload", reload, "Reload game.");
        //registerCommand("\\resetprefs", resetPrefs, "Reset & saves PlayerPrefs.");

        registerCommand("\\controls", controls, "List controls.");

        registerCommand("\\host", host, "Start hosting LAN server.");
        registerCommand("\\join", join, "Join a LAN server. join [full ip] or [short ip] - only last number: 192.168.0.[short ip]");
        registerCommand("\\serveronly", serveronly, "Starts server without joining");
        registerCommand("\\mm", mm, "Starts matchmaker");
        registerCommand("\\dc", dc, "Disconnect client");
        registerCommand("\\ds", ds, "Disconnect server");
        registerCommand("\\ip", ip, "Your IP");
        registerCommand("\\serverip", serverip, "Server IP");

        registerCommand("\\nick", nick, "Set player nick");
        registerCommand("\\kill", kill, "Commit suicide"); // Kills player or commits suicide. \\kill [playerName] - if none, will kill client
        registerCommand("\\goto", moveto, "Moves player to position. \\goto [x] [y] [z]");
        registerCommand("\\spawn", spawn, "Spawns an entity in front of player. \\spawn [entityName]");
        registerCommand("\\listentities", listentities, "Lists all spawnable entities.");

        registerCommand("\\networkgui", networkgui, "Toggles legacy network GUI.");

        registerCommand("\\quit", quit, "Quits application");
    }

    void registerCommand(string command, CommandHandler handler, string help)
    {
        commands.Add(command, new CommandRegistration(command, handler, help));
    }

    const string errorColor = "#a52a2aff";
    const string warningColor = "#550055ff";

    public enum lineColor { Error, Warning };

    public static string Colorize(string line, lineColor color)
    {
        string hex = "#00000000";

        switch (color)
        {
            case lineColor.Error: hex = errorColor; break;
            case lineColor.Warning: hex = warningColor; break;
        }

        return Colorize(line, hex);
    }

    public static string Colorize(string line, string hex)
    {
        return ("<color=" + hex + ">" + line + "</color>");
    }

    public static string Colorize(string line, Color color)
    {
        return Colorize(line, RGBToHex(color));
    }

    static string RGBToHex(Color color)
    {
        float red = color.r * 255;
        float green = color.g * 255;
        float blue = color.b * 255;

        char a, b, c, d, e, f;

        a = GetHex(Mathf.FloorToInt(red / 16));
        b = GetHex(Mathf.RoundToInt(red % 16));
        c = GetHex(Mathf.FloorToInt(green / 16));
        d = GetHex(Mathf.RoundToInt(green % 16));
        e = GetHex(Mathf.FloorToInt(blue / 16));
        f = GetHex(Mathf.RoundToInt(blue % 16));

        string hex = "" + a + b + c + d + e + f;
        Debug.Log(hex);

        return "#" + a + b + c + d + e + f;
    }

    static char GetHex(int dec)
    {
        dec = Mathf.Clamp(dec, 0, 15);

        var alpha = "0123456789ABCDEF";
        return alpha[dec];
    }

    public void appendLogLine(string line, lineColor color)
    {
        string hex = "#00000000";

        switch (color)
        {
            case lineColor.Error: hex = errorColor; break;
            case lineColor.Warning: hex = warningColor; break;
        }

        appendLogLine("<color=" + hex + ">" + line + "</color>");
    }

    public void appendNetworkLogLine(string line)
    {
        if (NetworkInactive()) appendLogLine(line);
        else
            PlayerController.client.GetComponent<ChatRelay>().CmdSendChat(line);
    }

    public void appendLogLine(string line)
    {
        Debug.Log(line);

        if (scrollback.Count >= ConsoleController.scrollbackSize)
        {
            scrollback.Dequeue();
        }
        scrollback.Enqueue(line);

        log = scrollback.ToArray();
        if (logChanged != null)
        {
            logChanged(log);
        }
    }

    public void runCommandString(string commandString)
    {
        appendLogLine("$ " + commandString);

        string[] commandSplit = parseArguments(commandString);
        string[] args = new string[0];
        if (commandSplit.Length < 1)
        {
            appendLogLine(string.Format("<color=#a52a2aff>Unable to process command '{0}'</color>", commandString));
            return;

        }
        else if (commandSplit.Length >= 2)
        {
            int numArgs = commandSplit.Length - 1;
            args = new string[numArgs];
            Array.Copy(commandSplit, 1, args, 0, numArgs);
        }
        runCommand(commandSplit[0].ToLower(), args);
        commandHistory.Add(commandString);
    }

    public void runCommand(string command, string[] args)
    {
        CommandRegistration reg = null;
        if (!commands.TryGetValue(command, out reg))
        {
            appendLogLine(string.Format("<color=#a52a2aff>Unknown command '{0}', type '\\help' for list.</color>", command));
        }
        else
        {
            if (reg.handler == null)
            {
                appendLogLine(string.Format("<color=#a52a2aff>Unable to process command '{0}', handler was null.</color>", command));
            }
            else
            {
                reg.handler(args);
            }
        }
    }

    static string[] parseArguments(string commandString)
    {
        LinkedList<char> parmChars = new LinkedList<char>(commandString.ToCharArray());
        bool inQuote = false;
        var node = parmChars.First;
        while (node != null)
        {
            var next = node.Next;
            if (node.Value == '"')
            {
                inQuote = !inQuote;
                parmChars.Remove(node);
            }
            if (!inQuote && node.Value == ' ')
            {
                node.Value = '\n';
            }
            node = next;
        }
        char[] parmCharsArr = new char[parmChars.Count];
        parmChars.CopyTo(parmCharsArr, 0);
        return (new string(parmCharsArr)).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
    }

    #region Command handlers
    //Implement new commands in this region of the file.

    /// <summary>
    /// A test command to demonstrate argument checking/parsing.
    /// Will repeat the given word a specified number of times.
    /// </summary>
    void babble(string[] args)
    {
        if (args.Length < 2)
        {
            appendLogLine("Expected 2 arguments.");
            return;
        }
        string text = args[0];
        if (string.IsNullOrEmpty(text))
        {
            appendLogLine("Expected arg1 to be text.");
        }
        else
        {
            int repeat = 0;
            if (!Int32.TryParse(args[1], out repeat))
            {
                appendLogLine("Expected an integer for arg2.");
            }
            else
            {
                for (int i = 0; i < repeat; ++i)
                {
                    appendLogLine(string.Format("{0} {1}", text, i));
                }
            }
        }
    }

    void echo(string[] args)
    {
        StringBuilder sb = new StringBuilder();
        foreach (string arg in args)
        {
            sb.AppendFormat("{0},", arg);
        }
        sb.Remove(sb.Length - 1, 1);
        appendLogLine(sb.ToString());
    }

    void help(string[] args)
    {
        foreach (CommandRegistration reg in commands.Values)
        {
            appendLogLine(string.Format("{0}: {1}", Colorize(reg.command, lineColor.Warning), reg.help));
        }
    }

    void hide(string[] args)
    {
        if (visibilityChanged != null)
        {
            visibilityChanged(false);
        }
    }

    void repeatCommand(string[] args)
    {
        for (int cmdIdx = commandHistory.Count - 1; cmdIdx >= 0; --cmdIdx)
        {
            string cmd = commandHistory[cmdIdx];
            if (String.Equals(repeatCmdName, cmd))
            {
                continue;
            }
            runCommandString(cmd);
            break;
        }
    }

    public string GetPrevCommand()
    {
        if (commandHistory == null || commandHistory.Count == 0)
            return "";

        return commandHistory[commandHistory.Count - 1];
    }

    /*
    void reload(string[] args)
    {
        Application.LoadLevel(Application.loadedLevel);
    }*/

    void resetPrefs(string[] args)
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    #endregion

    //// MY HANDLERS!!!

    // CONTROLS
    void controls(string[] args)
    {
        appendLogLine("---------------");
        appendLogLine("WASD - walk around");
        appendLogLine("Space - jump");
        appendLogLine("mouse buttons - interact");
        appendLogLine("q - drop item");
        appendLogLine("\\ or t - open console");
        appendLogLine("Esc - close console");
        appendLogLine("Left ctrl - lock/unlock mouse");
    }


    // SERVER STUFF

    void host(string[] args)
    {
        if (NetworkActive()) return;

        NetworkManager.singleton.StartHost();

        appendLogLine("..Starting host at " + NetworkManager.singleton.networkAddress);
    }

    void join(string[] args)
    {
        if (NetworkActive()) return;

        if (args != null && args.Length > 0)
        {
            if (args[0].Contains("."))
                NetworkManager.singleton.networkAddress = args[0];
            else
                NetworkManager.singleton.networkAddress = "192.168.0." + args[0];
        }

        NetworkManager.singleton.StartClient();

        //TillConnected();
    }

    void serveronly(string[] args)
    {
        if (NetworkActive()) return;

        NetworkManager.singleton.StartServer();
    }

    void mm(string[] args)
    {
        NetworkManager.singleton.StartMatchMaker();
    }

    void dc(string[] args)
    {
        if (NetworkInactive()) return;

        NetworkManager.singleton.StopClient();
    }

    void ds(string[] args)
    {
        if (NetworkInactive()) return;

        NetworkManager.singleton.StopHost();
        //NetworkManager.singleton.StopServer();
    }



    void ip(string[] args)
    {
        appendLogLine(Network.player.ipAddress);
    }

    void serverip(string[] args)
    {
        if (NetworkInactive()) return;

        appendLogLine(NetworkManager.singleton.client.serverIp);
    }


    void nick(string[] args)
    {
        if (NetworkInactive()) return;

        if (No(args)) return;

        appendNetworkLogLine(PlayerController.client.nick + " changed nick to " + args[0]);

        PlayerController.client.nick = args[0];
    }

    void kill(string[] args)
    {
        //NetworkManager.singleton.

        if (NetworkInactive()) return;

        PlayerController.client.GetComponent<Health>().TakeDamage(1000);

    }

    void moveto(string[] args)
    {
        if (NetworkInactive()) return;

        if (args == null || args.Length == 0)
        {
            appendLogLine("No argument submitted");
            return;
        }

        Vector3 pos = ParseV3(args);

        PlayerController.client.transform.position = pos;
    }

    void spawn(string[] args)
    {
        if (NetworkInactive()) return;

        if (No(args))
            return;

        foreach (var entity in NetworkManager.singleton.spawnPrefabs)
        {
            Vector3 position = PlayerController.client.transform.position + PlayerController.client.transform.forward * 2;

            if (entity.name == args[0])
            {
                GameObject go = GameObject.Instantiate(entity, position, Quaternion.identity) as GameObject;
                NetworkServer.Spawn(go);

                return;
            }
        }

        appendLogLine("Entity " + args[0] + " doesn't exist. Type \\listentities to see what exists");
    }

    bool No(string[] args)
    {

        if (args == null || args.Length == 0)
        {
            appendLogLine("No argument submitted");
            return true;
        }

        return false;
    }

    void listentities(string[] args)
    {
        foreach (var entity in NetworkManager.singleton.spawnPrefabs)
        {
            appendLogLine(entity.name);
        }
    }

    void quit(string[] args)
    {
        Application.Quit();
    }

    void networkgui(string[] args)
    {
        NetworkManager.singleton.GetComponent<NetworkManagerHUD>().enabled =
            !NetworkManager.singleton.GetComponent<NetworkManagerHUD>().enabled;
    }








    // UTILS

    Vector3 ParseV3(string[] args)
    {
        if (args == null || args.Length == 0)
        {
            return Vector3.zero;
        }

        float x, y, z;

        if (!float.TryParse(args[0], out x))
            appendLogLine("Parsing failed, used 0", lineColor.Error);

        if (args.Length < 2) y = 0;
        else if (!float.TryParse(args[1], out y))
            appendLogLine("Parsing failed, used 0", lineColor.Error);

        if (args.Length < 3) z = 0;
        else if (!float.TryParse(args[2], out z))
            appendLogLine("Parsing failed, used 0", lineColor.Error);

        return new Vector3(x, y, z);
    }

    bool NetworkInactive()
    {
        if (!NetworkManager.singleton.isNetworkActive)
        {
            appendLogLine("Network is inactive", lineColor.Error);
            return true;
        }

        return false;

    }

    bool NetworkActive()
    {
        if (NetworkManager.singleton.isNetworkActive)
        {
            appendLogLine("Network is already active, disconnect first", lineColor.Error);
            return true;
        }

        return false;
    }
}