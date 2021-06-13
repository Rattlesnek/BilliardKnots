using System.IO;
using UnityEngine;

public class ScreenshotHandler : MonoBehaviour
{
    private static ScreenshotHandler instance;

    private Camera myCamera;

    private bool takeScreenshotNextFrame;

    private string screenshotPath;

    private void Awake()
    {
        myCamera = GetComponent<Camera>();
        instance = this;
    }

    private void OnPostRender()
    {
        if (takeScreenshotNextFrame)
        {
            takeScreenshotNextFrame = false;

            RenderTexture renderTexture = myCamera.targetTexture;

            Texture2D renderResult = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, renderTexture.width, renderTexture.height);
            renderResult.ReadPixels(rect, 0, 0);

            byte[] byteArray = renderResult.EncodeToPNG();
            File.WriteAllBytes(screenshotPath, byteArray);

            RenderTexture.ReleaseTemporary(renderTexture);
            myCamera.targetTexture = null;
        }
    }

    private void TakeScreenshot_instance(string path, int width, int height)
    {
        myCamera.targetTexture = RenderTexture.GetTemporary(width, height, 16);
        screenshotPath = path;
        takeScreenshotNextFrame = true;
    }

    public static void TakeScreenshot(string path, int width, int height)
    {
        instance.TakeScreenshot_instance(path, width, height);
    }
}
