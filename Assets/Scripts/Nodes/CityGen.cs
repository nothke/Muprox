using UnityEngine;
using System.Collections;

public class CityGen : MonoBehaviour
{
    City city;


    void Start()
    {
        city = new City(4, 4);


    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.green;

        foreach (var plot in city.tileNodes)
        {
            Gizmos.DrawCube(new Vector3(plot.x, 0, plot.y), Vector3.one * 0.4f);
        }

        Gizmos.color = Color.yellow;
        foreach (var corner in city.cornerNodes)
        {
            Gizmos.DrawWireSphere(new Vector3(-0.5f + corner.x, 0, -0.5f + corner.y), 0.2f);
        }
    }
}
