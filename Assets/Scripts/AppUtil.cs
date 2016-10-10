using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AppUtil : MonoBehaviour
{

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
