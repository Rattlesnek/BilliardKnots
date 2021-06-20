using UnityEngine;
using System;
using System.IO;
using SFB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class KnotParameters : MonoBehaviour
{
    public Transform KnotTransform;

    private KnotSelector knotSelector;

    private LissajousKnot lissajous;

    private TorusKnot torus;

    private LissajousToricKnot lissajousToric;

    private MeshTiling meshTiling;


    private void Start()
    {
        knotSelector = KnotTransform.GetComponent<KnotSelector>();
        lissajous = KnotTransform.GetComponent<LissajousKnot>();
        torus = KnotTransform.GetComponent<TorusKnot>();
        lissajousToric = KnotTransform.GetComponent<LissajousToricKnot>();
        meshTiling = KnotTransform.GetComponent<MeshTiling>();
    }

    public void OnSave()
    {
        var knotSelectorParams = JsonUtility.ToJson(knotSelector);
        string knotTypeParams = "";
        switch (knotSelector.KnotType)
        {
            case KnotType.Lissajous:
                knotTypeParams = JsonUtility.ToJson(lissajous);
                break;

            case KnotType.Torus:
                knotTypeParams = JsonUtility.ToJson(torus);
                break;

            case KnotType.LissajousToric:
                knotTypeParams = JsonUtility.ToJson(lissajousToric);
                break;

            default:
                break;
        }
        var meshTilingParams = JsonUtility.ToJson(meshTiling);

        var knotSelectorObj = JObject.Parse(knotSelectorParams);
        var knotTypeObj = JObject.Parse(knotTypeParams);
        var meshTilingObj = JObject.Parse(meshTilingParams);
        meshTilingObj.Remove("mesh");
        meshTilingObj.Remove("Material");
        meshTilingObj.Remove("materialSettings");

        JObject allParameters = new JObject(
            new JProperty(knotSelector.ToString(), knotSelectorObj), 
            new JProperty(knotSelector.KnotType.ToString(), knotTypeObj),
            new JProperty(meshTiling.ToString(), meshTilingObj)
        );
        
        var path = StandaloneFileBrowser.SaveFilePanel("Save Knot Parameters", "", "knot_params", "json");
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, allParameters.ToString());
        }
    }

    public void OnOpen()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel("Open Knot Parameters", "", "json", false);
        if (paths.Length > 0)
        {
            var allParameters = JObject.Parse(File.ReadAllText(paths[0]));

            JObject knotSelectorObj = (JObject)allParameters[knotSelector.ToString()];
            KnotType knotType = (KnotType)(int)knotSelectorObj["knotType"];

            JObject knotTypeObj = (JObject)allParameters[knotType.ToString()];
            switch (knotType)
            {
                case KnotType.Lissajous:
                    JsonUtility.FromJsonOverwrite(knotTypeObj.ToString(), lissajous);
                    break;

                case KnotType.Torus:
                    JsonUtility.FromJsonOverwrite(knotTypeObj.ToString(), torus);
                    break;

                case KnotType.LissajousToric:
                    JsonUtility.FromJsonOverwrite(knotTypeObj.ToString(), lissajousToric);
                    break;

                default:
                    break;
            }

            JObject meshTilingObj = (JObject)allParameters[meshTiling.ToString()];
            JsonUtility.FromJsonOverwrite(meshTilingObj.ToString(), meshTiling);

            JsonUtility.FromJsonOverwrite(knotSelectorObj.ToString(), knotSelector);
            // Re-enable
            knotSelector.enabled = false;
            knotSelector.enabled = true;
        }
    }

}
