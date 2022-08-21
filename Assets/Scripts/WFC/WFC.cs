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
        if (GUILayout.Button("GenerateTest"))
        {
            Debug.Log("Generating");
            wfc.GenerateTest();
        }
    }
}
public class WFC : MonoBehaviour
{
    public RawImage image;
    public Texture2D pattern;
    public void GenerateTest()
    {
        image.texture = pattern;
        image.texture = Generate(pattern, 3, 48, 48, false, true, false, 8, (int)1e9, 1);
    }

    public static Texture2D Generate(Texture2D pattern, int n, int width, int height, bool periodic, bool periodicInput,
         bool ground, int symmetry, int limit, int seed)
    {
        var model = new OverlappingModel(pattern, n, width, height, periodicInput, periodic, symmetry, ground,
            Model.Heuristic.Entropy);
        model.Run(seed, limit);
        var outputPattern = model.Save();
        var outTex = intArrayToTexture(outputPattern, width, height);
        return outTex;
    }

    public static int[] textureToIntArray(Texture2D texture)
    {
        var x = texture.GetRawTextureData<int>();
        var ar = new int[x.Length];
        for (int i = 0; i < x.Length; i++) ar[i] = x[i];
        return ar;
    }

    public static Texture2D intArrayToTexture(int[] ints, int width, int height)
    {
        byte[] bytes = new byte[ints.Length * sizeof(int)];
        Buffer.BlockCopy(ints, 0, bytes, 0, bytes.Length);
        
        var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        texture.LoadRawTextureData(bytes);
        texture.Apply();
        return texture;
    }
}
