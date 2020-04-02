using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractalNoiseGenerator : PBSNoiseGenerator
{
    FastNoise fastNoise;
    public FractalNoiseGenerator(FastNoise.NoiseType noiseType, int seed, float frequency, float fractalGain, FastNoise.Interp interpolation, FastNoise.FractalType fractalType, int octaves, float lacunarity)
    {
        fastNoise = new FastNoise(seed);
        fastNoise.SetNoiseType(noiseType);
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
