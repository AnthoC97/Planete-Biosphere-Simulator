using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticTree : MonoBehaviour
{
    [SerializeField]
    private List<DeterministicGrammar> grammars;
    private List<Tree> trees_genotype;
    private List<GameObject> trees_phenotype;
    private List<Vector3> trees_position;
    private List<Vector3> pos_in_sphere;
    private List<float> trees_score;
    public int forestSize = 25;
    public int elementSpacing = 10;

    public GameObject planet;
    public GameObject leaf;
    public Material trunk_mat;
    public DeterministicGrammar g1;
    private float timer = 0.0f;

    private int treeNb = 0;
    private bool process_or_GA = true;

    private List<Vector3> positions;
    private bool process;
    private bool genetic_algorithm;

    public void Start()
    {
        positions = planet.GetComponent<SpawnerGenerator>().positions;
        int i = 0;
        trees_genotype = new List<Tree>();
        trees_phenotype = new List<GameObject>();
        trees_position = new List<Vector3>();
        pos_in_sphere = new List<Vector3>();
        trees_score = new List<float>();
        int count = 0;
        if (positions.Count > 0)
        {
            foreach(Vector3 pos in positions)
            {
                if (count % 100 == 0)
                {
                    for (int x = 0; x < forestSize; x += elementSpacing)
                    {
                        for (int z = 0; z < forestSize; z += elementSpacing)
                        {
                            treeNb++;
                            int rnd = Random.Range(0, grammars.Count);
                            Tree t = new Tree(grammars[rnd], 10);
                            //        //Debug.Log(t);
                            Vector3 position = Quaternion.Euler(x, 0f, z) * pos;
                            trees_position.Add(position);
                            pos_in_sphere.Add(pos);
                            //        //Vector3 offset = new Vector3(Random.Range(-0.75f, 0.75f), 0f, Random.Range(-0.75f, 0.75f));
                            //        //Vector3 rotation = new Vector3(Random.Range(0, 5f), Random.Range(0, 360f), Random.Range(0, 5f));
                            //        //Vector3 scale = Vector3.one * Random.Range(0.75f, 1.25f);

                            string word = GenerateWord(grammars[rnd], 1);
                            trees_genotype.Add(t);
                            trees_phenotype.Add(WordTo2DTree(word, position, t, "tree_" + i, Quaternion.FromToRotation(Vector3.up, (pos + position).normalized)));
                            StartCoroutine(t.GetWater());
                            StartCoroutine(t.SetReserve());
                            i++;
                        }
                    }
                }
                count++;
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
        if (timer >= 10) { enabled = false; return; }
        //processSelection
        if (timer % 10 < 5)
        {
            process_or_GA = true;
        }
        else
        {
            process_or_GA = false;
        }
        if (process_or_GA)
        {
            for (int i = 0; i < trees_genotype.Count; i++)
            {
                if (trees_genotype[i].Process(15, 20, 40, timer))
                {
                    Vector3 position = trees_position[i];
                    Vector3 pos = pos_in_sphere[i];
                    //Debug.Log("TEST : " + trees_phenotype.Count);
                    Destroy(trees_phenotype[i]);
                    trees_phenotype.RemoveAt(i);
                    string word = GenerateWord(g1, trees_genotype[i].level);
                    trees_phenotype.Insert(i, WordTo2DTree(word, position, trees_genotype[i], "tree_" + i, Quaternion.FromToRotation(Vector3.up, (pos + position).normalized)));
                }
            }
        }
        else
        {
            //Genetic algorithm
            Selection(treeNb / 2);
            Crossover(0.3f);
            ReplaceTrees();
            timer = 0;
            process_or_GA = true;
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

    public GameObject WordTo2DTree(string word, Vector3 pos, Tree tree, string name, Quaternion q)
    {
        float yMin, yMax, xMin, xMax; //y for height, x for width
        //Debug.Log(word);
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
                DrawMesh(i + "_mesh", treeGo, cylinder.vertices, cylinder.uvs, cylinder.triangles, q * Quaternion.Euler(0, 0, angle), pivot);
                old_pivot = pivot;
                pivot += q * Quaternion.Euler(0, 0, angle) * new Vector3(0, tree.trunc_height, 0);
                tmp_trunc_len = tree.trunc_height;
                //Debug.Log("tmp_len : " + tmp_trunc_len);
                ++i;
            }
            else if (c == 'A')
            {
                GameObject _leaf = Instantiate(leaf, pivot, Quaternion.Euler(0, 0, angle));
                _leaf.transform.SetParent(treeGo.transform);
                pivot += q * Quaternion.Euler(0, 0, angle) * new Vector3(pivot.x, tree.trunc_height, pivot.z);
                ++i;
            }
            else if (c == 'B')
            {
                //Debug.Log("OK");
                isBranch = true;
                Cylinder cylinder = new Cylinder(pivot, tree.trunc_radius, tree.trunc_height * 1.5f, tree.delta_angle);
                cylinder.CreateCylinder();
                DrawMesh(i + "_mesh", treeGo, cylinder.vertices, cylinder.uvs, cylinder.triangles, q * Quaternion.Euler(0, 0, angle), pivot);
                old_pivot = pivot;
                pivot += q * Quaternion.Euler(0, 0, angle) * new Vector3(0, tree.trunc_height * 1.5f, 0);
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
        //Debug.Log("Tree height : " + tree.height);
        //Debug.Log("Tree width : " + tree.radius);
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

    public void Selection(int number_selection)
    {
        int count = 0;
        List<Tree> final_population = new List<Tree>();
        List<float> final_score = new List<float>();
        List<Vector3> final_position = new List<Vector3>();
        List<Vector3> final_pos_in_sphere = new List<Vector3>();
        float maxDistance = 0;
        int max_index = 0;
        //Remplir tri score
        foreach(Tree t in trees_genotype)
        {
            trees_score.Add(Fitness(t));
        }
        while (count < number_selection)
        {
            for (int i = 0; i < trees_score.Count; i++)
            {
                if (i == 0)
                {
                    maxDistance = trees_score[i];
                    max_index = i;
                }
                if (trees_score[i] > maxDistance)
                {
                    maxDistance = trees_score[i];
                    max_index = i;
                }
            }
            final_population.Add(trees_genotype[max_index]);
            //d'autres final
            final_position.Add(trees_position[max_index]);
            final_pos_in_sphere.Add(pos_in_sphere[max_index]);
            //final_score.Add(trees_score[max_index]);
            trees_genotype.RemoveAt(max_index);
            trees_position.RemoveAt(max_index);
            pos_in_sphere.RemoveAt(max_index);
            trees_score.RemoveAt(max_index);
            ++count;
        }
        //mettre a jour les tableaux
        trees_genotype = final_population;
        trees_position = final_position;
        pos_in_sphere = final_pos_in_sphere;
        //trees_score = final_score;
    }

    public void Crossover(float alpha)
    {
        List<Tree> final_population = new List<Tree>();

        while(trees_genotype.Count != 0)
        {
            int rnd1 = Random.Range(0, trees_genotype.Count - 1);
            Tree t1 = trees_genotype[rnd1];
            trees_genotype.RemoveAt(rnd1);
            int rnd2 = Random.Range(0, trees_genotype.Count - 1);
            Tree t2 = trees_genotype[rnd2];
            trees_genotype.RemoveAt(rnd2);
            Tree child1, child2;
            float height1, height2, radius1, radius2;
            int delta_angle1, delta_anlge2;
            height1 = alpha * t1.trunc_height + (1 - alpha) * t2.trunc_height;
            //Debug.Log("height : " + height1 + ", t1 height " + t1.height + ", t2 height" + t2.height);
            height2 = alpha * t2.trunc_height + (1 - alpha) * t1.trunc_height;
            radius1 = alpha * t1.trunc_radius + (1 - alpha) * t2.trunc_radius;
            radius2 = alpha * t2.trunc_radius + (1 - alpha) * t1.trunc_radius;
            delta_angle1 = (int)(alpha * t1.delta_angle + (1 - alpha) * t2.delta_angle);
            delta_anlge2 = (int)(alpha * t2.delta_angle + (1 - alpha) * t1.delta_angle);
            child1 = new Tree(t1.g, t1.timeToGrow, height1, radius1, delta_angle1);
            child2 = new Tree(t2.g, t2.timeToGrow, height2, radius2, delta_anlge2);
            final_population.Add(t1);
            final_population.Add(t2);
            final_population.Add(child1);
            final_population.Add(child2);
        }
        trees_genotype = final_population;
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

    public void ReplaceTrees()
    {
        //Debug.Log("tree count : "+trees_position.Count);
        foreach(GameObject g in trees_phenotype)
        {
            Destroy(g);
        }
        trees_phenotype.Clear();
        trees_score.Clear();
        int index = 0; // sert a creer de nouveaux arbres
         //sert a creer les arbres existant
        int n = trees_genotype.Count / 2;
        bool find_existing_tree = false;


        int count = 0;
        if (positions.Count > 0)
        {
            foreach (Vector3 pos in positions)
            {
                if (count % 100 == 0)
                {
                    for (int x = 0; x < forestSize; x += elementSpacing)
                    {
                        for (int z = 0; z < forestSize; z += elementSpacing)
                        {
                            int index_pos = 0;
                            foreach (Vector3 pos_t in trees_position)
                            {
                                index_pos++;
                                if (pos_t.x == (Quaternion.Euler(x, 0f, z) * pos).x && pos_t.z == (Quaternion.Euler(x, 0f, z) * pos).z)
                                {
                                    //Debug.Log("Index : " + index_pos);
                                    string word = GenerateWord(trees_genotype[index_pos].g, trees_genotype[index_pos].level);
                                    trees_phenotype.Add(WordTo2DTree(word, pos_t, trees_genotype[index_pos], "tree_" + index_pos, Quaternion.FromToRotation(Vector3.up, (pos + pos_t).normalized)));
                                    StartCoroutine(trees_genotype[index_pos].GetWater());
                                    StartCoroutine(trees_genotype[index_pos].SetReserve());
                                    find_existing_tree = true;
                                    continue;
                                }
                            }
                            if (find_existing_tree)
                            {
                                Vector3 position = new Vector3(x, 0f, z);
                                string word = GenerateWord(trees_genotype[index + n].g, trees_genotype[index + n].level);
                                trees_phenotype.Add(WordTo2DTree(word, new Vector3(x, 0f, z), trees_genotype[index + n], "tree_" + (index + n), Quaternion.FromToRotation(Vector3.up, (pos + position).normalized)));
                                trees_position.Add(position);
                                pos_in_sphere.Add(pos);
                                StartCoroutine(trees_genotype[index + n].GetWater());
                                StartCoroutine(trees_genotype[index + n].SetReserve());
                                find_existing_tree = false;
                            }
                        }
                    }
                }
            }
        }


    }

    public bool updateBool()
    {
        process_or_GA = !process_or_GA;
        return process_or_GA;
    }
}
