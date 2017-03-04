using UnityEngine;
using System.Collections;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Level : MonoBehaviour
{

    [Header("Use this texture as map")]
    public Texture2D map;
    public Color bgColor = new Color(0, 0.101f, 0.02f, 1);

    [Header("If there is none, generate it")]
    public int width = 16;
    public static float[] lightmap;
    public bool displayLightmap = true;
    [Header("Beeinflusst Inselgrößen")]
    [Range(0, 255)]
    public int lightSteps = 10;
    [Range(0, 100)]
    public int probabilityLightPoint = 5;
    public int smoothDurchläufe = 5;

    [Range(0, 1)]
    public float minLight = 0;
    [Range(0, 1)]
    public float maxLight = 1;
    [Range(0, 1)]
    public float offset = 0;

    public static bool lightBoost = false;

    public KeyCode boostKey = KeyCode.L;
    public Material lightdisplayMat;
    public GameObject levelLightPlane;
    [Range(-1, 1)]
    public float displayOffset = 0;

    // Use this for initialization
    void Start()
    {
        CreateLightMap();
    }

    void Awake()
    {
        GameObject mainCam = GameObject.Find("Main Camera");
        if (mainCam.GetComponent<ScreenSpaceDistortionEffect>() == null)
        {
            mainCam.AddComponent<ScreenSpaceDistortionEffect>();
        }
    }

    public void CreateLightMap()
    {
        if (map == null)
        {
            GenerateLightMap();
        }
        else
        {
            LoadLightmapFromTexture();
        }
        /// DisplayLightmap
        if (displayLightmap)
        {
            DisplayLightmap();
        }
    }

    private void LoadLightmapFromTexture()
    {
        // Load map from Texture;
        if (map.width != map.height)
            throw new System.Exception("Map is not Cubic! Please fix it!");

        width = map.width;

        lightmap = new float[width * width];

        for (int i = 0; i < lightmap.Length; i++)
        {
            lightmap[i] = map.GetPixel(i % width, i / width).g;
        }
    }

    void Update()
    {
        lightBoost = Input.GetKey(boostKey);
    }

    public static float GetLightAtPosition(Vector3 pos)
    {
        int width = (int)Mathf.Sqrt(lightmap.Length);

        // the lightmap has its center at 0,0 => offset it by half the width;
        int x = (int)pos.x + width / 2;
        int y = (int)pos.y + width / 2;

        if (x < 0 || y < 0)
            return 0;

        if (x < width && y < width)
        {
            return lightmap[x + y * width] * (lightBoost ? 10f : 1f);
        }

        return 0;
    }

    private void GenerateLightMap()
    {
        lightmap = new float[width * width];

        for (int i = 0; i < lightmap.Length; i++)
        {
            if (UnityEngine.Random.Range(0f, 100f) < probabilityLightPoint)
                lightmap[i] = lightSteps;
            else
                lightmap[i] = 0;
        }
        for (int i = 0; i < smoothDurchläufe; i++)
        {
            SmoothLightmap();
        }

        ContrastCorrection();
        ApplyOffset();
        
    }

    private void ApplyOffset()
    {
        float[] tmpArr = new float[lightmap.Length];
        for (int i = 0; i < lightmap.Length; i++)
        {
            tmpArr[i] =Mathf.Clamp( lightmap[i]-offset,0,999);

        }
        lightmap = tmpArr;
    }
    void SmoothLightmap()
    {
        float[] tmpArr = new float[lightmap.Length];
        for (int i = 0; i < lightmap.Length; i++)
        {
            int count = 0;
            float summ = 0;

            summ += lightmap[i];
            count++;

            //pixel drüber
            if (i > width)
            {
                summ += lightmap[i - width];
                count++;
            }

            //pixel drunter
            if (i < lightmap.Length - width)
            {
                summ += lightmap[i + width];
                count++;
            }

            // Pixel rechts
            if (i % width > 0)
            {
                summ += lightmap[i - 1];
                count++;
            }

            // Pixel links
            if (i % width < width - 1)
            {
                summ += lightmap[i + 1];
                count++;
            }

            tmpArr[i] = summ / count;
        }

        lightmap = tmpArr;
    }

    /// <summary>
    /// Make sure the darkest is black and the brightest is pure white
    /// </summary>
    void ContrastCorrection()
    {
        float min = Mathf.Min(lightmap);


        for (int i = 0; i < lightmap.Length; i++)
        {
            lightmap[i] = (lightmap[i] - min);
        }

        float max = Mathf.Max(lightmap);
        for (int i = 0; i < lightmap.Length; i++)
        {
            lightmap[i] = (lightmap[i] / max);
        }

        //clamp:
        for (int i = 0; i < lightmap.Length; i++)
        {
            lightmap[i] = Mathf.Clamp(lightmap[i], minLight, maxLight);
        }


        Debug.Log("Level Generated. MinMax: " + Mathf.Min(lightmap) + "  :  " + Mathf.Max(lightmap));

    }

    void DisplayLightmap()
    {
        if (levelLightPlane != null)
        {
            int newWidth = width;

            Texture2D tex = new Texture2D(newWidth, newWidth);
            int factor = (int)(newWidth / width);

            // Write to Texture
            for (int i = 0; i < width; i++) // Y
            {
                for (int j = 0; j < width; j++) // X
                {

                    for (int k = i * factor; k < (i + 1) * factor; k++)
                    {
                        //yPos += amountAdded;

                        for (int l = j * factor; l < (j + 1) * factor; l++)
                        {

                            tex.SetPixel(l, k, new Color(1, 1, 0.95f) * ((GetLightValue(i, j))+displayOffset));
                        }
                    }
                }
            }

            tex.Apply();

            tex = IncreaseRes(tex);
            tex = SmoothTexture(tex);
            tex = SmoothTexture(tex);
            tex = IncreaseRes(tex);

            tex = SmoothTexture(tex);
            tex = IncreaseRes(tex);
            tex = SmoothTexture(tex);
            tex = SmoothTexture(tex);

            //  levelLightPlane.GetComponent<Renderer>().material.SetTexture("_MainTex", tex);

            levelLightPlane.GetComponent<Renderer>().material.color = bgColor;
            levelLightPlane.GetComponent<Renderer>().material.SetTexture("_EmissionMap", tex);
            levelLightPlane.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.white * 0.65f * 2);
            levelLightPlane.transform.localScale = new Vector3(width+2, width+2, 1);
            // levelLightPlane.transform.position = new Vector3(-0.5f, -0.5f, 5f);
            levelLightPlane.transform.position = new Vector3(0.5f, -0.5f, 5f);

            Debug.Log("TEXTURE: " + tex.width + "  : " + tex.height);

        }
        else
        {
            Debug.LogError("No LevelQuad Assigned!");
        }

        // tex.scal

        // CUBES For Light

        //for (int i = 0; i < lightmap.Length; i++)
        //{
        //    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //    cube.transform.position = new Vector3(i % width - (width / 2), i / width - (width / 2), 50);
        //    float lightvalue = ((float)lightmap[i]);

        //    if (lightdisplayMat != null)
        //    {
        //        cube.GetComponent<Renderer>().material = lightdisplayMat;
        //    }

        //    cube.GetComponent<Renderer>().material.color = lightvalue * new Color(0.85f, 0.85f, 1);
        //    cube.GetComponent<Renderer>().material.SetColor("_EmissionColor", lightvalue * Color.white);
        //    cube.transform.parent = transform;
        //    cube.name = "lightmapPoint";
        //    Destroy(cube.GetComponent<Collider>());
        //}
    }

    private Texture2D IncreaseRes(Texture2D tex)
    {
        Texture2D nTex = new Texture2D((int)(tex.width * 2f), (int)(tex.height * 2f));

        for (int i = 0; i < tex.height; i++)
        {
            for (int j = 0; j < tex.width; j++)
            {
                
                nTex.SetPixel(j * 2, i * 2, tex.GetPixel(j, i));
                nTex.SetPixel(j * 2 + 1, i * 2, tex.GetPixel(j, i));
                nTex.SetPixel(j * 2, i * 2 + 1, tex.GetPixel(j, i));
                nTex.SetPixel(j * 2 + 1, i * 2 + 1, tex.GetPixel(j, i));
            }
        }
        nTex.filterMode = FilterMode.Trilinear;
        tex.filterMode = FilterMode.Trilinear;
        nTex.Apply();
        
        return nTex;
    }

    private Texture2D SmoothTexture(Texture2D tex)
    {
        Texture2D tmpTex = new Texture2D(tex.width, tex.height);

        for (int i = 0; i < tex.height; i++)
        {
            for (int j = 0; j < tex.width; j++)
            {
                Color c= Color.black;

                if (i > 0 && i < tex.height - 1)
                {
                    if (j > 0 && j < tex.width - 1)
                    {
                        c = (tex.GetPixel(j, i - 1) + tex.GetPixel(j, i + 1)) * 0.25f
                            + (tex.GetPixel(j + 1, i) + tex.GetPixel(j + 1, i))*0.25f;
                    }
                }
                else
                    if (i == 0)
                {
                    c = (tex.GetPixel(j, i) + tex.GetPixel(j, i + 1)) * 0.5f;
                }
                else
                    if (i == tex.height - 1)
                {
                    c = (tex.GetPixel(j, i - 1) + tex.GetPixel(j, i)) * 0.5f;
                }

                tmpTex.SetPixel(j, i,c);
            }
        }

        tmpTex.filterMode = FilterMode.Trilinear;
        tmpTex.Apply();
        return tmpTex;

    }


    float GetLightValue(int i, int j)
    {
        i = Mathf.Clamp(i, 0, width - 1);
        j = Mathf.Clamp(j, 0, width - 1);

        return ((float)lightmap[i * width + j]);

    }


}


#if UNITY_EDITOR
[CustomEditor(typeof(Level))]
public class LevelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Level myScript = (Level)target;
        GUILayout.Label("---------------------");
        GUILayout.Label("Dauert ewig: ");
        if (GUILayout.Button("Generate Lightmap"))
        {
            myScript.CreateLightMap();
        }
    }
}
#endif