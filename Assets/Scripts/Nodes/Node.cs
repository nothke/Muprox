using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Not used
public class Grid
{
    public Node[,] nodes;

    public void Generate(int xNum, int yNum)
    {
        nodes = new Node[xNum, yNum];

        for (int y = 0; y < yNum; y++)
            for (int x = 0; x < xNum; x++)
                nodes[x, y] = new Node(x, y);
    }

    public void Interconnect()
    {
        foreach (var node in nodes)
            node.AssignSelfNodes(nodes);
    }
}

public class Node
{
    public Node N, NE, E, SE, S, SW, W, NW;
    public int x, y;

    public Node() { }

    public Node(int x, int y) { this.x = x; this.y = y; }

    public static implicit operator bool(Node node)
    {
        return node != null;
    }

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

    public virtual Vector3 Position()
    {
        return new Vector3(x, 0, y);
    }

    public override string ToString()
    {
        return "Node(" + x + ", " + y + ")";
    }
}

public class Corner : Node
{
    bool exists;

    public Corner(int x, int y) : base(x, y) { }

    public override Vector3 Position()
    {
        return new Vector3(x, 0, y);
    }


    public bool HasNeighbourStreets()
    {
        Street sN = N as Street;
        Street sE = E as Street;
        Street sS = S as Street;
        Street sW = W as Street;

        if (sN && sN.exists) return true;
        if (sE && sE.exists) return true;
        if (sS && sS.exists) return true;
        if (sW && sW.exists) return false;

        return false;
    }
}

public class Street : Node
{
    public bool exists;

    /// <summary>
    /// Is it oriented North-South? If not, it is West-East
    /// </summary>
    public bool isNS;

    public Street(int x, int y) : base(x, y) { }

    public override Vector3 Position()
    {
        if (!isNS)
            // horizontal
            return new Vector3(x + 0.5f, 0, y);
        else
            // vertical
            return new Vector3(x, 0, y + 0.5f);
    }
}

public class Plot : Node
{
    public enum Type { Residential, Business, Industrial, Park, Water };
    public Type type;

    public Plot(int x, int y) : base(x, y) { }

    public override Vector3 Position()
    {
        return new Vector3(x + 0.5f, 0, y + 0.5f);
    }
}

public class City
{
    //
    // 4 intermeshed grids:
    //
    // + - + - + - + - +
    // | P | P | P | P |
    // + - + - + - + - +
    // | P | P | P | P |
    // + - + - + - + - +
    // | P | P | P | P |
    // + - + - + - + - +
    //
    //                      quantity:
    // P: plot              x * y
    // +: corner            (x + 1) * (y + 1)
    // -: horizontal edge   x * (y + 1)
    // |: vertical edge     (x + 1) * y
    //

    public Plot[,] plots;
    public Corner[,] corners;
    public Street[,] horizontalStreets;
    public Street[,] verticalStreets;

    public City(int xNum, int yNum)
    {
        // CREATE

        plots = new Plot[xNum, yNum];
        corners = new Corner[xNum + 1, yNum + 1];
        horizontalStreets = new Street[xNum, yNum + 1];
        verticalStreets = new Street[xNum + 1, yNum];

        for (int y = 0; y < yNum; y++)
            for (int x = 0; x < xNum; x++)
                plots[x, y] = new Plot(x, y);

        for (int y = 0; y < yNum + 1; y++)
            for (int x = 0; x < xNum + 1; x++)
                corners[x, y] = new Corner(x, y);

        for (int y = 0; y < yNum + 1; y++)
            for (int x = 0; x < xNum; x++)
                horizontalStreets[x, y] = new Street(x, y);

        for (int y = 0; y < yNum; y++)
            for (int x = 0; x < xNum + 1; x++)
            {
                verticalStreets[x, y] = new Street(x, y);
                verticalStreets[x, y].isNS = true;
            }

        // RELATIONSHIPS

        // Plot relationships

        for (int y = 0; y < yNum; y++)
        {
            for (int x = 0; x < xNum; x++)
            {
                // diagonals are corners
                plots[x, y].SetSW(corners[x, y]);
                plots[x, y].SetNE(corners[x + 1, y + 1]);
                plots[x, y].SetNW(corners[x, y + 1]);
                plots[x, y].SetSE(corners[x + 1, y]);

                // vertical streets on the sides
                plots[x, y].SetW(verticalStreets[x, y]);
                plots[x, y].SetE(verticalStreets[x + 1, y]);

                // horizontal streets on the N and S
                plots[x, y].SetS(horizontalStreets[x, y]);
                plots[x, y].SetN(horizontalStreets[x, y + 1]);
            }
        }

        // Street-Corner relationships

        // horizontal
        for (int y = 0; y < yNum + 1; y++)
        {
            for (int x = 0; x < xNum; x++)
            {
                horizontalStreets[x, y].SetW(corners[x, y]);
                horizontalStreets[x, y].SetE(corners[x + 1, y]);
            }
        }

        // vertical
        for (int y = 0; y < yNum; y++)
        {
            for (int x = 0; x < xNum + 1; x++)
            {
                verticalStreets[x, y].SetS(corners[x, y]);
                verticalStreets[x, y].SetN(corners[x, y + 1]);
            }
        }

        // ^ all neighbors except horizontal - vertical street are now set, but that one is not really needed
    }
}