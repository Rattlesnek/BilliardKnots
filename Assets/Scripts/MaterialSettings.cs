using UnityEngine;

public class MaterialSettings : MonoBehaviour
{
    public Color Color;

    // TODO
    //public Texture Texture;

    [Range(0, 1)]
    public float Metallic;

    [Range(0, 1)]
    public float Smoothness;

    public Material Material;

    private void Start()
    {
        Color = Material.color;
        //Texture = Material.mainTexture;
        Metallic = Material.GetFloat("_Metallic");
        Smoothness = Material.GetFloat("_Glossiness");
    }

    private void Update()
    {
        Material.color = Color;
        //Material.mainTexture = Texture;
        Material.SetFloat("_Metallic", Metallic);
        Material.SetFloat("_Glossiness", Smoothness);
    }
}
