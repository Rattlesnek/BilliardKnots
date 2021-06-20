using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public enum SkyboxType
{
    SolidColor,
    Procedural,
    HDR
}

public class SkyboxSettings : MonoBehaviour
{
    [SerializeField]
    private SkyboxType skyboxType = SkyboxType.Procedural;
    public SkyboxType SkyboxType
    {
        get { return skyboxType; }
        set
        {
            if (skyboxType != value)
            {
                skyboxType = value;
                switched = true;
            }
        }
    }

    // General variables
    public Camera Camera;

    public GUIHandler GUIHandler;

    public Material ProceduralSkybox;


    // Solid color variables
    public Color SolidColor = Color.grey;


    // HDR variables
    public Material HDRSkybox;

    public Color Tint;


    // Procedural variables
    [Range(0, 1)]
    public float SunSize = 0.05f;

    [Range(1, 10)]
    public float SunSizeConvergence = 5;

    [Range(0, 5)]
    public float AtmosphereThickness = 1;

    public Color SkyTint = Color.grey;

    public Color Ground = Color.grey;


    // Mixed variable
    [Range(0, 8)]
    public float Exposure = 1;


    // Private
    private Material proceduralSky;

    private Material hdrSky;


    private ReflectionProbe refProbe;

    private bool switched = false;

    private HashSet<string> generalVariables = new HashSet<string>
    {
        "ProceduralSkybox", "Camera", "GUIHandler"
    };

    private HashSet<string> solidColorVariables = new HashSet<string>()
    {
        "SolidColor"
    };

    private HashSet<string> proceduralVariables = new HashSet<string>()
    {
        "SunSize", "SunSizeConvergence", "AtmosphereThickness", "SkyTint", "Ground", "Exposure"
    };

    private HashSet<string> hrdVariables = new HashSet<string>()
    {
        "HDRSkybox", "Tint", "Exposure"
    };


private void Start()
    {
        refProbe = GetComponent<ReflectionProbe>();
        
        proceduralSky = new Material(ProceduralSkybox);
        hdrSky = new Material(HDRSkybox);

        Tint = hdrSky.GetColor("_Tint");

        SunSize = proceduralSky.GetFloat("_SunSize");
        SunSizeConvergence = proceduralSky.GetFloat("_SunSizeConvergence");
        AtmosphereThickness = proceduralSky.GetFloat("_AtmosphereThickness");
        SkyTint = proceduralSky.GetColor("_SkyTint");
        Ground = proceduralSky.GetColor("_GroundColor");
        Exposure = proceduralSky.GetFloat("_Exposure");

        switched = true;
    }

    private void Update()
    {       
        if (switched ||
            hdrSky.name != HDRSkybox.name)
        {
            GUIHandler.HiddenVariables.UnionWith(generalVariables);
            GUIHandler.HiddenVariables.UnionWith(solidColorVariables);
            GUIHandler.HiddenVariables.UnionWith(proceduralVariables);
            GUIHandler.HiddenVariables.UnionWith(hrdVariables);
            GUIHandler.HiddenVariablesChanged = true;

            switch (SkyboxType)
            {
                case SkyboxType.SolidColor:
                    Camera.clearFlags = CameraClearFlags.SolidColor;
                    Camera.backgroundColor = SolidColor;
                    refProbe.clearFlags = ReflectionProbeClearFlags.SolidColor;
                    refProbe.backgroundColor = SolidColor;
                    GUIHandler.HiddenVariables.ExceptWith(solidColorVariables);
                    break;

                case SkyboxType.Procedural:
                    Camera.clearFlags = CameraClearFlags.Skybox;
                    refProbe.clearFlags = ReflectionProbeClearFlags.Skybox;
                    RenderSettings.skybox = proceduralSky;
                    DynamicGI.UpdateEnvironment();
                    GUIHandler.HiddenVariables.ExceptWith(proceduralVariables);
                    break;

                case SkyboxType.HDR:
                    Camera.clearFlags = CameraClearFlags.Skybox;
                    refProbe.clearFlags = ReflectionProbeClearFlags.Skybox;
                    hdrSky = new Material(HDRSkybox);
                    RenderSettings.skybox = hdrSky;
                    DynamicGI.UpdateEnvironment();
                    GUIHandler.HiddenVariables.ExceptWith(hrdVariables);
                    break;

                default:
                    break;
            }
            switched = false;
        }
        
        switch (SkyboxType)
        {
            case SkyboxType.SolidColor:
                Camera.backgroundColor = SolidColor;
                refProbe.backgroundColor = SolidColor;
                break;

            case SkyboxType.Procedural:
                proceduralSky.SetFloat("_SunSize", SunSize);
                proceduralSky.SetFloat("_SunSizeConvergence", SunSizeConvergence);
                proceduralSky.SetFloat("_AtmosphereThickness", AtmosphereThickness);
                proceduralSky.SetColor("_SkyTint", SkyTint);
                proceduralSky.SetColor("_GroundColor", Ground);
                proceduralSky.SetFloat("_Exposure", Exposure);
                break;

            case SkyboxType.HDR:
                hdrSky.SetColor("_Tint", Tint);
                hdrSky.SetFloat("_Exposure", Exposure);
                break;

            default:
                break;
        }
    }
}
