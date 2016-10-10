using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldGenerator : MonoBehaviour
{

    float gridUnit = 1;

    float minGridSize = 10;
    float maxGridSize = 30;

    [System.Serializable]
    public class PrefabSettings
    {
        public GameObject wall_corner;
        public GameObject wall_wall;
        public GameObject wall_window;
        public GameObject wall_door;
        public GameObject floor;
    }

    public PrefabSettings prefabs;

    public class Tile
    {
        public bool borderUp;
        public bool borderLeft;

        public int up;
        public int down;
        public int left;
        public int right;

        public int gridPosX;
        public int gridPosY;
    }

    public List<Tile> tiles = new List<Tile>();

    IEnumerator InstantiateTiles()
    {
        foreach (var tile in tiles)
        {
            InstantiateTile(tile);
            yield return null;
        }
    }

    void InstantiateTile(Tile tile)
    {
        /*
        float angle = 0;

        if (tile.down + tile.down + tile.left + tile.right > 2)
        {
            if (tile.left > 0 && tile.down > 0)
                angle = 90;

            if (tile.down > 0 && tile.right > 0)

        }*/

        float half = gridUnit / 2;
        Vector3 position = new Vector3(tile.gridPosX * gridUnit, 0, tile.gridPosY * gridUnit);

        GameObject floor = Instantiate(prefabs.floor, position, Quaternion.identity) as GameObject;

        /*
        if (tile.borderUp && tile.up > 0)
        {
            GameObject go = Instantiate(prefabs.wall_wall, position + Vector3.up * gridUnit, Quaternion.identity) as GameObject;


        }*/



        if (tile.borderUp && tile.up > 0)
        {
            Instantiate(prefabs.wall_wall, position - Vector3.forward * half, Quaternion.LookRotation(Vector3.forward));
        }

        if (tile.borderLeft && tile.left > 0)
        {
            Instantiate(prefabs.wall_wall, position - Vector3.right * half, Quaternion.LookRotation(Vector3.right));
        }

        if (tile.down > 0)
        {
            Instantiate(prefabs.wall_wall, position + Vector3.forward * half, Quaternion.LookRotation(Vector3.forward));
        }

        if (tile.right > 0)
        {
            Instantiate(prefabs.wall_wall, position + Vector3.right * half, Quaternion.LookRotation(Vector3.right));
        }

    }

    void GenerateBuilding()
    {


        // block
    }

    void GenerateRoom()
    {
        int w = Random.Range(5, 10);
        int h = Random.Range(5, 10);

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                Tile tile = new Tile();

                if (y == 0) { tile.up = 1; tile.borderUp = true; }
                if (y == h - 1) tile.down = 1;

                if (x == 0) { tile.left = 1; tile.borderLeft = true; }
                if (x == w - 1) tile.right = 1;

                tile.gridPosX = x;
                tile.gridPosY = y;

                tiles.Add(tile);
            }
        }
    }

    void Start()
    {
        GenerateRoom();
        StartCoroutine(InstantiateTiles());
    }



    void Update()
    {

    }
}
