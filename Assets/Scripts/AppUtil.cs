using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AppUtil : MonoBehaviour
{
    public bool canRestart;


    void Start()
    {
        ToggleLockMouse();
    }

    void Update()
    {
        if (canRestart && Input.GetKeyDown(KeyCode.Tab))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            ToggleLockMouse();
        }
    }

    void ToggleLockMouse()
    {
        Cursor.visible = !Cursor.visible;
        Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
