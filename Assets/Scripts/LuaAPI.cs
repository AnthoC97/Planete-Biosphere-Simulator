using UnityEngine;
using UnityEngine.UI;
using MoonSharp.Interpreter;

[MoonSharpUserData]
public class LuaAPI
{
    private Planet planet;
    private PBSNoiseScript planetNoiseScript;
    private static GameObject rabbitPrefab;
    private static GameObject sliderPrefab;
    private static GameObject textPrefab;

    public LuaAPI()
    {
        planet = GameObject.Find("Planet").GetComponent<Planet>();
        planetNoiseScript =
            GameObject.Find("Planet").GetComponent<PBSNoiseScript>();
        if (rabbitPrefab == null) {
            rabbitPrefab = Resources.Load<GameObject>("Rabbit");
            if (rabbitPrefab == null) {
                Debug.LogError("Could not load rabbit prefab!");
            } else {
                sliderPrefab =
                    rabbitPrefab.transform.Find("Canvas/Slider").gameObject;
                textPrefab =
                    rabbitPrefab.transform.Find("Canvas/State").gameObject;
            }
        }
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

        GameObject slider = GameObject.Instantiate(sliderPrefab);
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

        Debug.Log("Successfully added an UI Slider element!");

        return slider;
    }

    public GameObject AddUIText(GameObject gameObject, string sharedVar)
    {
        EntityUI entityUI = gameObject.GetComponent<EntityUI>();
        Transform canvasTransform = gameObject.transform.Find("Canvas");

        if (canvasTransform == null || entityUI == null) {
            Debug.LogError("canvasTransform == null (" + canvasTransform
                    +") || entityUI == null (" + entityUI + ")");
            return null;
        }

        GameObject text = GameObject.Instantiate(textPrefab);
        text.SetActive(true); // XXX WTF? Not active by default?

        Text textComponent = text.GetComponent<Text>();
        text.transform.SetParent(canvasTransform);
        text.transform.localScale = new Vector3(1, 1, 1); // XXX WTF? Keep it!
        Vector3 pos = text.transform.localPosition;
        pos.z = 0; // XXX Again, why does it take the position of the parent?
        text.transform.localPosition = pos;

        entityUI.AddElement(sharedVar, textComponent);

        Debug.Log("Successfully added an UI Text element!");

        return text;
    }
}
