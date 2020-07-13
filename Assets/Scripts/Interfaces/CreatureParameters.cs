using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreatureParameters : MonoBehaviour
{
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
        for (int i = 0; i < groupCount.value; ++i) {
            Vector3 groupPosition = Random.onUnitSphere;
            for (int j = 0; j < groupSize.value; ++j) {
                EntityFactory.AddEntityInWorld(creatureName.text,
                        groupPosition + Random.insideUnitSphere / 10);
            }
        }
    }
}
