using UnityEngine;
using SFB;

public class Renderer : MonoBehaviour
{
    public bool takeScreenshot = false;

    public Vector2Int Resolution = new Vector2Int(1920, 1080);
    
    private void Update()
    {
        if (takeScreenshot)
        {
            var path = StandaloneFileBrowser.SaveFilePanel("Save Rendered Image As", "", "knot", "png");
            if (!string.IsNullOrEmpty(path))
            {
                ScreenshotHandler.TakeScreenshot(path, Resolution.x, Resolution.y);
            }           
            takeScreenshot = false;
        }
    }
}
