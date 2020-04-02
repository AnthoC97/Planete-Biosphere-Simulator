using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractalNoiseGenerator : PBSNoiseGenerator
{
    FastNoise fastNoise;
    public FractalNoiseGenerator(FractalNoiseType noiseType, int seed, float frequency, float fractalGain, FastNoise.Interp interpolation, FastNoise.FractalType fractalType, int octaves, float lacunarity)
    {
        fastNoise = new FastNoise(seed);
        switch (noiseType)
        {
            case FractalNoiseType.Perlin:
                fastNoise.SetNoiseType(FastNoise.NoiseType.PerlinFractal);
                break;
            case FractalNoiseType.Simplex:
                fastNoise.SetNoiseType(FastNoise.NoiseType.SimplexFractal);
                break;
            case FractalNoiseType.Value:
                fastNoise.SetNoiseType(FastNoise.NoiseType.ValueFractal);
                break;
        }
        fastNoise.SetFrequency(frequency);
        fastNoise.SetFractalGain(fractalGain);
        fastNoise.SetInterp(interpolation);
        fastNoise.SetFractalType(fractalType);
        fastNoise.SetFractalOctaves(octaves);
        fastNoise.SetFractalLacunarity(lacunarity);
    }

    public override float GetNoise3D(Vector3 vector)
    {
        return fastNoise.GetNoise(vector.x, vector.y, vector.z);
    }
}
