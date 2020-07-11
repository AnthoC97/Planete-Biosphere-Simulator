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
            float minValue, float maxValue)
    {
        EntityUI entityUI = gameObject.GetComponent<EntityUI>();
        Transform canvasTransform = gameObject.transform.Find("Canvas");

        if (canvasTransform == null || entityUI == null) {
            Debug.LogError("canvasTransform == null (" + canvasTransform
                    +") || entityUI == null (" + entityUI + ")");
            return null;
        }

        //GameObject canvas = canvasTransform.gameObject;

        /*
        GameObject slider = new GameObject("Slider");

        slider.SetLayer("UI");

        Slider sliderComponent = slider.AddComponent<Slider>();
        sliderComponent.interactable = false;
        sliderComponent.minValue = minValue;
        sliderComponent.maxValue = maxValue;

        RectTransform rt = slider.GetComponent<RectTransform>();
        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 60);

        slider.transform.SetParent(canvasTransform);

        GameObject background = new GameObject("Background");
        Image bgImage = background.AddComponent<Image>();
        bgImage.
        */

        GameObject slider = GameObject.Instantiate(sliderPrefab);
        slider.SetActive(true); // FIXME WTF?

        Slider sliderComponent = slider.GetComponent<Slider>();
        sliderComponent.minValue = minValue;
        sliderComponent.maxValue = maxValue;
        slider.transform.SetParent(canvasTransform);
        slider.transform.localScale = new Vector3(1, 1, 1); // XXX WTF? Keep it!
        Vector3 pos = slider.transform.localPosition;
        pos.z = 0;
        slider.transform.localPosition = pos;

        entityUI.AddElement(sharedVar, sliderComponent);

        Debug.Log("Successfully added an UI Slider element!");

        return slider;
    }
}
