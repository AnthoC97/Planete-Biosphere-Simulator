using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomNoiseScript : PBSNoiseScript
{
    public override PBSNoiseGenerator GetNoiseGenerator()
    {
        //return 5000; // Constant Exemple
        return new FractalNoiseGenerator(FastNoise.NoiseType.SimplexFractal, 12345, 2f, 0.5f, FastNoise.Interp.Linear, FastNoise.FractalType.RigidMulti, 6, 1.7f);
    }
}
