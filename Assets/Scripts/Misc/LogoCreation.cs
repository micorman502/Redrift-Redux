using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
public class LogoCreation : MonoBehaviour //Credit to Helix for providing this script
{ //"you have to create a folder in assets called "icons" "
#if UNITY_EDITOR
    Camera cam;

    public GameObject[] items;
    void Start()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].SetActive(false);
        }
        for (int i = 0; i < items.Length; i++)
        {   
            items[i].SetActive(true);
            SaveScreenshot(items[i].name);
            items[i].SetActive(false);
        }
    }

    void SaveScreenshot(string name)
    {
        if (cam == null)
        {
            cam = GetComponent<Camera>();
            cam.enabled = false;
        }
        RenderTexture tempRT = new RenderTexture(512, 512,32,RenderTextureFormat.ARGB32);
        cam.targetTexture = tempRT;
        cam.Render();
        RenderTexture.active = tempRT;
        Texture2D tex = new Texture2D(512, 512,TextureFormat.RGBA32,true);
        tex.ReadPixels(new Rect(0, 0, 512, 512),0,0);
       cam.targetTexture = null;
         Destroy(tempRT);
        tex.Apply();
        byte[] pngTex = tex.EncodeToPNG();
        string path = Application.dataPath + "/icons/" + name + ".png";
        File.WriteAllBytes(path, pngTex);
    }


#endif
}
