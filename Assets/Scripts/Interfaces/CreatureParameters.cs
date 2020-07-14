using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureParameters : MonoBehaviour
{
    private Simulation simulation;

    public InputField creatureName;
    public Slider MeshNum;
    public Slider groupCount;
    public Slider groupSize;
    public Dropdown ScriptDropDown;
    public Image img;
    List<Sprite> sprites;

    void Start()
    {
        sprites = new List<Sprite>();
        sprites.Add(Resources.Load<Sprite>("MeshSprite/mesh1"));
        sprites.Add(Resources.Load<Sprite>("MeshSprite/mesh2"));
        sprites.Add(Resources.Load<Sprite>("MeshSprite/mesh3"));
        creatureName.text = "rabbit";

        simulation = GameObject.Find("Simulation").GetComponent<Simulation>();
        EntityFactory.Initialize();

        updateMesh();
        loadscript();
    }

    public void loadscript()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath /*+ "/lua"*/);
        if (!dir.Exists)
            dir.Create();

        ScriptDropDown.ClearOptions();
        List<string> list = new List<string>();
        foreach (FileInfo f in dir.GetFiles())
        {
            list.Add(f.Name);
        }
        ScriptDropDown.AddOptions(list);
    }

    public void updateMesh()
    {
        img.sprite = sprites[(int)MeshNum.value];
    }

    public void Generate()
    {
        GameObject[] generatedEntities =
            new GameObject[(int) groupCount.value * (int) groupSize.value];

        for (int i = 0; i < groupCount.value; ++i) {
            Vector3 groupPosition = Random.onUnitSphere;
            for (int j = 0; j < (int) groupSize.value; ++j) {
                generatedEntities[i * (int) groupSize.value + j] =
                    EntityFactory.AddEntityInWorld(this,
                            groupPosition + Random.insideUnitSphere / 10);
            }
        }

        foreach (GameObject entity in generatedEntities) {
            Vector3 normalizedPosition = entity.transform.position.normalized;
            entity.transform.position =
                simulation.GetGroundPositionWithElevation(normalizedPosition,
                        .5f);
        }
    }
}
