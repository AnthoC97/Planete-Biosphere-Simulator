using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void QuitConfirm()
    {
        Application.Quit();
    }

    public void Resume()
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }

    public void LoadMeny()
    {
        SceneManager.LoadScene(0);
    }

}
