using UnityEngine;
using System.Collections;

public class Node_Test : MonoBehaviour
{

    void Start()
    {

        Grid field = new Grid();

        field.Generate(4, 4);

        Debug.Log("Node[0,2].W should be null: " + field.nodes[0, 2].W);
        Debug.Log("Node[0,2].E should be [1,2]: " + field.nodes[0, 2].E);
        Debug.Log("Node[3,3].NE should be null: " + field.nodes[3, 3].NE);
        Debug.Log("Node[3,2].S should be [3,1]: " + field.nodes[3, 2].S);
        Debug.Log("Node[2,2].NW should be [1,3]: " + field.nodes[2, 2].NW);
    }

}
