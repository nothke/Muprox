using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AppUtil : MonoBehaviour
{

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Cursor.visible = !Cursor.visible;
            Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}
