using UnityEngine;
using UnityEngine.UI;
using SFB;

/// <summary>
/// Class responsible for item loading.
/// </summary>
public class ItemHandler : MonoBehaviour
{
    public bool loadItem = false;

    public bool removeItem = false;

    private const string itemName = "Item";

    private void Update()
    {
        if (loadItem)
        {
            var paths = StandaloneFileBrowser.OpenFilePanel("Open Mesh", "", "obj", false);
            if (paths.Length > 0)
            {
                LoadItem(paths[0]);
            }

            loadItem = false;
        }

        if (removeItem)
        {
            RemoveItem();
            removeItem = false;
        }
    }

    /// <summary>
    /// Loads item from path specified in input field.
    /// </summary>
    public void LoadItem(string path)
    {
        if (!GameObject.Find(itemName))
        {
            Mesh mesh = FastObjImporter.Instance.ImportFile(path);
            CreateItem(mesh);
        }
    }

    /// <summary>
    /// Removes item from scene.
    /// </summary>
    public void RemoveItem()
    {
        var item = GameObject.Find(itemName);
        Destroy(item);
    }

    private void CreateItem(Mesh mesh)
    {
        var go = new GameObject(itemName);
        go.AddComponent<MeshFilter>().mesh = mesh;
        go.AddComponent<MeshRenderer>().material = new Material(Shader.Find("Diffuse"));
    }
}

