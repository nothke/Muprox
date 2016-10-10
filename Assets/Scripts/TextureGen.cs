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

        //StartCoroutine(AfterFrame());
    }

    IEnumerator AfterFrame()
    {
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        yield return null;
        material.SetTexture("_BumpMap", normalTexture);
    }

    int size = 256;

    void Create()
    {
        //int size = 256;

        texture = Init();
        normalTexture = Init();
        smoothTexture = Init();


        // ALBEDO

        TrueNoise(texture, baseColor, noiseColor);
        Grid(texture, lineColor, 10, 10, true);

        texture.filterMode = FilterMode.Point;

        texture.Apply();

        // NORMAL

        float noiseDisp = 0.05f;
        float d1 = 0.5f - noiseDisp;
        float d2 = 0.5f + noiseDisp;

        //Fill(normalTexture, new Color(0.5f, 0.5f, 0.5f));
        TrueNoise(normalTexture, new Color(d1, d1, 0), new Color(d2, d2, 0));
        Grid(normalTexture, new Color(0.5f, 0.6f, 1), 10, 10, true, true, false);
        Grid(normalTexture, new Color(0.6f, 0.5f, 1), 10, 10, true, false, true);
        ToNormal(normalTexture);

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
        Texture2D texture = new Texture2D(size, size, TextureFormat.ARGB32, true);
        texture.filterMode = FilterMode.Point;

        return texture;
    }

    void ToNormal(Texture2D tex)
    {
        Color[] colours = tex.GetPixels();
        for (int i = 0; i < colours.Length; i++)
        {
            Color c = colours[i];
            c.a = c.r;
            c.r = c.b = c.g;
            //c.r = c.b = 0;
            colours[i] = c;
        }

        tex.SetPixels(colours);
        //tex.Apply();
    }

    void Fill(Texture2D tex, Color color)
    {
        Color[] c = new Color[tex.width * tex.height];

        for (int i = 0; i < c.Length; i++)
            c[i] = color;

        tex.SetPixels(c);
    }

    Texture2D Grid(Texture2D tex, Color linesColor, int lineSepX, int lineSepY, bool stagger = false, bool doVertical = true, bool doHorizontal = true)
    {
        int size = tex.width;

        int linesY = size / lineSepX;
        int linesX = size / lineSepY;

        if (doHorizontal)
            for (int y = 0; y < linesY; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    tex.SetPixel(x, y * lineSepY, linesColor);
                }
            }

        if (doVertical)
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

    void TrueNoise(Texture2D tex, Color from, Color to)
    {
        int size = tex.width;

        for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
                tex.SetPixel(x, y, Color.Lerp(from, to, Random.value));

        tex.Apply();
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
