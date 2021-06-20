using UnityEngine;

public enum LightTypeCustom
{
    Directional = 1,
    Point = 2
}

public class LightSettings : MonoBehaviour
{
    public LightTypeCustom LightTypeCustom;

    public Color Color;

    [Range(0, 5)]
    public float Intensity;

    public float Range;

    private Light myLight;

    void Start()
    {
        myLight = GetComponent<Light>();
        LightTypeCustom = (LightTypeCustom) myLight.type;
        Color = myLight.color;
        Intensity = myLight.intensity;
        Range = myLight.range;
    }

    void Update()
    {
        myLight.type = (LightType)LightTypeCustom;
        myLight.color = Color;
        myLight.intensity = Intensity;
        myLight.range = Range;
    }
}
