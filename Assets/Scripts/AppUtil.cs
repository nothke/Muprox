using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AppUtil : MonoBehaviour
{
    public bool canRestart;

    static bool cursorLocked;

    void Start()
    {
        SetCursor(false);
    }

    void Update()
    {
        if (canRestart && Input.GetKeyDown(KeyCode.Tab))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            ToggleCursor();
        }

        /*
        if (cursorLocked && Cursor.lockState != CursorLockMode.Locked)
            SetCursor(false);*/
    }

    void OnApplicationFocus(bool focus)
    {
        SetCursor(!cursorLocked);
    }

    public static void ToggleCursor()
    {
        SetCursor(!Cursor.visible);
    }

    public static void SetCursor(bool enabled)
    {
        cursorLocked = !enabled;

        Cursor.visible = enabled;
        Cursor.lockState = enabled ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
