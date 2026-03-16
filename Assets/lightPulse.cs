using UnityEngine;

public class lightPulse : MonoBehaviour
{
    public int materialIndex = 1; 
    public float pulseSpeed = 3f;
    public float minIntensity = 0.5f;
    public float maxIntensity = 5f;

    private Material targetMaterial;
    private Color baseColor;

    void Start()
    {
       //getting material from mesh renderer 
        Renderer renderer = GetComponent<Renderer>();
        targetMaterial = renderer.materials[materialIndex];
        
        //grabbing color
        baseColor = targetMaterial.GetColor("_EmissionColor");
    }

    void Update()
    {
        //using sine function for intensity of light
        float emission = minIntensity + Mathf.PingPong(Time.time * pulseSpeed, maxIntensity - minIntensity);
        
        // assigning intensity to the color
        targetMaterial.SetColor("_EmissionColor", baseColor * emission);
    }
}