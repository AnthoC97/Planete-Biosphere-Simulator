using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PBSNoiseConstantModule : PBSNoiseGenerator
{
    private int constantVal;
    public PBSNoiseConstantModule(int constantVal)
    {
        this.constantVal = constantVal;
    }

    public override float GetNoise3D(Vector3 vector)
    {
        return constantVal;
    }
}
