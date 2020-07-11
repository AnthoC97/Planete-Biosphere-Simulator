using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GeneratorInterface : MonoBehaviour
{
    List<Slider> parameters;

    void Start()
    {
        parameters = new List<Slider>();
        for (int i = 0; i < transform.childCount ; i++)
        {
            parameters.Add(transform.GetChild(i).GetComponentInChildren<Slider>());
        }
    }

    public void Generate()
    {
        StaticSettings.useStaticSettings = true;
        StaticSettings.planetRadius = parameters[0].value;
        StaticSettings.waterPercent = parameters[1].value;
        StaticSettings.planetIntervalBeforeNewGeneration = parameters[2].value;
        StaticSettings.planetPopulationSize = (int)parameters[3].value;
        StaticSettings.planetSelectedCount = (int)parameters[4].value;
        StaticSettings.planetProbaMutation = parameters[5].value;
        StaticSettings.planetIcosahedronSubDiv = (int)parameters[6].value;

        SceneManager.LoadScene(1);
    }
}
