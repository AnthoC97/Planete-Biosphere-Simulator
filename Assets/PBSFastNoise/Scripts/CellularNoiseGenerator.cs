using UnityEngine;

public class CellularNoiseGenerator : PBSNoiseGenerator
{
    FastNoise fastNoise;
    public CellularNoiseGenerator(int seed = 12345, float frequency = 0.1f, FastNoise.CellularDistanceFunction cellularDistanceFunction = FastNoise.CellularDistanceFunction.Euclidean, FastNoise.CellularReturnType cellularReturnType = FastNoise.CellularReturnType.CellValue)
    {
        fastNoise = new FastNoise(seed);
        fastNoise.SetNoiseType(FastNoise.NoiseType.Cellular);
        fastNoise.SetFrequency(frequency);
        fastNoise.SetCellularDistanceFunction(cellularDistanceFunction);
        fastNoise.SetCellularReturnType(cellularReturnType);
    }

    public override float GetNoise3D(Vector3 vector)
    {
        return fastNoise.GetNoise(vector.x, vector.y, vector.z);
    }
}