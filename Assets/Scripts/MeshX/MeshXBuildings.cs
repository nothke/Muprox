using UnityEngine;
using System.Collections;
using MeshXtensions;
using System.Collections.Generic;

public class MeshXBuildings : MonoBehaviour
{
    public static Mesh Stairwell(float stepWidth, float length, float floorHeight, float slabDepth, float separation, float slabThickness = 0.3f)
    {
        List<Mesh> meshes = new List<Mesh>();

        float hH = floorHeight / 2;
        float y = 0;

        float stairLength = length - 2 * slabDepth;

        // STAIRS

        Mesh leftFlight = MeshX.StairsBounds(stepWidth, hH, stairLength, true, 0.18f);
        Mesh rightFlight = MeshX.StairsBounds(stepWidth, hH, stairLength, true, 0.18f);

        leftFlight.Translate(new Vector3(0, y, slabDepth));

        rightFlight.Rotate(180, Vector3.up);
        rightFlight.Translate(new Vector3(stepWidth + separation, y + hH, length - slabDepth));

        // SLABS

        Mesh backSlab = MeshX.Cube(new Vector3(stepWidth * 2 + separation, slabThickness, slabDepth));
        backSlab.Translate(new Vector3((stepWidth + separation) / 2, y + hH - slabThickness / 2, length - (slabDepth / 2)));

        Mesh frontSlab = MeshX.Cube(new Vector3(stepWidth * 2 + separation, slabThickness, slabDepth));
        frontSlab.Translate(new Vector3((stepWidth + separation) / 2, y - slabThickness / 2, slabDepth / 2));

        meshes.Add(leftFlight);
        meshes.Add(rightFlight);
        meshes.Add(backSlab);
        meshes.Add(frontSlab);

        Mesh combined = MeshX.Combine(meshes.ToArray());

        return combined;
    }



}
