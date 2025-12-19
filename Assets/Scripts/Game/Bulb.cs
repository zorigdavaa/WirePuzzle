using Unity.VisualScripting;
using UnityEngine;

public class Bulb : MonoBehaviour
{
    Light Light;
    public MeshRenderer Renderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Light = gameObject.GetOrAddComponent<Light>();
        Light.color = Color.yellow;
        Light.intensity = 1.5f;
        Light.range = 10f;
        Light.type = LightType.Point;
        Light.enabled = false;
        // Renderer = gameObject.GetComponentInChildren<MeshRenderer>();
    }
    [ContextMenu("Turn On")]
    public void TurnOn()
    {
        Light.enabled = true;
        Renderer.material.EnableKeyword("_EMISSION");
        // Implementation for turning on the bulb
        Debug.Log("Bulb is turned on.");
    }
}
