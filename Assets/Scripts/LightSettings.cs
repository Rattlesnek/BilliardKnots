using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CustomLightType
{
    Spot = 0,
    Directional = 1,
    Point = 2
}

public class LightSettings : MonoBehaviour
{
    public CustomLightType LightType;

    public Color Color;

    public float Intensity;

    public float Range;

    [Range(0, 179)]
    public float SpotAngle;

    private Light light;

    private bool updateNextFrame = false;

    void Start()
    {
        light = GetComponent<Light>();
        LightType = (CustomLightType) light.type;
        Color = light.color;
        Intensity = light.intensity;
        Range = light.range;
        SpotAngle = light.spotAngle;
    }

    void Update()
    {
        light.type = (LightType) LightType;
        light.color = Color;
        light.intensity = Intensity;
        light.range = Range;
        light.spotAngle = SpotAngle;
    }
}
