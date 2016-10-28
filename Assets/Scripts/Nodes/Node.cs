using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid
{
    public Node[,] nodes;

    public void Generate(int xNum, int yNum)
    {
        nodes = new Node[xNum, yNum];

        for (int y = 0; y < yNum; y++)
        {
            for (int x = 0; x < xNum; x++)
            {
                nodes[x, y] = new Node(x, y);
            }
        }
    }

    public void Interconnect()
    {
        foreach (var node in nodes)
        {
            node.AssignSelfNodes(nodes);
        }
    }

    public enum Connection { Diagonal, Orthogonal, Horizontal, Vertical, NW_SE, SW_NE };

    public void Connect(Connection connection, Grid grid)
    {
        switch (connection)
        {
            case Connection.Diagonal:
                for (int y = 0; y < nodes.GetLength(0); y++)
                {
                    for (int x = 0; x < nodes.GetLength(1); x++)
                    {
                        nodes[x, y].SW = grid.nodes[x, y];
                        nodes[x, y].SE = grid.nodes[x + 1, y];
                        nodes[x, y].NW = grid.nodes[x, y + 1];
                        nodes[x, y].NE = grid.nodes[x + 1, y + 1];
                    }
                }

                break;



                break;
            default:
                break;
        }
    }
}

public class Node
{
    public Node N, NE, E, SE, S, SW, W, NW;
    public int x, y;

    public Node() { }

    public Node(int x, int y) { this.x = x; this.y = y; }


    public void SetN(Node n)
    {
        N = n;
        n.S = this;
    }

    public void SetNE(Node n)
    {
        NE = n;
        n.SW = this;
    }

    public void SetE(Node n)
    {
        E = n;
        n.W = this;
    }

    public void SetSE(Node n)
    {
        SE = n;
        n.NW = this;
    }

    public void SetS(Node n)
    {
        S = n;
        n.N = this;
    }

    public void SetSW(Node n)
    {
        SW = n;
        n.NE = this;
    }

    public void SetW(Node n)
    {
        W = n;
        n.E = this;
    }

    public void SetNW(Node n)
    {
        NW = n;
        n.SE = this;
    }


    public void AssignSelfNodes(Node[,] field)
    {
        int xL = field.GetLength(0) - 1;
        int yL = field.GetLength(1) - 1;

        N = y == yL ? null :
            field[x, y + 1];

        NE = x == xL || y == yL ? null :
            field[x + 1, y + 1];

        E = x == xL ? null :
            field[x + 1, y];

        SE = x == xL || y == 0 ? null :
            field[x + 1, y - 1];

        S = y == 0 ? null :
            field[x, y - 1];

        SW = x == 0 || y == 0 ? null :
            field[x - 1, y - 1];

        W = x == 0 ? null :
            field[x - 1, y];

        NW = x == 0 || y == yL ? null :
            field[x - 1, y + 1];
    }

    public override string ToString()
    {
        return "Node(" + x + ", " + y + ")";
    }
}

public class Cell : Node
{

}

public class Edge : Node
{
    bool exists;
}

public class Corner : Node
{
    bool exists;

    public Corner(int x, int y) : base(x, y) { }
}

public class Street : Node
{
    bool exists;

    public Street(int x, int y) : base(x, y) { }
}

public class Plot : Node
{
    public enum Type { Water, Residential, Business, Industrial, Park };
    public Type type;

    public Plot(int x, int y) : base(x, y) { }
}

public class City
{
    public Plot[,] tileNodes;
    public Corner[,] cornerNodes;
    public Street[,] horizontalStreets;
    public Street[,] verticalStreets;

    public City(int xNum, int yNum)
    {
        tileNodes = new Plot[xNum, yNum];

        cornerNodes = new Corner[xNum + 1, yNum + 1];

        horizontalStreets = new Street[xNum, yNum + 1];
        verticalStreets = new Street[xNum + 1, yNum];

        for (int y = 0; y < yNum; y++)
            for (int x = 0; x < xNum; x++)
                tileNodes[x, y] = new Plot(x, y);

        for (int y = 0; y < yNum + 1; y++)
            for (int x = 0; x < xNum + 1; x++)
                cornerNodes[x, y] = new Corner(x, y);

        for (int y = 0; y < yNum + 1; y++)
            for (int x = 0; x < xNum; x++)
                horizontalStreets[x, y] = new Street(x, y);

        for (int y = 0; y < yNum; y++)
            for (int x = 0; x < xNum + 1; x++)
                verticalStreets[x, y] = new Street(x, y);

        for (int y = 0; y < yNum; y++)
        {
            for (int x = 0; x < xNum; x++)
            {
                // diagonals are corners
                tileNodes[x, y].SetSW(cornerNodes[x, y]);
                tileNodes[x, y].SetSW(cornerNodes[x + 1, y + 1]);
                tileNodes[x, y].SetNW(cornerNodes[x, y + 1]);
                tileNodes[x, y].SetSE(cornerNodes[x + 1, y]);

                // vertical streets on the sides
                tileNodes[x, y].SetW(verticalStreets[x, y]);
                tileNodes[x, y].SetE(verticalStreets[x + 1, y]);

                // horizontal streets on the N and S
                tileNodes[x, y].SetS(horizontalStreets[x, y]);
                tileNodes[x, y].SetN(horizontalStreets[x, y + 1]);
            }
        }

        // Street references

        // horizontal
        for (int y = 0; y < yNum + 1; y++)
        {
            for (int x = 0; x < xNum; x++)
            {
                horizontalStreets[x, y].SetW(cornerNodes[x, y]);
                horizontalStreets[x, y].SetE(cornerNodes[x + 1, y]);
            }
        }

        // vertical
        for (int y = 0; y < yNum; y++)
        {
            for (int x = 0; x < xNum + 1; x++)
            {
                verticalStreets[x, y].SetS(cornerNodes[x, y]);
                verticalStreets[x, y].SetN(cornerNodes[x, y + 1]);
            }
        }

        // ^ all corner neighbours should be set
    }
}