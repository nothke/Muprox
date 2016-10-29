using UnityEngine;
using System.Collections;

public class Shape : MonoBehaviour
{
    public Color lineColor = Color.green;

    public bool drawAxes = true;

    /*
    [InspectorButton("Mirror")]
    public bool mirror;

    [InspectorButton("Reverse")]
    public bool reverse;
    */

    void OnDrawGizmos()
    {
        if (drawAxes)
        {
            Gizmos.color = Color.green * 0.5f;
            Gizmos.DrawLine(transform.position + transform.up * 4, transform.position - transform.up * 4);

            Gizmos.color = Color.red * 0.5f;
            Gizmos.DrawLine(transform.position + transform.right * 4, transform.position - transform.right * 4);
        }

        int count = transform.childCount;

        if (count < 2) return;

        for (int i = 1; i < count; i++)
        {
            Gizmos.color = lineColor;
            Gizmos.DrawLine(transform.GetChild(i - 1).position, transform.GetChild(i).position);
        }
    }

    public Vector2[] GetPoints2D(float scale = 1)
    {
        Vector3[] points = GetPoints(scale);

        Vector2[] points2D = new Vector2[points.Length];

        for (int i = 0; i < points.Length; i++)
        {
            points2D[i] = points[i];
        }

        return points2D;
    }

    public Vector2[] GetPoints2DMirrored(float scale = 1)
    {
        Vector2[] points = GetPoints2D(scale);
        Vector2[] pointsMirrored = new Vector2[points.Length];


        for (int i = 0; i < points.Length; i++)
        {
            points[i].x = -points[i].x;
            pointsMirrored[points.Length - 1 - i] = points[i];
        }

        return pointsMirrored;
    }

    public Vector3[] GetPoints(float scale = 1)
    {
        if (transform.childCount == 0) return null;

        Vector3[] points = new Vector3[transform.childCount];

        for (int i = 0; i < points.Length; i++)
        {
            points[i] = transform.GetChild(i).localPosition * scale;
        }

        return points;
    }

    [ContextMenu("Mirror")]
    public void Mirror()
    {
        if (transform.childCount == 0) return;

        int childCount = transform.childCount;

        Transform[] newTs = new Transform[childCount];

        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);

            GameObject go = new GameObject("m_" + child.name);
            go.transform.parent = transform;
            go.transform.localPosition = new Vector3(-child.localPosition.x, child.localPosition.y, child.localPosition.z);

            newTs[i] = go.transform;
        }
    }

    [ContextMenu("Reverse")]
    public void Reverse()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(0);

            child.SetSiblingIndex(transform.childCount - (i + 1));
        }
    }
}