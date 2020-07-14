using System;
using UnityEngine;

public class EntityFactory
{
    public static GameObject rabbitPrefab;
    public static GameObject sliderPrefab;
    public static GameObject textPrefab;
    public static Mesh cubeMesh;
    public static Mesh sphereMesh;
    public static Mesh capsuleMesh;
    public static Mesh cylinderMesh;

    public EntityFactory()
    {
        Initialize();
    }

    public static void Initialize()
    {
        if (rabbitPrefab == null) {
            rabbitPrefab = Resources.Load<GameObject>("Rabbit");
            if (rabbitPrefab == null) {
                Debug.LogError("Could not load rabbit prefab!");
            } else {
                sliderPrefab =
                    rabbitPrefab.transform.Find("Canvas/Slider").gameObject;
                textPrefab =
                    rabbitPrefab.transform.Find("Canvas/State").gameObject;
            }
        }

        if (cubeMesh == null) {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubeMesh = cube.GetComponent<MeshFilter>().mesh;
            GameObject.Destroy(cube);

            GameObject sphere =
                GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphereMesh = sphere.GetComponent<MeshFilter>().mesh;
            GameObject.Destroy(sphere);

            GameObject capsule =
                GameObject.CreatePrimitive(PrimitiveType.Capsule);
            capsuleMesh = capsule.GetComponent<MeshFilter>().mesh;
            GameObject.Destroy(capsule);

            GameObject cylinder =
                GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cylinderMesh = cylinder.GetComponent<MeshFilter>().mesh;
            GameObject.Destroy(cylinder);
        }
    }

    public static bool AddEntityInWorld(string entityTypeName, Vector3 position)
    {
        // TODO Get the actual entity type configuration
        Mesh configuredMesh = GameObject.Instantiate(rabbitPrefab.transform.Find("Model").gameObject.GetComponent<MeshFilter>().sharedMesh);
        Type configuredBehaviour = typeof(ScriptedBehaviour);

        rabbitPrefab.SetActive(false);
        GameObject newEntity = GameObject.Instantiate(rabbitPrefab);
        rabbitPrefab.SetActive(true);

        newEntity.name = entityTypeName;
        newEntity.transform.position = position;

        GameObject model = newEntity.transform.Find("Model").gameObject;
        MeshFilter meshFilter = model.GetComponent<MeshFilter>();
        meshFilter.sharedMesh = configuredMesh;

        MeshRenderer renderer = model.GetComponent<MeshRenderer>();

        string scriptPath = "rabbitAI.lua";
        if (entityTypeName == "rabbit") {
            renderer.material.SetColor("_BaseColor", Color.white);
        } else if (entityTypeName == "fox") {
            scriptPath = "foxAI.lua";
            renderer.material.SetColor("_BaseColor", Color.red);
        }

        ScriptedBehaviour scriptedBehaviour =
            newEntity.GetComponent<ScriptedBehaviour>();
        if (configuredBehaviour == typeof(ScriptedBehaviour)) {
            scriptedBehaviour.scriptPath = scriptPath;
        } else {
            scriptedBehaviour.enabled = false;
            newEntity.AddComponent(configuredBehaviour);
        }

        newEntity.SetActive(true);

        return true;
    }

    public static GameObject AddEntityInWorld(CreatureParameters parameters,
            Vector3 position)
    {
        // TODO Get the actual entity type configuration
        string entityTypeName = parameters.creatureName.text;
        var scriptDropDownValue = parameters.ScriptDropDown.value;
        string scriptPath =
            parameters.ScriptDropDown.options[scriptDropDownValue].text;
        var colorDropdownValue = parameters.ColorDropdown.value;
        string colorText =
            parameters.ColorDropdown.options[colorDropdownValue].text;
        Mesh configuredMesh = MeshFromInt((int) parameters.MeshNum.value);
        Type configuredBehaviour = typeof(ScriptedBehaviour);

        rabbitPrefab.SetActive(false);
        GameObject newEntity = GameObject.Instantiate(rabbitPrefab);
        rabbitPrefab.SetActive(true);

        newEntity.name = entityTypeName;
        newEntity.transform.position = position;

        GameObject model = newEntity.transform.Find("Model").gameObject;
        MeshFilter meshFilter = model.GetComponent<MeshFilter>();
        meshFilter.sharedMesh = configuredMesh;

        MeshRenderer renderer = model.GetComponent<MeshRenderer>();

        renderer.material.SetColor("_BaseColor", ColorFromText(colorText));

        ScriptedBehaviour scriptedBehaviour =
            newEntity.GetComponent<ScriptedBehaviour>();
        if (configuredBehaviour == typeof(ScriptedBehaviour)) {
            scriptedBehaviour.scriptPath = scriptPath;
        } else {
            scriptedBehaviour.enabled = false;
            newEntity.AddComponent(configuredBehaviour);
        }

        newEntity.SetActive(true);

        return newEntity;
    }

    public static bool AddEntityInWorld(string entityTypeName, float x, float y,
            float z)
    {
        return AddEntityInWorld(entityTypeName, new Vector3(x, y, z));
    }

    private static Color ColorFromText(string text)
    {
        switch (text) {
            case "Red":
                return Color.red;
            case "Green":
                return Color.green;
            case "Blue":
                return Color.blue;
            case "White":
                return Color.black;
            case "Grey / Gray":
                return Color.grey;
            case "Magenta":
                return Color.magenta;
            case "Yellow":
                return Color.yellow;
            case "Cyan":
                return Color.cyan;
            default:
                Debug.Log("[ColorFromText] Unknown color " + text + ".");
                return Color.white;
        }
    }

    private static Mesh MeshFromInt(int number)
    {
        switch (number) {
            case 0:
                return cylinderMesh;
            case 1:
                return cubeMesh;
            case 2:
                return sphereMesh;
            default:
                return cubeMesh;
        }
    }
}
