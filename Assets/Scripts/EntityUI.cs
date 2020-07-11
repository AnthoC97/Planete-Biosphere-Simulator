using UnityEngine;
using UnityEngine.UI;
using MoonSharp.Interpreter;
using System.Collections.Generic;

public class EntityUI : MonoBehaviour
{
    private Dictionary<string, ICanvasElement> UIElements;
    private Dictionary<string, DynValue> sharedContext;

    public void Awake()
    {
        UIElements = new Dictionary<string, ICanvasElement>();
    }

    public void Start()
    {
        sharedContext = GetComponent<ScriptedBehaviour>().sharedContext;
    }

    public void LateUpdate()
    {
        foreach (var pair in UIElements) {
            if (!sharedContext.ContainsKey(pair.Key)) {
                Debug.LogError("Shared context doesn't contain key "
                        + pair.Key);
            }

            if (pair.Value is Slider) {
                ((Slider) pair.Value).value =
                    (float) sharedContext[pair.Key].CastToNumber();
            } else if (pair.Value is Text) {
                ((Text) pair.Value).text =
                    sharedContext[pair.Key].CastToString();
            }
        }
    }

    public void AddElement(string key, ICanvasElement element)
    {
        UIElements[key] = element;
    }
}
