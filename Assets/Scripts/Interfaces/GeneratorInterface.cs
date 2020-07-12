using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GeneratorInterface : MonoBehaviour
{
    List<Slider> parameters;
    Dropdown drop;

    void Start()
    {
        parameters = new List<Slider>();
        for (int i = 0; i < transform.childCount ; i++)
        {
            if (i != 7)
                parameters.Add(transform.GetChild(i).GetComponentInChildren<Slider>());
            else if (i == 7)
            {
                drop = transform.GetChild(i).GetComponentInChildren<Dropdown>();
                loadscript();
            }
        }

    }

    public void loadscript()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/lua");
        if (!dir.Exists)
            dir.Create();

        Debug.Log(Application.dataPath + "/lua");
        drop.ClearOptions();
        List<string> list = new List<string>();
        foreach (FileInfo f in dir.GetFiles())
        {
            list.Add(f.Name);
        }
        drop.AddOptions(list);
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
        if (drop.options.Count > 0)
            StaticSettings.planetScript = Application.dataPath + drop.options[drop.value].text;

        SceneManager.LoadScene(1);
    }
}
