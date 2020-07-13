using UnityEngine;
using UnityEngine.UI;

public class InterfaceTree : MonoBehaviour
{
    GeneticTree script;
    Text message;

    void Start()
    {
        message = GetComponent<Text>();
    }

    public void changeBool()
    {
        if (script.updateBool())
            message.text = "Process Tree Growth";
        else
            message.text = "Make Genetic Tree";
    }
}

