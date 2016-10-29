using UnityEngine;
using System.Collections;
using MeshXtensions;
using System.Collections.Generic;

public class MeshXExample_Office : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Generate();

        gameObject.AddComponent<MeshCollider>();
    }

    void OnValidate()
    {
        if (!Application.isPlaying) return;

        Generate();
    }

    public float xWidth = 5;
    public float zWidth = 5;

    public int xNum = 4;
    public int zNum = 5;

    public int floors = 10;
    public float floorHeight = 3;

    public float slabThickness = 0.3f;

    public float columnWidth = 0.3f;
    public float parapetWidth = 0.3f;
    public float parapetHeight = 1;

    public int stairWellX = 2;
    public int stairWellZ = 1;

    void Generate()
    {
        gameObject.InitializeMesh(null, null);

        List<Mesh> meshes = new List<Mesh>();

        float totalHeight = floorHeight * floors;
        float totalX = xWidth * xNum;
        float totalZ = zWidth * zNum;

        Vector3 start = new Vector3(-xWidth / 2, 0, -zWidth / 2);
        Vector3 mid = new Vector3((-xWidth / 2) + xWidth * xNum / 2, 0, (-zWidth / 2) + zWidth * zNum / 2);

        // Columns
        for (int x = -1; x < xNum; x++)
        {
            for (int z = -1; z < zNum; z++)
            {
                Mesh column = MeshX.Cube(new Vector3(columnWidth, totalHeight - floorHeight, columnWidth));
                column.Translate(-start + new Vector3(x * xWidth + xWidth / 2, (totalHeight - floorHeight) / 2, z * zWidth + zWidth / 2));

                meshes.Add(column);
            }
        }



        for (int f = 0; f < floors; f++)
        {
            float y = f * floorHeight;

            // Slab
            for (int x = 0; x < xNum; x++)
            {

                for (int z = 0; z < zNum; z++)
                {
                    if (x == stairWellX && z == stairWellZ) continue;

                    Mesh slab = MeshX.Cube(new Vector3(xWidth, slabThickness, zWidth));
                    slab.Translate(-start + new Vector3(x * xWidth, y - slabThickness * 0.5f, z * zWidth));

                    meshes.Add(slab);
                }
            }

            // Parapet

            if (f > 0)
            {
                Mesh parapetUp = MeshX.Cube(new Vector3(totalX, parapetHeight, parapetWidth));
                Mesh parapetDown = MeshX.Cube(new Vector3(totalX, parapetHeight, parapetWidth));
                Mesh parapetLeft = MeshX.Cube(new Vector3(parapetWidth, parapetHeight, totalZ));
                Mesh parapetRight = MeshX.Cube(new Vector3(parapetWidth, parapetHeight, totalZ));

                parapetUp.Translate(-start + mid + new Vector3(0, y, totalZ / 2));
                parapetDown.Translate(-start + mid + new Vector3(0, y, -totalZ / 2));
                parapetLeft.Translate(-start + mid + new Vector3(-totalX / 2, y, 0));
                parapetRight.Translate(-start + mid + new Vector3(+totalX / 2, y, 0));

                meshes.Add(parapetUp);
                meshes.Add(parapetDown);
                meshes.Add(parapetLeft);
                meshes.Add(parapetRight);
            }

            // Stairs

            if (f == floors - 1) continue;

            float stairSeparation = xWidth - 1.2f * 2;

            Mesh stairs = MeshXBuildings.Stairwell(1.2f, zWidth, floorHeight, 1.2f, stairSeparation, slabThickness);
            stairs.Translate(new Vector3(stairWellX * xWidth + 0.6f, y, stairWellZ * zWidth));

            meshes.Add(stairs);
        }

        gameObject.SetMesh(MeshX.Combine(meshes.ToArray()));

    }

}
