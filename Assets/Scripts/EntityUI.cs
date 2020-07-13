using UnityEngine;
using UnityEngine.UI;
using MoonSharp.Interpreter;
using System.Collections.Generic;

public class EntityUI : MonoBehaviour
{
    private Dictionary<string, ICanvasElement> UIElements;
    private Dictionary<string, DynValue> sharedContext;
    private Dictionary<string, string> translationDictionary;

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
                string text = sharedContext[pair.Key].CastToString();
                if (translationDictionary != null
                        && translationDictionary.ContainsKey(text)) {
                    text = translationDictionary[text];
                }
                ((Text) pair.Value).text = text;
            }
        }
    }

    public void AddElement(string key, ICanvasElement element)
    {
        UIElements[key] = element;
    }

    public void AddTranslationTable(Table translationTable)
    {
        translationDictionary = new Dictionary<string, string>();

        foreach (var pair in translationTable.Pairs) {
            translationDictionary[pair.Key.String] = pair.Value.String;
        }
    }
}
