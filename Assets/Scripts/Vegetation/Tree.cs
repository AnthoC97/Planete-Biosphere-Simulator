using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree
{
    //chromosome=========
    public float trunc_height;
    public float trunc_radius;
    public int delta_angle;
    //float depth;
    //===================
    //===================
    public float height;
    public float radius;
    //===================
    public float glucid;
    public float water;
    public float minerals;
    public List<float> genotype;
    public DeterministicGrammar g;
    public int reserve;
    public int level;
    public float timeToGrow;

    //public Tree(float timeToGrow)
    //{
    //    this.timeToGrow = timeToGrow;
    //    trunc_height = 0;
    //    trunc_radius = 0;
    //    delta_angle = 0;
    //    //depth = 0;
    //    glucid = 0;
    //    water = 0;
    //    minerals = 0;
    //    genotype = new List<float>();
    //    genotype.Add(trunc_height);
    //    genotype.Add(trunc_radius);
    //    level = 1;
    //    reserve = 0;
    //    //genotype.Add(depth);
    //}

    public Tree(DeterministicGrammar g, float timeToGrow)
    {
        this.timeToGrow = timeToGrow;
        this.g = g;
        trunc_height = g.trunc_height * Random.Range(0.7f, 1.3f);
        trunc_radius = g.trunc_radius * Random.Range(0.7f, 1.3f);
        delta_angle = (int)(g.delta_angle * Random.Range(0.7f, 1.3f));
        glucid = g.glucid;
        water = g.water;
        minerals = g.minerals;
        genotype = new List<float>();
        genotype.Add(trunc_height);
        genotype.Add(trunc_radius);
        genotype.Add(delta_angle);
        level = 1;
        reserve = 0;
    }

    public Tree(DeterministicGrammar g, float timeToGrow, float trunc_height, float trunc_radius, int delta_angle)
    {
        this.timeToGrow = timeToGrow;
        this.g = g;
        this.trunc_height = trunc_height * Random.Range(0.7f, 1.3f);
        this.trunc_radius = trunc_radius * Random.Range(0.7f, 1.3f);
        this.delta_angle = (int)(delta_angle * Random.Range(0.7f, 1.3f));
        glucid = g.glucid;
        water = g.water;
        minerals = g.minerals;
        genotype = new List<float>();
        genotype.Add(trunc_height);
        genotype.Add(trunc_radius);
        genotype.Add(delta_angle);
        level = 1;
        reserve = 0;
    }

    void Photosynthesis(int lux)
    {
        if(water > 0)
        {
            glucid += lux / 15 * (water / 2) + 0.002f * height + 0.002f * radius;
            water = water / 2;
            //Debug.Log("glucid : " + glucid);
            //Debug.Log("water : " + water);
        }
    }

    void Transpiration(float temperature, float humidity)
    {
        water -= 1 / 6 * water;
    }

    public IEnumerator SetReserve()
    {
        for(; ; )
        {
            if(glucid >= 1)
            { 
                Debug.Log("OK");
                glucid -= 1;
                reserve += 1;
            }
            yield return new WaitForSeconds(1f);
        }
        //Debug.Log(time % timeToGrow);
    }

    //renseigner un argument pour timer intervalle pompage
    public IEnumerator GetWater()
    {
        for(; ; )
        {
            water += 0.1f;
            yield return new WaitForSeconds(10f);
        }
    }

    public bool Grow()
    {
        Debug.Log("reserve : " + reserve);
        Debug.Log("glucid : " + glucid);
        if (reserve == level)
        {
            reserve = 0;
            level += 1;
            Debug.Log("reserve : " + reserve);
            Debug.Log("level : " + level);
            return true;
        }
        return false;
    }

    public bool Process(int lux, float temperature, float humidity, float time)
    {
        
        Photosynthesis(lux);
        //Transpiration(temperature, humidity);
        return Grow();
    }
}
