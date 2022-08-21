using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Bitmap
{
    
}


[CustomEditor(typeof(WFC))]
class DecalMeshHelperEditor : Editor {
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var wfc = (WFC)target;
        if (GUILayout.Button("Generate"))
        {
            Debug.Log("Generating");
            wfc.Generate();
        }
    }
}
public class WFC : MonoBehaviour
{
    public RawImage image;
    public Texture2D pattern;
    public int N = 3, width = 48, height = 48, symmetry = 8;
    public bool periodic = false, periodicInput = true, ground = false;
    public int limit = (int) 1e9;
    public int seed = 1;
    public void Generate()
    {
        image.texture = pattern;
        var model = new OverlappingModel(pattern, N, width, height, periodicInput, periodic, symmetry, ground,
            Model.Heuristic.Entropy);
        model.Run(seed, limit);
        var outputPattern = model.Save();
        var outTex = intArrayToTexture(outputPattern, width, height);
        image.texture = outTex;
    }

    public static int[] textureToIntArray(Texture2D texture)
    {
        Debug.Log(texture.width + " " + texture.height + " " + texture.format);
    
        var x = texture.GetRawTextureData<int>();
        var ar = new int[x.Length];
        for (int i = 0; i < x.Length; i++) ar[i] = x[i];
        // foreach(var y in ar) Debug.Log(Convert.ToString(y, 2));
        return ar;
    }

    public static Texture2D intArrayToTexture(int[] ints, int width, int height)
    {
        byte[] bytes = new byte[ints.Length * sizeof(int)];
        Buffer.BlockCopy(ints, 0, bytes, 0, bytes.Length);
        
        Debug.Log(ints.Length + " " + bytes.Length + " " + width + " " + height);
        foreach(var b in bytes) Debug.Log(Convert.ToString(b, 2));
        var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.LoadRawTextureData(bytes);
        texture.Apply();
        return texture;
    }
}
