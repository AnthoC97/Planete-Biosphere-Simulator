using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticTree : MonoBehaviour
{
    [SerializeField]
    private List<DeterministicGrammar> grammars;
    private List<Tree> trees_genotype;
    private List<GameObject> trees_phenotype;
    public int forestSize = 25;
    public int elementSpacing = 3;

    public GameObject leaf;
    public Material trunk_mat;
    public DeterministicGrammar g1;
    private float timer = 0.0f;

    public void Start()
    {
        int i = 0;
        trees_genotype = new List<Tree>();
        trees_phenotype = new List<GameObject>();
        for (int x = 0; x < forestSize; x += elementSpacing)
        {
            for (int z = 0; z < forestSize; z += elementSpacing)
            {
                int rnd = Random.Range(0, grammars.Count);
                Tree t = new Tree(grammars[rnd], 10);
                //        //Debug.Log(t);
                Vector3 position = new Vector3(x, 0f, z);
                //        //Vector3 offset = new Vector3(Random.Range(-0.75f, 0.75f), 0f, Random.Range(-0.75f, 0.75f));
                //        //Vector3 rotation = new Vector3(Random.Range(0, 5f), Random.Range(0, 360f), Random.Range(0, 5f));
                //        //Vector3 scale = Vector3.one * Random.Range(0.75f, 1.25f);

                string word = GenerateWord(grammars[rnd], 1);
                trees_genotype.Add(t);
                trees_phenotype.Add(WordTo2DTree(word, position, t, "tree_" + i));
                StartCoroutine(t.GetWater());
                StartCoroutine(t.SetReserve());
                i++;
            }
        }
        //RemoveTree("tree_1");
        //t = new Tree(g1, 10);
        //string word = GenerateWord(g1, 1);
        //tree = WordTo2DTree(word, Vector3.one, t, "tree");
    }

    void Update()
    {
        timer += Time.deltaTime;
        for(int i = 0; i < trees_genotype.Count; i++)
        {
            if (trees_genotype[i].Process(15, 20, 40, timer))
            {
                Vector3 pos = trees_phenotype[i].transform.position;
                Destroy(trees_phenotype[i]);
                string word = GenerateWord(g1, trees_genotype[i].level);
                trees_phenotype.Insert(i, WordTo2DTree(word, Vector3.one, trees_genotype[i], "tree"));
            }
        }

        //Debug.Log("Level : " + t.level);
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

    public GameObject WordTo2DTree(string word, Vector3 pos, Tree tree, string name)
    {
        float yMin, yMax, xMin, xMax; //y for height, x for width
        Debug.Log(word);
        Vector3 pivot = pos;
        Vector3 old_pivot = Vector3.zero;
        yMin = yMax = pos.y;
        xMin = xMax = pos.x;
        bool isBranch = false;
        Stack<Vector3> vectors_stack = new Stack<Vector3>();
        Stack<float> angles_stack = new Stack<float>();
        float angle = 0;
        int i = 0;
        GameObject treeGo = new GameObject(name);
        foreach (char c in word)
        {
            float tmp_trunc_len = 0;
            //Debug.Log("i : " + i + ", pivot : " + pivot);
            if (c == 'F')
            {
                isBranch = true;
                Cylinder cylinder = new Cylinder(pivot, tree.trunc_radius, tree.trunc_height, tree.delta_angle);
                cylinder.CreateCylinder();
                DrawMesh(i + "_mesh", treeGo, cylinder.vertices, cylinder.uvs, cylinder.triangles, Quaternion.Euler(0, 0, angle), pivot);
                old_pivot = pivot;
                pivot += Quaternion.Euler(0, 0, angle) * new Vector3(0, tree.trunc_height, 0);
                tmp_trunc_len = tree.trunc_height;
                Debug.Log("tmp_len : " + tmp_trunc_len);
                ++i;
            }
            else if (c == 'A')
            {
                GameObject _leaf = Instantiate(leaf, pivot, Quaternion.Euler(0, 0, angle));
                _leaf.transform.SetParent(treeGo.transform);
                pivot += Quaternion.Euler(0, 0, angle) * new Vector3(pivot.x, tree.trunc_height, pivot.z);
                ++i;
            }
            else if (c == 'B')
            {
                //Debug.Log("OK");
                isBranch = true;
                Cylinder cylinder = new Cylinder(pivot, tree.trunc_radius, tree.trunc_height * 1.5f, tree.delta_angle);
                cylinder.CreateCylinder();
                DrawMesh(i + "_mesh", treeGo, cylinder.vertices, cylinder.uvs, cylinder.triangles, Quaternion.Euler(0, 0, angle), pivot);
                old_pivot = pivot;
                pivot += Quaternion.Euler(0, 0, angle) * new Vector3(0, tree.trunc_height * 1.5f, 0);
                tmp_trunc_len = tree.trunc_height * 1.5f;
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
                angle += tree.delta_angle;
            }
            else if (c == '-')
            {
                angle += -tree.delta_angle;
            }
            if(isBranch)
            { 
                float tmp_y = (Mathf.Cos(angle * Mathf.PI / 180) * tmp_trunc_len) + old_pivot.y;
                float tmp_x = (Mathf.Sin(angle * Mathf.PI / 180) * tmp_trunc_len) + old_pivot.x;
                old_pivot = pivot;
                if (tmp_y < yMin)
                    yMin = tmp_y;
                if (tmp_y > yMax)
                    yMax = tmp_y;
                if (tmp_x < xMin)
                    xMin = tmp_x;
                if (tmp_x > xMax)
                    xMax = tmp_x;
            }
            isBranch = false;
        }
        tree.height = Mathf.Abs(yMax - yMin);
        tree.radius = Mathf.Abs(xMax - xMin);
        Debug.Log("Tree height : " + tree.height);
        Debug.Log("Tree width : " + tree.radius);
        return treeGo;
    }

    public void DrawMesh(string name, GameObject treeGo, List<Vector3> vertices, List<Vector2> uvs, List<int> triangles, Quaternion q, Vector3 custom_pivot)
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
        go_parent.transform.SetParent(treeGo.transform);
    }

    public void RemoveTree(string name)
    {
        trees_genotype.RemoveAt((int)name[name.Length-1]);
        trees_phenotype.RemoveAt((int)name[name.Length - 1]);
        Destroy(GameObject.Find(name));
        
    }
    
    //Fonctions spécifiques a l'aglo genetique
    public float Fitness(Tree t)
    {
        float rate = t.height * (1 / getMaxHeight()) +
            t.radius * (1 / getMaxRadius()) +
            t.glucid * (1 / getMaxGlucid()) +
            t.minerals * (1 / getMaxManerals()) +
            t.water * (1 / getMaxWater());

        return rate;
    }

    public float getMaxHeight()
    {
        float max = 0;
        for(int i = 0; i < trees_genotype.Count; i++)
        {
            if (max < trees_genotype[i].height)
                max = trees_genotype[i].height;
        }
        return max;
    }

    public float getMaxRadius()
    {
        float max = 0;
        for(int i = 0; i < trees_genotype.Count; i++)
        {
            if (max < trees_genotype[i].radius)
                max = trees_genotype[i].radius;
        }
        return max;
    }
    public float getMaxWater()
    {
        float max = 0;
        for(int i = 0; i < trees_genotype.Count; i++)
        {
            if (max < trees_genotype[i].water)
                max = trees_genotype[i].water;
        }
        return max;
    }
    public float getMaxGlucid()
    {
        float max = 0;
        for(int i = 0; i < trees_genotype.Count; i++)
        {
            if (max < trees_genotype[i].glucid)
                max = trees_genotype[i].glucid;
        }
        return max;
    }
    public float getMaxManerals()
    {
        float max = 0;
        for(int i = 0; i < trees_genotype.Count; i++)
        {
            if (max < trees_genotype[i].minerals)
                max = trees_genotype[i].minerals;
        }
        return max;
    }
}
