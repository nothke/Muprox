using UnityEngine;
using System.Collections;
using MeshXtensions;

public class MeshXN_Example : MonoBehaviour
{
    void Start()
    {
        gameObject.InitializeMesh(MeshXNCube());


    }

    Mesh FirstEverTest()
    {
        Strip s = new Strip(
            -1, 2, -1,
            -1, 2, 1,
            0, 2, -1,
            0, 2, 1,
            1, 2, -1,
            1, 2, 1,
            2, 2, -1,
            2, 2, -1);

        Fan f = new Fan(
            -1, 5, -1,
            -1, 5, 1,
            1, 5, 1,
            2, 5, 1,
            2, 5, -1);

        Vector3 v0a = new Vector3(-1, 0, -1);
        Vector3 v0b = new Vector3(-1, 0, 1);
        Vector3 v1a = new Vector3(0, 0, -1);
        Vector3 v1b = new Vector3(0, 0, 1);
        Vector3 v2a = new Vector3(1, 0, -1);
        Vector3 v2b = new Vector3(1, 0, 1);
        Vector3 v3a = new Vector3(2, 0, -1);
        Vector3 v3b = new Vector3(2, 0, -1);

        Strip s2 = new Strip(v0a, v0b, v1a, v1b, v2a, v2b, v3a, v3b);

        return MeshXN.Combine(s, f, s2);
    }

    Mesh MeshXNCube()
    {
        // 8 vertices   
        Vector3 ldb = new Vector3(-1, -1, -1);
        Vector3 rdb = new Vector3(1, -1, -1);
        Vector3 ldf = new Vector3(-1, -1, 1);
        Vector3 rdf = new Vector3(1, -1, 1);
        Vector3 lub = new Vector3(-1, 1, -1);
        Vector3 rub = new Vector3(1, 1, -1);
        Vector3 luf = new Vector3(-1, 1, 1);
        Vector3 ruf = new Vector3(1, 1, 1);

        // 6 faces
        Strip down = new Strip(ldb, rdb, ldf, rdf);
        Strip up = new Strip(rub, lub, ruf, luf);
        Strip left = new Strip(ldb, ldf, lub, luf);
        Strip right = new Strip(rdf, rdb, ruf, rub);
        Strip back = new Strip(rdb, ldb, rub, lub);
        Strip front = new Strip(ldf, rdf, luf, ruf);

        return MeshXN.Combine(down, up, left, right, back, front);
    }
}