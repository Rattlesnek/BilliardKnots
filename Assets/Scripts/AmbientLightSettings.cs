using UnityEngine;

public class AmbientLightSettings : MonoBehaviour
{
    [Range(0, 5)]
    public float AmbientIntensity;

    void Start()
    {
        AmbientIntensity = RenderSettings.ambientIntensity;
    }

    void Update()
    {
        RenderSettings.ambientIntensity = AmbientIntensity;
    }
}
