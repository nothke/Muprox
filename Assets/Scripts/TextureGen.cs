using UnityEngine;
using System.Collections;

public class TextureGen : MonoBehaviour
{

    Texture2D texture;
    Texture2D normalTexture;
    Texture2D smoothTexture;


    public Material material;

    public Color baseColor;
    public Color noiseColor;
    public Color lineColor;


    void Start()
    {
        Create();

        material.mainTexture = texture;
        material.SetTexture("_BumpMap", normalTexture);
        material.SetTexture("_MetallicGlossMap", smoothTexture);

    }

    int size = 256;

    void Create()
    {
        //int size = 256;

        texture = Init();
        normalTexture = Init();
        smoothTexture = Init();

        // ALBEDO

        TrueNoise(baseColor, noiseColor);
        Grid(texture, lineColor, 10, 10, true);

        texture.filterMode = FilterMode.Point;

        texture.Apply();

        // NORMAL

        Fill(normalTexture, new Color(0.5f, 0.5f, 1));
        Grid(normalTexture, new Color(1, 0.5f, 1), 10, 10, true);

        normalTexture.Apply();

        // SMOOTHNESS

        Color smoothBase = Color.black;
        smoothBase.a = 0.8f;

        Color smoothLine = Color.black;
        smoothBase.a = 0;

        Fill(smoothTexture, smoothBase);
        Grid(smoothTexture, smoothLine, 10, 10, true);

        smoothTexture.Apply();

    }

    Texture2D Init()
    {
        Texture2D texture = new Texture2D(size, size);
        texture.filterMode = FilterMode.Point;

        return texture;
    }

    void Fill(Texture2D tex, Color color)
    {
        Color[] c = new Color[tex.width * tex.height];

        for (int i = 0; i < c.Length; i++)
            c[i] = color;

        tex.SetPixels(c);
    }

    Texture2D Grid(Texture2D tex, Color linesColor, int lineSepX, int lineSepY, bool stagger = false)
    {
        int size = tex.width;

        int linesY = size / lineSepX;
        int linesX = size / lineSepY;

        for (int y = 0; y < linesY; y++)
        {
            for (int x = 0; x < size; x++)
            {
                tex.SetPixel(x, y * lineSepY, linesColor);
            }
        }

        if (!stagger)
        {
            for (int y = 0; y < size; y++)
            {


                for (int x = 0; x < linesX; x++)
                {
                    tex.SetPixel(x * lineSepX, y, linesColor);
                }
            }
        }
        else
        {
            int offset = 0;

            for (int y = 0; y < size; y++)
            {
                offset = 0;
                if ((y / lineSepX) % 2 == 0)
                    offset = lineSepY / 2;

                for (int x = 0; x < linesX; x++)
                {
                    tex.SetPixel(x * lineSepX + offset, y, linesColor);
                }
            }
        }

        return tex;
    }

    void TrueNoise(Color from, Color to)
    {
        int size = texture.width;

        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                texture.SetPixel(x, y, Color.Lerp(from, to, Random.value));

        texture.Apply();
    }

    void Update()
    {
        //TrueNoise(baseColor, noiseColor);
        /*
        texture.SetPixel(Random.Range(0, texture.width), Random.Range(0, texture.height), Color.white);
        texture.SetPixel(Random.Range(0, texture.width), Random.Range(0, texture.height), Color.white);
        texture.SetPixel(Random.Range(0, texture.width), Random.Range(0, texture.height), Color.white);
        texture.SetPixel(Random.Range(0, texture.width), Random.Range(0, texture.height), Color.white);
        texture.SetPixel(Random.Range(0, texture.width), Random.Range(0, texture.height), Color.white);
        texture.SetPixel(Random.Range(0, texture.width), Random.Range(0, texture.height), Color.white);
        texture.Apply();*/
    }
}
