using UnityEngine;
using UnityEngine.UI;
using SFB;

public class Renderer : MonoBehaviour
{
    public InputField InputX;

    public InputField InputY;

    private Vector2Int Resolution = new Vector2Int(1920, 1080);

    public void OnEndEditResolution()
    {
        Resolution.x = int.Parse(InputX.text);
        Resolution.y = int.Parse(InputY.text);
    }

    public void OnRenderClicked()
    {
        var path = StandaloneFileBrowser.SaveFilePanel("Save Rendered Image As", "", "knot", "png");
        if (!string.IsNullOrEmpty(path))
        {
            ScreenshotHandler.TakeScreenshot(path, Resolution.x, Resolution.y);
        }
    }
}
