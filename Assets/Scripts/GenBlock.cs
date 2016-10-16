using UnityEngine;
using System.Collections;

public class GenBlock : MonoBehaviour
{

    public GameObject blockPrefab;
    public int seed = 0;

    //float separation = 10;
    //int size = 10;

    //float missingChance = 0.1f;

    public int num = 100;
    public float radius = 200;

    public float minRadius = 50;

    void Start()
    {
        Random.InitState(seed);

        ConsoleGlobal.e.console.appendLogLine("Generating blocks..");

        for (int i = 0; i < num; i++)
        {



        RedoPosition:

            Vector3 pos = Random.insideUnitSphere * radius;

            pos.y = 0;

            if (pos.magnitude < minRadius) goto RedoPosition;

            pos.y = Random.Range(-5f, 2f);

            int vBlocks = Random.Range(1, 5);

            for (int v = 0; v < vBlocks; v++)
            {
                Instantiate(blockPrefab, pos + Vector3.up * 10 * v, Quaternion.identity);
            }

        }

        ConsoleGlobal.e.console.appendLogLine("Generated " + num + " blocks");

        /*
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector3 pos = 
                Instantiate(blockPrefab, y)
            }
        }*/
    }
}
