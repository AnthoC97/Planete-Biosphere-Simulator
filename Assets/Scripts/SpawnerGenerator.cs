using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum RandomGenerator
{
    //PoissonDisc,
    RandomLinear
}

[Serializable]
public struct SpawnerConfig
{
    public RandomGenerator randomGenerator;
    public int seed;
    public float minDistance;
    public float maxIteration;
    public GameObject obj;
}

public class SpawnerGenerator : MonoBehaviour
{
    [SerializeField]
    SpawnerConfig[] configs;

    public DeterministicGrammar g1;
    public GameObject leaf;
    public float trunck_height = 1f;
    public float base_radius = 0.2f;
    public int deltaAngle = 15;
    public float base_angle = 30f;
    public Material trunk_mat;

    Planet planet;
    [HideInInspector]
    public List<Vector3> positions;

    // Start is called before the first frame update
    void Start()
    {
        planet = GetComponent<Planet>();

        Random.State rdState = Random.state;
        foreach (SpawnerConfig config in configs)
        {
            switch(config.randomGenerator)
            {
            case RandomGenerator.RandomLinear:
                RandomLinear(config);
                break;
            }
        }
        Random.state = rdState;
    }

    void RandomLinear(SpawnerConfig config)
    {
        Random.InitState(config.seed);
        positions = new List<Vector3>();

        for (int i = 0; i < config.maxIteration; ++i)
        {
            Vector3 unitSphere = Random.onUnitSphere;
            //Debug.Log("Test : " + unitSphere);
            float elevation = planet.noiseGenerator.GetNoise3D(unitSphere);
            Vector3 pos = unitSphere * (1 + elevation) * planet.radius;

            bool isOk = true;
            foreach(Vector3 opos in positions)
            {
                if (Vector3.Distance(pos, opos) < config.minDistance)
                {
                    isOk = false;
                    break;
                }
            }
            if(isOk)
            {
                positions.Add(pos);
            }
        }
        int count = 0;
        int forestSize = 25;
        int elementSpacing = 3;

        foreach(Vector3 pos in positions)
        {
            if(count % 100 == 0)
            {
                //Debug.Log("OOOOOOOOOOOOOOOOK");
                //float x_old = pos.x, y_old = pos.y;
                //for (int x = 0; x < forestSize; x += elementSpacing)
                //{
                //    for (int z = 0; z < forestSize; z += elementSpacing)
                //    {
                //        Vector3 position = Quaternion.Euler(x, 0, z) * pos;
                //        Vector3 offset = new Vector3(Random.Range(-0.75f, 0.75f), 0f, Random.Range(-0.75f, 0.75f));
                //        Vector3 rotation = new Vector3(Random.Range(0, 5f), Random.Range(0, 360f), Random.Range(0, 5f));
                //        Vector3 scale = Vector3.one * Random.Range(0.75f, 1.25f);



                //        string word = GenerateWord(g1, 2);
                //        WordTo2DTree(word, position, Quaternion.FromToRotation(Vector3.up, (pos + position).normalized), 3);

                //    }
                //}

                GameObject go = GameObject.Instantiate(config.obj);
                go.transform.position = pos;
                go.transform.transform.rotation = Quaternion.FromToRotation(Vector3.up, pos.normalized);

                //Vector3 position = new Vector3(pos.x, pos.y, pos.z);
                //Vector3 offset = new Vector3(Random.Range(-0.75f, 0.75f), 0f, Random.Range(-0.75f, 0.75f));
                //Vector3 rotation = new Vector3(Random.Range(0, 5f), Random.Range(0, 360f), Random.Range(0, 5f));
                //Vector3 scale = Vector3.one * Random.Range(0.75f, 1.25f);

                //string word = GenerateWord(g1, 2);
                //WordTo2DTree(word, position, Quaternion.FromToRotation(Vector3.up, pos.normalized));
            }

            count++;
        }
    }

    public string GenerateWord(DeterministicGrammar g, int iterations)
    {
        string word = g.axiom;
        for (int i = 0; i < iterations; i++)
        {
            //Debug.Log(i);
            string tmp = "";
            foreach (char c in word)
            {
                int j = 0;
                foreach (char symbol in g.V)
                {
                    if (symbol == c)
                        tmp += g.P[j];
                    //Debug.Log(symbol);
                    j++;
                }
                if (c == '+' || c == '-' || c == '[' || c == ']')
                {
                    tmp += c;
                }
            }
            //Debug.Log(tmp);
            word = tmp;
        }
        return word;
    }

    public string GenerateWord(StochasticGrammar g, int iterations)
    {
        string word = g.axiom;
        int prob = 0;
        for (int i = 0; i < iterations; i++)
        {
            string tmp = "";
            foreach (char c in word)
            {
                int j = 0;
                foreach (char symbol in g.V)
                {
                    float rdm = Random.value;
                    float step = 0;
                    foreach (ProbOfRules por in g.probsOfRules)
                    {
                        if (por.symbol == symbol)
                        {
                            for (int k = 0; k < por.probs.Count; k++)
                            {
                                //Debug.Log("k : " + k + ", step : " + step + ", rdm : " + rdm + ", current prob + step : " + (por.probs[k] + step));
                                if (step < rdm && rdm <= por.probs[k] + step)
                                {
                                    //Debug.Log("ooook : " + g.P[j + prob]);
                                    tmp += g.P[j + prob];
                                    break;
                                }
                                step += por.probs[k];
                            }
                        }
                    }
                }
                if (c == '+' || c == '-' || c == '[' || c == ']')
                {
                    tmp += c;
                }
            }
            //Debug.Log("tmp : " + tmp);
            word = tmp;
        }
        return word;
    }

    public void WordTo2DTree(string word, Vector3 pos, Quaternion rotation, float offset)
    {
        //Debug.Log(word);
        Vector3 pivot = pos;
        Stack<Vector3> vectors_stack = new Stack<Vector3>();
        Stack<float> angles_stack = new Stack<float>();
        float angle = 0;
        int i = 0;
        foreach (char c in word)
        {
            //Debug.Log("i : " + i + ", pivot : " + pivot);
            if (c == 'F')
            {
                Cylinder cylinder = new Cylinder(pivot, base_radius, trunck_height, deltaAngle);
                cylinder.CreateCylinder();
                DrawMesh(i + "_mesh", cylinder.vertices, cylinder.uvs, cylinder.triangles, rotation * Quaternion.Euler(0, 0, angle), pivot);
                pivot += rotation * Quaternion.Euler(0, 0, angle) * new Vector3(0, trunck_height, 0);
                ++i;
            }
            else if (c == 'A')
            {
                //Debug.Log("OOOOOK");
                Instantiate(leaf, pivot, Quaternion.Euler(0, 0, angle));
                pivot += rotation * Quaternion.Euler(0, 0, angle) * new Vector3(pivot.x, trunck_height, pivot.z);
                ++i;
            }
            else if (c == 'B')
            {
                //Debug.Log("OK");
                Cylinder cylinder = new Cylinder(pivot, base_radius, trunck_height * 1.5f, deltaAngle);
                cylinder.CreateCylinder();
                DrawMesh(i + "_mesh", cylinder.vertices, cylinder.uvs, cylinder.triangles, rotation * Quaternion.Euler(0, 0, angle), pivot);
                pivot += rotation * Quaternion.Euler(0, 0, angle) * new Vector3(0, trunck_height * 1.5f, 0);
                ++i;
            }
            else if (c == '[')
            {
                vectors_stack.Push(pivot);
                angles_stack.Push(angle);
            }
            else if (c == ']')
            {
                pivot = vectors_stack.Pop();
                angle = angles_stack.Pop();
            }
            else if (c == '+')
            {
                angle += base_angle;
            }
            else if (c == '-')
            {
                angle += -base_angle;
            }
        }
    }
    public void WordTo3DTree(string word, Vector3 pos)
    {
        //Debug.Log(word);
        Vector3 pivot = pos;
        Stack<Vector3> vectors_stack = new Stack<Vector3>();
        Stack<Vector3> angles_stack = new Stack<Vector3>();
        float u_angle = 0;
        float h_angle = 0;
        float l_angle = 0;
        int i = 0;
        foreach (char c in word)
        {
            //Debug.Log("i : " + i + ", pivot : " + pivot);
            if (c == 'F')
            {
                Cylinder cylinder = new Cylinder(pivot, base_radius, trunck_height, deltaAngle);
                cylinder.CreateCylinder();
                DrawMesh(i + "_mesh", cylinder.vertices, cylinder.uvs, cylinder.triangles, Quaternion.Euler(l_angle, u_angle, h_angle), pivot);
                pivot += Quaternion.Euler(l_angle, u_angle, h_angle) * new Vector3(pivot.x, trunck_height, pivot.z);
                ++i;
            }
            else if (c == 'B')
            {
                //Debug.Log("OK");
                Cylinder cylinder = new Cylinder(pivot, base_radius, trunck_height * 1.5f, deltaAngle);
                cylinder.CreateCylinder();
                DrawMesh(i + "_mesh", cylinder.vertices, cylinder.uvs, cylinder.triangles, Quaternion.Euler(l_angle, u_angle, h_angle), pivot);
                pivot += Quaternion.Euler(l_angle, u_angle, h_angle) * new Vector3(pivot.x, trunck_height, pivot.z);
                ++i;
            }
            else if (c == 'A')
            {
                Instantiate(leaf, pivot, Quaternion.Euler(l_angle, u_angle, h_angle));
                pivot += Quaternion.Euler(l_angle, u_angle, h_angle) * new Vector3(pivot.x, trunck_height, pivot.z);
                ++i;
            }
            else if (c == 'S')
            {
                //Debug.Log("OK");
                Cylinder cylinder = new Cylinder(pivot, base_radius, trunck_height * 0.5f, deltaAngle);
                cylinder.CreateCylinder();
                DrawMesh(i + "_mesh", cylinder.vertices, cylinder.uvs, cylinder.triangles, Quaternion.Euler(l_angle, u_angle, h_angle), pivot);
                pivot += Quaternion.Euler(l_angle, u_angle, h_angle) * new Vector3(pivot.x, trunck_height, pivot.z);
                ++i;
            }
            else if (c == '[')
            {
                vectors_stack.Push(pivot);
                angles_stack.Push(new Vector3(l_angle, u_angle, h_angle));
            }
            else if (c == ']')
            {
                pivot = vectors_stack.Pop();
                Vector3 angles = angles_stack.Pop();
                l_angle = angles.x;
                u_angle = angles.y;
                h_angle = angles.z;
            }
            else if (c == '+')
            {
                u_angle += base_angle;
            }
            else if (c == '-')
            {
                u_angle += -base_angle;
            }
            else if (c == '&')
            {
                l_angle += base_angle;
            }
            else if (c == '^')
            {
                l_angle -= base_angle;
            }
            else if (c == '\\')
            {
                h_angle += base_angle;
            }
            else if (c == '/')
            {
                h_angle -= base_angle;
            }
            else if (c == '|')
            {
                u_angle += 180;
            }
        }
    }

    public void DrawMesh(string name, List<Vector3> vertices, List<Vector2> uvs, List<int> triangles, Quaternion q, Vector3 custom_pivot)
    {
        GameObject go = new GameObject(name);
        GameObject go_parent = new GameObject("parent_" + name);
        //Debug.Log("custom_pivot : " + custom_pivot);
        go_parent.transform.position = custom_pivot;
        go.transform.SetParent(go_parent.transform);
        //go.transform.localPosition = Vector3.zero;
        MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = trunk_mat;

        MeshFilter meshFilter = go.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();
        mesh = go.GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.Optimize();
        mesh.RecalculateNormals();
        go_parent.transform.rotation = q;
    }
}
