using UnityEngine;
using System.Collections;
using MeshXtensions;
using System.Collections.Generic;

public class RandomPoints : MonoBehaviour
{

    public int numOfPoints = 1000;

    Bounds bounds;

    GameObject go;

    public Material grassMat;

    void Start()
    {

        Mesh mesh = new Mesh();

        bounds = GetComponent<Renderer>().bounds;

        List<Vector3> v3s = new List<Vector3>();

        for (int i = 0; i < numOfPoints; i++)
        {
            Vector3 v1 = RandomPointsOnBoundsTop(bounds);

            RaycastHit hit;
            Physics.Raycast(v1, Vector3.down, out hit, Mathf.Infinity);

            if (hit.point == null) continue;

            v3s.Add(hit.point);
        }

        mesh.SetVertices(v3s);

        List<int> tris = new List<int>();

        for (int i = 0; i < v3s.Count; i++)
        {
            tris.Add(i);
            tris.Add(i);
            tris.Add(i);
        }

        mesh.SetTriangles(tris, 0);

        go = MeshX.InitializeSeparateMesh(gameObject, mesh, grassMat);

    }

    Vector3 RandomPointsOnBoundsTop(Bounds b)
    {
        return new Vector3(Random.Range(b.min.x, b.max.x), b.max.y, Random.Range(b.min.z, b.max.z));
    }

    void OnDrawGizmos()
    {
        bounds = GetComponent<Renderer>().bounds;
        Gizmos.DrawSphere(bounds.min, 1);
        Gizmos.DrawSphere(bounds.max, 1);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
