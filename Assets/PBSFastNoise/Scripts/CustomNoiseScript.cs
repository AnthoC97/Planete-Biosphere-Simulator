using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomNoiseScript : PBSNoiseScript
{
    [Range(0, 999999)]
    public int seed1;

    [Range(0, 999999)]
    public int seed2;

    [Range(0, 999999)]
    public int seed3;

    public override PBSNoiseGenerator GetNoiseGenerator()
    {
        //return 5000; // Constant Exemple

        // Add Exemple
        /*PBSNoiseGenerator a = 4;
        PBSNoiseGenerator b = 6;
        return a + b;*/

        /*PBSNoiseGenerator fractalGenerator = new FractalNoiseGenerator(FractalNoiseType.Simplex, 12345, 2f, 0.5f, FastNoise.Interp.Linear, FastNoise.FractalType.RigidMulti, 6, 1.7f);
        //return fractalGenerator;

        PBSNoiseGenerator gen = fractalGenerator + 4;

        PBSNoiseGenerator scaleModule = new ScaleBiasNoiseModule(fractalGenerator, 10, 0); // scale fractal by 10
        return scaleModule;*/
        PBSNoiseGenerator fractalPlaineGenerator = new FractalNoiseGenerator(FractalNoiseType.Perlin, seed1, 0.8f, 0.5f, FastNoise.Interp.Hermite, FastNoise.FractalType.FBM, 3, 1.0f);

        PBSNoiseGenerator fractalMountainsGenerator = new FractalNoiseGenerator(FractalNoiseType.Perlin, seed2, 1.0f, 0.5f, FastNoise.Interp.Hermite, FastNoise.FractalType.FBM, 6, 2.0f);
        //PBSNoiseGenerator scaleBiasGenerator = new ScaleBiasNoiseModule(fractalMountainsGenerator, 100.0f, 0.1f);
        //return scaleBiasGenerator;

        PBSNoiseGenerator maskGenerator = new FractalNoiseGenerator(FractalNoiseType.Perlin, seed3, 1.6f, 0.5f, FastNoise.Interp.Hermite, FastNoise.FractalType.FBM, 1, 1.8f);
        //PBSNoiseGenerator scaleMaskGenerator = new ScaleBiasNoiseModule(maskGenerator, 100.0f, 0.1f);

        return new SelectNoiseModule(fractalPlaineGenerator, fractalMountainsGenerator, maskGenerator, SelectInterpType.SineInOut, 0.5f, 0.0f, 4);

        //return scaleMaskGenerator;
    }
}
