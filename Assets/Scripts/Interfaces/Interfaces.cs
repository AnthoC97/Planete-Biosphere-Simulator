using UnityEngine;
using UnityEngine.InputSystem;

public class Interfaces : MonoBehaviour
{
    public GameObject creaturesParam;
    public GameObject menu;
    GameObject quitCheck;

    void Start()
    {
        quitCheck = menu.transform.GetChild(0).Find("QuitCheck").gameObject;

    }

    void Update()
    {
        if (Keyboard.current.cKey.wasPressedThisFrame && !creaturesParam.activeInHierarchy)
            creaturesParam.SetActive(true);
        else if (Keyboard.current.escapeKey.wasPressedThisFrame && !creaturesParam.activeInHierarchy)
        {
            menu.SetActive(!menu.activeInHierarchy);
            quitCheck.SetActive(false);
        }
        else if (Keyboard.current.escapeKey.wasPressedThisFrame && creaturesParam.activeInHierarchy)
        {
            creaturesParam.SetActive(false);
        }
    }
}
