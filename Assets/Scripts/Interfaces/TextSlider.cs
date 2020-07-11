using System;
using UnityEngine;
using UnityEngine.UI;

public class TextSlider : MonoBehaviour
{
    Slider slid;
    Text text;

    void Start()
    {
        text = transform.parent.GetChild(2).GetComponent<Text>();
        slid = GetComponent<Slider>();
        UpdateText();
    }

    public void UpdateText()
    { 
        text.text = Math.Round(slid.value,4).ToString();
    }

}
