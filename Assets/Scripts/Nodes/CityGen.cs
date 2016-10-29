using UnityEngine;
using System.Collections;
using MeshXtensions;

public class CityGen : MonoBehaviour
{
    City city;

    public int tiles = 4;

    public float tileSize = 100;

    public float streetWidth = 10;

    void Start()
    {
        city = new City(tiles, tiles);

        Generate();
    }

    public Material plotMat;
    public Material cornerMat;
    public Material streetMat;

    public Shape streetShape;
    public Shape streetWaterShape;

    public GameObject buildingPrefab;

    void Generate()
    {
        // create meshes
        foreach (var plot in city.plots)
        {
            if (Random.value < 0.3f) plot.type = Plot.Type.Water;

            if (plot.type == Plot.Type.Water) continue;

            Mesh quad = MeshX.Quad(Vector3.zero, tileSize - streetWidth, tileSize - streetWidth, Vector3.up);
            GameObject plotGO = gameObject.InitializeSeparateMesh(quad, plotMat);

            plotGO.transform.position = plot.Position() * tileSize;

            if (buildingPrefab)
            {
                GameObject officeGO = Instantiate(buildingPrefab, plot.Position() * tileSize, Quaternion.identity) as GameObject;
                MeshXExample_Office office = officeGO.GetComponent<MeshXExample_Office>();

                office.floors = Random.Range(3, 10);
            }
        }

        DoStreets(city.horizontalStreets);
        DoStreets(city.verticalStreets);

        foreach (var corner in city.corners)
        {
            if (!corner.HasNeighbourStreets()) continue;

            Mesh quad = MeshX.Quad(Vector3.zero, streetWidth, streetWidth, Vector3.up);
            GameObject cornerGO = gameObject.InitializeSeparateMesh(quad, cornerMat);

            cornerGO.transform.position = corner.Position() * tileSize;
        }

        float dist = (tileSize - streetWidth) / 2;
    }

    void DoStreets(Street[,] streets)
    {
        float dist = (tileSize - streetWidth) / 2;

        foreach (var street in streets)
        {
            street.exists = Random.value > 0.2f;

            Plot leftPlot = street.isNS ? street.E as Plot : street.S as Plot;
            Plot rightPlot = street.isNS ? street.W as Plot : street.N as Plot;

            bool surrounded = false;

            if ((leftPlot && leftPlot.type != Plot.Type.Water) ||
                (rightPlot && rightPlot.type != Plot.Type.Water))
                surrounded = true;

            if (!surrounded) continue;

            Mesh mesh = null;

            Material mat;

            if (!street.exists)
            {
                float width = street.isNS ? streetWidth : tileSize - streetWidth;
                float height = street.isNS ? tileSize - streetWidth : streetWidth;

                mesh = MeshX.Quad(Vector3.zero, width, height, Vector3.up);
                mat = plotMat;
            }
            else
            {
                Vector3 direction = street.isNS ? Vector3.forward : Vector3.right;
                Vector3[] points = new Vector3[] { direction * -dist, direction * dist };


                Shape leftShape = GetShape(leftPlot);
                Shape rightShape = GetShape(rightPlot);

                Mesh streetMeshR = MeshX.Sweep(leftShape.GetPoints2D(), points);
                Mesh streetMeshL = MeshX.Sweep(rightShape.GetPoints2DMirrored(), points);

                mesh = MeshX.Combine(new Mesh[] { streetMeshL, streetMeshR });
                mat = streetMat;
            }

            GameObject streetGO = gameObject.InitializeSeparateMesh(mesh, mat);

            streetGO.transform.position = street.Position() * tileSize;
        }
    }

    Shape GetShape(Plot plot)
    {
        if (plot == null)
            return streetWaterShape;

        switch (plot.type)
        {
            case Plot.Type.Water:
                return streetWaterShape;
            default:
                return streetShape;
        }
    }

    /*
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.green;

        foreach (var plot in city.plots)
        {
            Gizmos.DrawCube(plot.Position(), Vector3.one * 0.4f);
        }

        Gizmos.color = Color.yellow;

        foreach (var corner in city.corners)
        {
            Vector3 pos = corner.Position();

            Gizmos.DrawWireSphere(pos, 0.2f);

            Gizmos.color = Color.red;

            if (corner.NE)
                Gizmos.DrawLine(pos, corner.NE.Position());
            if (corner.NW)
                Gizmos.DrawLine(pos, corner.NW.Position());
            if (corner.SW)
                Gizmos.DrawLine(pos, corner.SW.Position());
            if (corner.SE)
                Gizmos.DrawLine(pos, corner.SE.Position());

            Gizmos.color = Color.blue;

            if (corner.N)
                Gizmos.DrawLine(pos, corner.N.Position());
            if (corner.E)
                Gizmos.DrawLine(pos, corner.E.Position());
            if (corner.S)
                Gizmos.DrawLine(pos, corner.S.Position());
            if (corner.W)
                Gizmos.DrawLine(pos, corner.W.Position());
        }

        // relation lines

    }
    */
}
