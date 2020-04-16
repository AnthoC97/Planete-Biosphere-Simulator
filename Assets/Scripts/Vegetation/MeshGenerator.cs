using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public Grammar g1;
    public GameObject go;
    public float trunck_height = 1f;
    public float base_radius = 0.2f;
    public int deltaAngle = 15;
    public float base_angle = 30f;

    public void Start()
    {
        string word = GenerateWord(g1, 3);
        WordToTree(word);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GenerateWord(Grammar g,int iterations)
    {
        string word = g.axiom;
        for(int i = 0; i < iterations; i++)
        {
            string tmp = "";
            foreach(char c in word)
            {
                int j = 0;
                foreach (char symboml in g.V)
                {
                    if (symboml == c)
                        tmp += g.P[j];
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

    public void WordToTree(string word)
    {
        Debug.Log(word);
        Vector3 pivot = Vector3.zero;
        Stack<Vector3> vectors_stack = new Stack<Vector3>();
        Stack<float> angles_stack = new Stack<float>();
        bool sub = false;
        float angle = 0;
        int i = 0;
        foreach (char c in word)
        {
            Debug.Log("i : " + i + ", pivot : " + pivot);
            if (c == 'F')
            {
                Cylinder cylinder = new Cylinder(pivot, base_radius, trunck_height, deltaAngle);
                cylinder.CreateCylinder();
                DrawMesh(i + "_mesh", cylinder.vertices, cylinder.uvs, cylinder.triangles, Quaternion.Euler(0, 0, angle), pivot);
                pivot += Quaternion.Euler(0, 0, angle) * new Vector3(0,trunck_height, 0);
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
        
    public void DrawMesh(string name, List<Vector3> vertices, List<Vector2> uvs, List<int> triangles, Quaternion q, Vector3 custom_pivot) 
    {
        GameObject go = new GameObject(name);
        GameObject go_parent = new GameObject("parent_"+name);
        //Debug.Log("custom_pivot : " + custom_pivot);
        go_parent.transform.position = custom_pivot;
        go.transform.SetParent(go_parent.transform);
        //go.transform.localPosition = Vector3.zero;
        MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = new Material(Shader.Find("Diffuse"));

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
