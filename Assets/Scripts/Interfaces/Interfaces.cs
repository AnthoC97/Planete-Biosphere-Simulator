using UnityEngine;
using UnityEngine.InputSystem;

public class Interfaces : MonoBehaviour
{
    public GameObject menu;
    GameObject quitCheck;

    void Start()
    {
        quitCheck = menu.transform.GetChild(0).Find("QuitCheck").gameObject;

    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            menu.SetActive(!menu.activeInHierarchy);
            quitCheck.SetActive(false);
        }
    }
}
