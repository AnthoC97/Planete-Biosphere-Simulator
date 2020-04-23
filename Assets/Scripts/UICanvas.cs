using UnityEngine;
using UnityEngine.UI;

public class UICanvas : MonoBehaviour
{
    public Text remainingCreaturesText;

    void Update()
    {
        GameObject[] creatures =
            GameObject.FindGameObjectsWithTag("SimulationEntity");

        remainingCreaturesText.text =
            "Remaining creatures: " + creatures.Length.ToString();
    }
}
