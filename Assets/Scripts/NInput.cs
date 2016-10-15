using UnityEngine;
using System.Collections;

public class NInput
{
    public static bool bypass = false;

    public static bool GetKeyDown(KeyCode keyCode)
    {
        if (bypass) return false;

        return Input.GetKeyDown(keyCode);
    }

    public static bool GetKey(KeyCode keyCode)
    {
        if (bypass) return false;

        return Input.GetKey(keyCode);
    }

    public static bool GetKeyUp(KeyCode keyCode)
    {
        if (bypass) return false;

        return Input.GetKeyUp(keyCode);
    }

    public static float GetAxis(string axisName)
    {
        if (bypass) return 0;

        return Input.GetAxis(axisName);
    }

    public static bool GetMouseButtonDown(int button)
    {
        if (bypass) return false;

        return Input.GetMouseButtonDown(button);
    }

    public static bool GetMouseButtonUp(int button)
    {
        if (bypass) return false;

        return Input.GetMouseButtonUp(button);
    }

    public static bool GetMouseButton(int button)
    {
        if (bypass) return false;

        return Input.GetMouseButton(button);
    }
}
