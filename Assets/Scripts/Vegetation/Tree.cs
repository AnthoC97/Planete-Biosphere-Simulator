using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree
{
    public float trunc_height;
    public float trunc_radius;
    public int delta_angle;
    //float depth;
    float glucid; //between 0 and  1
    float water; //between 0 and  1
    float minerals; //between 0 and  1
    public List<float> genotype;
    public DeterministicGrammar g;

    public Tree()
    {
        trunc_height = 0;
        trunc_radius = 0;
        delta_angle = 0;
        //depth = 0;
        glucid = 0;
        water = 0;
        minerals = 0;
        genotype = new List<float>();
        genotype.Add(trunc_height);
        genotype.Add(trunc_radius);
        //genotype.Add(depth);
    }

    public Tree(DeterministicGrammar g)
    {
        this.g = g;
        trunc_height = g.trunc_height;
        trunc_radius = g.trunc_radius;
        delta_angle = g.delta_angle;
        glucid = g.glucid;
        water = g.water;
        minerals = g.minerals;
        genotype = new List<float>();
        genotype.Add(trunc_height);
        genotype.Add(trunc_radius);
    }

    void Photosynthesis(int lux)
    {

    }

    void Evapotranspiration(float temperature, float humidity)
    {

    }
}
