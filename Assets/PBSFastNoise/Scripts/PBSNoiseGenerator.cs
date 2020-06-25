using JetBrains.Annotations;
using MoonSharp.Interpreter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SimpleNoiseType 
{
    Value,
	Perlin, // Gradient
    Simplex,
	WhiteNoise
};
public enum FractalNoiseType
{
    Value,
    Perlin, // Gradient
    Simplex
};

[MoonSharpUserData]
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

    public static PBSNoiseGenerator operator +(PBSNoiseGenerator noiseGenerator1, PBSNoiseGenerator noiseGenerator2) {
        return new AddNoiseModule(noiseGenerator1, noiseGenerator2, 1, 0);
    }
}
