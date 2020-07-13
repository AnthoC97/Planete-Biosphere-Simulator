using System;
using UnityEngine;
using UnityEngine.UI;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class LuaAPI
{
    private Planet planet;
    private PBSNoiseScript planetNoiseScript;

    public LuaAPI()
    {
        planet = GameObject.Find("Planet").GetComponent<Planet>();
        planetNoiseScript =
            GameObject.Find("Planet").GetComponent<PBSNoiseScript>();

        EntityFactory.Initialize();
    }

    public static void Register(Script script)
    {
        script.Globals["API"] = new LuaAPI();
    }

    public Vector3 GetGroundPositionWithElevation(Vector3 normalizedPosition,
            float addedElevation)
    {
        float elevation = planetNoiseScript
            .GetNoiseGenerator().GetNoise3D(normalizedPosition);
        return normalizedPosition * (1 + elevation) * planet.radius;
    }

    public GameObject AddUISlider(GameObject gameObject, string sharedVar,
            float minValue, float maxValue, float r = 1, float g = 59/255,
            float b = 59/255)
    {
        EntityUI entityUI = gameObject.GetComponent<EntityUI>();
        Transform canvasTransform = gameObject.transform.Find("Canvas");

        if (canvasTransform == null || entityUI == null) {
            Debug.LogError("canvasTransform == null (" + canvasTransform
                    +") || entityUI == null (" + entityUI + ")");
            return null;
        }

        GameObject slider = GameObject.Instantiate(EntityFactory.sliderPrefab);
        slider.SetActive(true); // XXX WTF? Not active by default?

        Slider sliderComponent = slider.GetComponent<Slider>();
        sliderComponent.minValue = minValue;
        sliderComponent.maxValue = maxValue;
        slider.transform.SetParent(canvasTransform);
        slider.transform.localScale = new Vector3(1, 1, 1); // XXX WTF? Keep it!
        Vector3 pos = slider.transform.localPosition;
        pos.z = 0; // XXX Again, why does it take the position of the parent?
        slider.transform.localPosition = pos;

        GameObject fill = slider.transform.Find("Fill Area/Fill").gameObject;
        Image image = fill.GetComponent<Image>();
        image.color = new Color(r, g, b, 1);

        entityUI.AddElement(sharedVar, sliderComponent);

        return slider;
    }

    public GameObject AddUIText(GameObject gameObject, string sharedVar,
            DynValue translationTable = null)
    {
        EntityUI entityUI = gameObject.GetComponent<EntityUI>();
        Transform canvasTransform = gameObject.transform.Find("Canvas");

        if (canvasTransform == null || entityUI == null) {
            Debug.LogError("canvasTransform == null (" + canvasTransform
                    +") || entityUI == null (" + entityUI + ")");
            return null;
        }

        GameObject text = GameObject.Instantiate(EntityFactory.textPrefab);
        text.SetActive(true); // XXX WTF? Not active by default?

        Text textComponent = text.GetComponent<Text>();
        text.transform.SetParent(canvasTransform);
        text.transform.localScale = new Vector3(1, 1, 1); // XXX WTF? Keep it!
        Vector3 pos = text.transform.localPosition;
        pos.z = 0; // XXX Again, why does it take the position of the parent?
        text.transform.localPosition = pos;

        entityUI.AddElement(sharedVar, textComponent);

        if (translationTable != null
                && translationTable.Type != DataType.Void) {
            if (translationTable.Type == DataType.Table) {
                entityUI.AddTranslationTable(translationTable.Table);
            } else {
                Debug.LogError("[AddUIText] Translation table is not a Table.");
                Debug.LogError("[AddUIText] Translation table is a "
                        + translationTable.Type);
            }
        }

        return text;
    }

    public bool AddEntityInWorld(string entityTypeName, Vector3 position)
    {
        return EntityFactory.AddEntityInWorld(entityTypeName, position);
    }

    public bool AddEntityInWorld(string entityTypeName, float x, float y,
            float z)
    {
        return EntityFactory.AddEntityInWorld(entityTypeName, x, y, z);
    }
}
