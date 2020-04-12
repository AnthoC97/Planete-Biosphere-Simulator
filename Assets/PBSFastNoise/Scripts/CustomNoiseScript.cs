using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class CustomNoiseScript : PBSNoiseScript
{
    [Range(0, 999999)]
    public int seedPlains;

    [Range(0, 10)]
    public float frequencyPlains;

    [Range(0f, 2)]
    public float fractalGainPlains;

    [Range(0, 20)]
    public int octavePlains;

    [Range(0, 10)]
    public float lacunarityPlains;


    [Range(0, 10)]
    public int seedMountains;

    [Range(0, 10)]
    public float frequencyMountains;

    [Range(0, 2)]
    public float fractalGainMountains;

    [Range(0, 20)]
    public int octaveMountains;

    [Range(0, 10)]
    public float lacunarityMountains;


    [Range(0, 999999)]
    public int seedMask;

    [Range(0, 10)]
    public float frequencyMask;

    [Range(0, 2)]
    public float fractalGainMask;

    [Range(0, 20)]
    public int octaveMask;

    [Range(0, 10)]
    public float lacunarityMask;


    [Range(0, 1)]
    public float falloffSelect;

    [Range(-1, 1)]
    public float thresholdSelect;

    [Range(0, 4)]
    public int numStepsSelect;

    public SelectInterpType selectInterpType;

    public FractalNoiseType fractalNoiseTypePlains;
    public FastNoise.Interp interpPlains;
    public FastNoise.FractalType fractalTypePlains;

    public FractalNoiseType fractalNoiseTypeMountains;
    public FastNoise.Interp interpMountains;
    public FastNoise.FractalType fractalTypeMountains;

    public FractalNoiseType fractalNoiseTypeMask;
    public FastNoise.Interp interpMask;
    public FastNoise.FractalType fractalTypeMask;

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
        PBSNoiseGenerator fractalPlaineGenerator = new FractalNoiseGenerator(fractalNoiseTypePlains, seedPlains, frequencyPlains, fractalGainPlains, interpPlains, fractalTypePlains, octavePlains, lacunarityPlains);

        PBSNoiseGenerator fractalMountainsGenerator = new FractalNoiseGenerator(fractalNoiseTypeMountains, seedMountains, frequencyMountains, fractalGainMountains, interpMountains, fractalTypeMountains, octaveMountains, lacunarityMountains);
        //PBSNoiseGenerator scaleBiasGenerator = new ScaleBiasNoiseModule(fractalMountainsGenerator, 100.0f, 0.1f);
        //return scaleBiasGenerator;

        PBSNoiseGenerator maskGenerator = new FractalNoiseGenerator(fractalNoiseTypeMask, seedMask, frequencyMask, fractalGainMask, interpMask, fractalTypeMask, octaveMask, lacunarityMask);

        PBSNoiseGenerator result = new SelectNoiseModule(fractalPlaineGenerator, fractalMountainsGenerator, maskGenerator, selectInterpType, falloffSelect, thresholdSelect, numStepsSelect);
        //return result;

        PBSNoiseGenerator addGenerator = result + 1;
        PBSNoiseGenerator scaleGenerator = new ScaleBiasNoiseModule(addGenerator, 0.5f, 0.0f);

        return scaleGenerator;
    }

    public Gradient gradient = new Gradient();

    public override Color GetColor(Vector3 pointOnUnitSphere) 
    {
        float elevation = GetNoiseGenerator().GetNoise3D(pointOnUnitSphere);
        //return Color.Lerp(Color.red, Color.green, elevation);
        return gradient.Evaluate(elevation);
    }
}
