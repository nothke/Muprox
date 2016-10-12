using UnityEngine;
using System.Collections;
using UnityEngine.Networking;



public class NetManager : NetworkManager
{

    public GameObject sceneCamera;

    public override void OnStopHost()
    {
        ShowSceneCamera(true);

        base.OnStopHost();
    }

    public override void OnStopClient()
    {
        ShowSceneCamera(true);

        base.OnStopClient();
    }

    public override void OnStartHost()
    {
        ShowSceneCamera(false);

        base.OnStartHost();
    }

    public override void OnStartClient(NetworkClient client)
    {
        ShowSceneCamera(false);

        base.OnStartClient(client);
    }

    public void ShowSceneCamera(bool enable)
    {
        if (sceneCamera)
            sceneCamera.SetActive(enable);
    }
}
