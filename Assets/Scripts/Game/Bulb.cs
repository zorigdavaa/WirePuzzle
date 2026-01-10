using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Bulb : PuzzleElement
{
    public Sprite OnSprite;
    public Sprite OffSprote;
    public Image BulbImage;
    // Light Light;
    // public MeshRenderer Renderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Light = gameObject.GetOrAddComponent<Light>();
        // Light.color = Color.yellow;
        // Light.intensity = 1.5f;
        // Light.range = 10f;
        // Light.type = LightType.Point;
        // Light.enabled = false;
        // Renderer = gameObject.GetComponentInChildren<MeshRenderer>();
    }

    public void TurnOn(bool on)
    {
        if (on)
        {
            // Light.enabled = true;
            // Renderer.material.EnableKeyword("_EMISSION");
            // Implementation for turning on the bulb
            Debug.Log("Bulb is turned on.");
            BulbImage.sprite = OnSprite;
        }
        else
        {
            // Light.enabled = false;
            // Renderer.material.DisableKeyword("_EMISSION");
            BulbImage.sprite = OffSprote;
        }


    }
    [ContextMenu("Turn On")]
    public void TurnOn()
    {

        // Light.enabled = true;
        // Renderer.material.EnableKeyword("_EMISSION");
        // Implementation for turning on the bulb
        Debug.Log("Bulb is turned on.");
        BulbImage.sprite = OnSprite;
    }

    public override void TakeDamage()
    {

        // throw new NotImplementedException();
    }
}
