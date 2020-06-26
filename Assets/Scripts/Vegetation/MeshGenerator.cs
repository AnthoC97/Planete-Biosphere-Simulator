using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public DeterministicGrammar g1;
    public StochasticGrammar g2;
    public DeterministicGrammar g3;
    public GameObject leaf;
    public float trunck_height = 1f;
    public float base_radius = 0.2f;
    public int deltaAngle = 15;
    public float base_angle = 30f;
    public Material trunk_mat;

    public int forestSize = 25;
    public int elementSpacing = 3;

    public GameObject[] trees;

    public void Start()
    {

        for (int x = 0; x < forestSize; x+= elementSpacing)
        {
            for (int z = 0; z < forestSize; z+= elementSpacing)
            {
                Vector3 position = new Vector3(x, 0f, z);
                Vector3 offset = new Vector3(Random.Range(-0.75f, 0.75f), 0f, Random.Range(-0.75f, 0.75f));
                Vector3 rotation = new Vector3(Random.Range(0, 5f), Random.Range(0, 360f), Random.Range(0, 5f));
                Vector3 scale = Vector3.one * Random.Range(0.75f, 1.25f);

                string word = GenerateWord(g1, 1);
                WordTo2DTree(word, position);
            }
        }

        //word = GenerateWord(g2, 3);
        //WordTo2DTree(word, new Vector3(5, 0, 0));
        
        //word = GenerateWord(g3, 1);
        //WordTo3DTree(word, Vector3.zero);
        //for (int i = 0; i < 50; i++)
        //{
        //    float x = Random.Range(-28, 28);
        //    float z = Random.Range(-28, 28);
        //    WordToTree(word, new Vector3(x, 0, z));
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GenerateWord(DeterministicGrammar g,int iterations)
    {
        string word = g.axiom;
        for(int i = 0; i < iterations; i++)
        {
            Debug.Log(i);
            string tmp = "";
            foreach(char c in word)
            {
                int j = 0;
                foreach (char symbol in g.V)
                {
                    if (symbol == c)
                        tmp += g.P[j];
                    Debug.Log(symbol);
                    j++;
                }
                if(c == '+' || c == '-' || c == '[' || c == ']')
                {
                    tmp += c;
                }
            }
            //Debug.Log(tmp);
            word = tmp;
        }
        return word;
    }

    public string GenerateWord(StochasticGrammar g,int iterations)
    {
        string word = g.axiom;
        int prob = 0;
        for(int i = 0; i < iterations; i++)
        {
            string tmp = "";
            foreach(char c in word)
            {
                int j = 0;
                foreach (char symbol in g.V)
                {
                    float rdm = Random.value;
                    float step = 0;
                    foreach(ProbOfRules por in g.probsOfRules)
                    {
                        if(por.symbol == symbol)
                        {
                            for (int k = 0; k < por.probs.Count; k++)
                            {
                                Debug.Log("k : " + k + ", step : " + step + ", rdm : " + rdm + ", current prob + step : " + (por.probs[k] + step));
                                if (step < rdm && rdm <= por.probs[k] + step)
                                {
                                    Debug.Log("ooook : " + g.P[j + prob]);
                                    tmp += g.P[j + prob];
                                    break;
                                }
                                step += por.probs[k];
                            }
                        }
                    }
                }
                if(c == '+' || c == '-' || c == '[' || c == ']')
                {
                    tmp += c;
                }
            }
            Debug.Log("tmp : " + tmp);
            word = tmp;
        }
        return word;
    }

    public void WordTo2DTree(string word, Vector3 pos)
    {
        Debug.Log(word);
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
                DrawMesh(i + "_mesh", cylinder.vertices, cylinder.uvs, cylinder.triangles, Quaternion.Euler(0, 0, angle), pivot);
                pivot += Quaternion.Euler(0, 0, angle) * new Vector3(0,trunck_height, 0);
                ++i;
            }
            else if (c == 'A')
            {
                Debug.Log("OOOOOK");
                Instantiate(leaf, pivot, Quaternion.Euler(0, 0, angle));
                pivot += Quaternion.Euler(0, 0, angle) * new Vector3(pivot.x, trunck_height, pivot.z);
                ++i;
            }
            else if(c == 'B')
            {
                Debug.Log("OK");
                Cylinder cylinder = new Cylinder(pivot, base_radius, trunck_height*1.5f, deltaAngle);
                cylinder.CreateCylinder();
                DrawMesh(i + "_mesh", cylinder.vertices, cylinder.uvs, cylinder.triangles, Quaternion.Euler(0, 0, angle), pivot);
                pivot += Quaternion.Euler(0, 0, angle) * new Vector3(0, trunck_height*1.5f, 0);
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
            else if(c == '+')
            {
                angle += base_angle;
            }
            else if(c == '-')
            {
                angle += -base_angle;
            }
        }
    }
    public void WordTo3DTree(string word, Vector3 pos)
    {
        Debug.Log(word);
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
                pivot += Quaternion.Euler(l_angle, u_angle, h_angle) * new Vector3(pivot.x,trunck_height, pivot.z);
                ++i;
            }
            else if(c == 'B')
            {
                Debug.Log("OK");
                Cylinder cylinder = new Cylinder(pivot, base_radius, trunck_height*1.5f, deltaAngle);
                cylinder.CreateCylinder();
                DrawMesh(i + "_mesh", cylinder.vertices, cylinder.uvs, cylinder.triangles, Quaternion.Euler(l_angle, u_angle, h_angle), pivot);
                pivot += Quaternion.Euler(l_angle, u_angle, h_angle) * new Vector3(pivot.x, trunck_height, pivot.z);
                ++i;
            }
            else if(c == 'A')
            {
                Instantiate(leaf, pivot, Quaternion.Euler(l_angle, u_angle, h_angle));
                pivot += Quaternion.Euler(l_angle, u_angle, h_angle) * new Vector3(pivot.x, trunck_height, pivot.z);
                ++i;
            }
            else if(c == 'S')
            {
                Debug.Log("OK");
                Cylinder cylinder = new Cylinder(pivot, base_radius, trunck_height*0.5f, deltaAngle);
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
            else if(c == '+')
            {
                u_angle += base_angle;
            }
            else if(c == '-')
            {
                u_angle += -base_angle;
            }
            else if(c == '&')
            {
                l_angle += base_angle;
            }
            else if(c == '^')
            {
                l_angle -= base_angle;
            }
            else if(c == '\\')
            {
                h_angle += base_angle;
            }
            else if(c == '/')
            {
                h_angle -= base_angle;
            }
            else if(c == '|')
            {
                u_angle += 180;
            }
        }
    }
        
    public void DrawMesh(string name, List<Vector3> vertices, List<Vector2> uvs, List<int> triangles, Quaternion q, Vector3 custom_pivot) 
    {
        GameObject go = new GameObject(name);
        GameObject go_parent = new GameObject("parent_"+name);
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

    private void CreateCube()
    {
        Vector3[] vertices = {
            new Vector3 (0, 0, 0),
            new Vector3 (1, 0, 0),
            new Vector3 (1, 1, 0),
            new Vector3 (0, 1, 0),
            new Vector3 (0, 1, 1),
            new Vector3 (1, 1, 1),
            new Vector3 (1, 0, 1),
            new Vector3 (0, 0, 1),
        };

        int[] triangles = {
            0, 2, 1, //face front
			0, 3, 2,
            2, 3, 4, //face top
			2, 4, 5,
            1, 2, 5, //face right
			1, 5, 6,
            0, 7, 4, //face left
			0, 4, 3,
            5, 4, 7, //face back
			5, 7, 6,
            0, 6, 7, //face bottom
			0, 1, 6
        };

        MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Diffuse"));

        MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();

        Mesh mesh = new Mesh();
        mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.Optimize();
        mesh.RecalculateNormals();
    }
}
