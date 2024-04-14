using System.IO;
using UnityEngine;

public class ItemScreenshotAutomater : MonoBehaviour
{
    const string defaultPath = "/Content/_Items/Icons/";
    [SerializeField] bool useTargetItemName;
    [SerializeField] ItemInfo targetItem;
    [SerializeField] string overrideItemName;
    [SerializeField] Camera targetCamera;

    public void CaptureImage ()
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = targetCamera.targetTexture;

        targetCamera.Render();

        Texture2D image = new Texture2D(targetCamera.targetTexture.width, targetCamera.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, targetCamera.targetTexture.width, targetCamera.targetTexture.height), 0, 0);
        image.Apply();
        RenderTexture.active = currentRT;

        byte[] bytes = image.EncodeToPNG();
        DestroyImmediate(image);

        File.WriteAllBytes(GetPath(), bytes);
    }

    string GetPath ()
    {
        return Application.dataPath + defaultPath + (useTargetItemName ? targetItem.name : overrideItemName) + ".png";
    }
}