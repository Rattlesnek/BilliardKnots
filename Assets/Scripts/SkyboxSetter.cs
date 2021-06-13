using UnityEngine;

public enum SkyboxType
{
    SolidColor,
    Procedural,
    Panoramic,
    Cube
}

public class SkyboxSetter : MonoBehaviour
{
    public SkyboxType SkyboxType = SkyboxType.Procedural;

    public Color SolidColor = Color.grey;

    [Range(0, 1)]
    public float SunSize = 0.05f;

    [Range(1, 10)]
    public float SunSizeConvergence = 5;

    [Range(0, 5)]
    public float AtmosphereThickness = 1;

    public Color SkyTint = Color.grey;

    public Color Ground = Color.grey;

    [Range(0, 8)]
    public float Exposure = 1;

    public Material ProceduralSkyboxSource;

    public Camera Camera;

    private Material proceduralSkybox;

    private void Start()
    {
        proceduralSkybox = new Material(ProceduralSkyboxSource);
        RenderSettings.skybox = proceduralSkybox;

        SunSize = proceduralSkybox.GetFloat("_SunSize");
        SunSizeConvergence = proceduralSkybox.GetFloat("_SunSizeConvergence");
        AtmosphereThickness = proceduralSkybox.GetFloat("_AtmosphereThickness");
        SkyTint = proceduralSkybox.GetColor("_SkyTint");
        Ground = proceduralSkybox.GetColor("_GroundColor");
        Exposure = proceduralSkybox.GetFloat("_Exposure");
    }

    private void Update()
    {
        switch (SkyboxType)
        {
            case SkyboxType.SolidColor:
                Camera.clearFlags = CameraClearFlags.SolidColor;
                Camera.backgroundColor = SolidColor;
                break;

            case SkyboxType.Procedural:
                Camera.clearFlags = CameraClearFlags.Skybox;
                RenderSettings.skybox = proceduralSkybox;
                proceduralSkybox.SetFloat("_SunSize", SunSize);
                proceduralSkybox.SetFloat("_SunSizeConvergence", SunSizeConvergence);
                proceduralSkybox.SetFloat("_AtmosphereThickness", AtmosphereThickness);
                proceduralSkybox.SetColor("_SkyTint", SkyTint);
                proceduralSkybox.SetColor("_GroundColor", Ground);
                proceduralSkybox.SetFloat("_Exposure", Exposure);
                break;

            default:

                break;
        }
    }
}
