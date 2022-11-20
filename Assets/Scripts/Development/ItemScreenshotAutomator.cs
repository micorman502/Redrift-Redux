using System.IO;
using UnityEngine;

public class ItemScreenshotAutomator : MonoBehaviour
{
    const string defaultPath = "/_Items/Icons/";
    [SerializeField] ItemScreenshotData data;
    [SerializeField] ItemInfo targetItem;
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
        Destroy(image);

        File.WriteAllBytes(GetPath(), bytes);
    }

    string GetPath ()
    {
        return Application.dataPath + (data.useDefaultPath ? defaultPath : data.customPath) + targetItem.itemName + ".png";
    }
}