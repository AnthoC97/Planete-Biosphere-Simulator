using MoonSharp.Interpreter;
using UnityEngine;

[MoonSharpUserData]
public class CustomNoiseScript : PBSNoiseScript
{
    // Plains
    [Range(0, 999999)]
    public int seedPlains;

    [Range(0, 10)]
    public float frequencyPlains;

    //[Range(0f, 2)]
    public float fractalGainPlains;

    //[Range(0, 20)]
    public int octavePlains;

    //[Range(0, 10)]
    public float lacunarityPlains;

    // Mountains
    [Range(0, 10)]
    public int seedMountains;

    [Range(0, 10)]
    public float frequencyMountains;

    //[Range(0, 2)]
    public float fractalGainMountains;

    //[Range(0, 20)]
    public int octaveMountains;

    //[Range(0, 10)]
    public float lacunarityMountains;


    // Select mask
    [Range(0, 999999)]
    public int seedMask;

    [Range(0, 10)]
    public float frequencyMask;

    //[Range(0, 2)]
    public float fractalGainMask;

    //[Range(0, 20)]
    public int octaveMask;

    //[Range(0, 10)]
    public float lacunarityMask;


    //[Range(0, 1)]
    public float falloffSelect;

    //[Range(-1, 1)]
    public float thresholdSelect;

    //[Range(0, 4)]
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

    /*[Range(0, 999999)]
    public int seedCellular;

    [Range(0, 10)]
    public float frequencyCellular;

    public FastNoise.CellularDistanceFunction cellularDistanceFunc;
    [Min(1)]
    public FastNoise.CellularReturnType cellularReturnType;

    // Select cell mask
    [Range(0, 999999)]
    public int seedCellMask;

    [Range(0, 10)]
    public float frequencyCellMask;

    public FractalNoiseType fractalNoiseTypeCellMask;
    public FastNoise.Interp interpCellMask;
    public FastNoise.FractalType fractalTypeCellMask;

    //[Range(0, 1)]
    public float falloffSelectCell;

    //[Range(-1, 1)]
    public float thresholdSelectCell;

    //[Range(0, 4)]
    public int numStepsSelectCell;

    public SelectInterpType selectInterpTypeCell;*/


    public override PBSNoiseGenerator GetNoiseGenerator()
    {
        //return 5000; // Constant Exemple

        // Add Exemple
        /*PBSNoiseGenerator a = 4;
        PBSNoiseGenerator b = 6;
        return a + b;*/

        /*PBSNoiseGenerator fractalGenerator = new FractalNoiseGenerator(FractalNoiseType.Simplex, 12345, 2f, 0.5f, FastNoise.Interp.Linear, FastNoise.FractalType.RigidMulti, 6, 1.7f);
        return fractalGenerator;

        PBSNoiseGenerator gen = fractalGenerator + 4;

        PBSNoiseGenerator scaleModule = new ScaleBiasNoiseModule(fractalGenerator, 10, 0); */

        // Make a fractal noise for the plains
        PBSNoiseGenerator fractalPlainGenerator = new FractalNoiseGenerator(fractalNoiseTypePlains, seedPlains, frequencyPlains, fractalGainPlains, interpPlains, fractalTypePlains, octavePlains, lacunarityPlains);
        // Make a fractal noise for the mountains
        PBSNoiseGenerator fractalMountainsGenerator = new FractalNoiseGenerator(fractalNoiseTypeMountains, seedMountains, frequencyMountains, fractalGainMountains, interpMountains, fractalTypeMountains, octaveMountains, lacunarityMountains);
        // Make a fractal noise for the mask used to select between plains and mountains
        PBSNoiseGenerator maskGenerator = new FractalNoiseGenerator(fractalNoiseTypeMask, seedMask, frequencyMask, fractalGainMask, interpMask, fractalTypeMask, octaveMask, lacunarityMask);
        // Select and can interpolate between plains and mountains with the mask
        PBSNoiseGenerator result = new SelectNoiseModule(fractalPlainGenerator, fractalMountainsGenerator, maskGenerator, selectInterpType, falloffSelect, thresholdSelect, numStepsSelect);

        // Exemple of cellular noise 
        /*PBSNoiseGenerator cellular = new CellularNoiseGenerator(seedCellular, frequencyCellular, cellularDistanceFunc, cellularReturnType);
        PBSNoiseGenerator maskCellGenerator = new FractalNoiseGenerator(fractalNoiseTypeCellMask, seedCellMask, frequencyCellMask, 0, interpCellMask, fractalTypeCellMask, 0, 0);
        */

        // Make the result clamp between 0 and 1
        PBSNoiseGenerator zeroOneResult = new ZeroOneNoiseModule(result);

        // Exemple of domain warping
        //PBSNoiseGenerator warpGen = new WarpModule(zeroOneResult, zeroOneResult, 0.5f, WarpIterationsType.Two);

        // Multiply the result by 0.2 and add 0
        PBSNoiseGenerator scaleBiasGenerator = new ScaleBiasNoiseModule(zeroOneResult, 0.2f, 0f);
        return scaleBiasGenerator;
    }

    public Gradient gradient = new Gradient();

    public override Color GetColor(Vector3 pointOnUnitSphere) 
    {
        float elevation = GetNoiseGenerator().GetNoise3D(pointOnUnitSphere);
        //return Color.Lerp(Color.red, Color.green, elevation);
        /*if (float.IsInfinity(elevation) || float.IsNaN(elevation))
            elevation = 0;*/
        if(elevation < 0 || elevation > 1)
        print(elevation);
        return gradient.Evaluate(elevation/*Mathf.Clamp(elevation, 0.0f, 1.0f)*/);
    }
}