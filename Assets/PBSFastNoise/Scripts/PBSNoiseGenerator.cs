using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PBSNoiseGenerator
{
    public virtual float GetNoise3D(Vector3 vector)
    {
        return 0;
    }

    public static implicit operator PBSNoiseGenerator(int i)
    {
        return new ConstantNoiseModule(i);
    }
}
