using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SelectInterpType
{
    None,
	CircularIn,
	CircularOut,
	CircularInOut,
	ExponentialIn,
	ExponentialOut,
	ExponentialInOut,
	SineIn,
	SineOut,
	SineInOut,
	Step,
	Linear
};

public struct Interpolation
{
    public static float InterpCircularIn(float A, float B, float Alpha)
    {
        float ModifiedAlpha = -1.0f * (Mathf.Sqrt(1.0f - Alpha * Alpha) - 1.0f);
        return Mathf.Lerp(A, B, ModifiedAlpha);
    }

    public static float InterpCircularOut(float A, float B, float Alpha)
	{
        Alpha -= 1.0f;
		float ModifiedAlpha = Mathf.Sqrt(1.0f - Alpha * Alpha);
		return Mathf.Lerp(A, B, ModifiedAlpha);
	}

    public static float InterpCircularInOut(float A, float B, float Alpha)
	{
		return Mathf.Lerp(A, B, (Alpha< 0.5f) ?
            InterpCircularIn(0.0f, 1.0f, Alpha* 2.0f) * 0.5f :
			InterpCircularOut(0.0f, 1.0f, Alpha* 2.0f - 1.0f) * 0.5f + 0.5f);
	}

    public static float InterpExpoIn(float A, float B, float Alpha)
	{

        float ModifiedAlpha = (Alpha == 0.0f) ? 0.0f : Mathf.Pow(2.0f, 10.0f * (Alpha - 1.0f));
		return Mathf.Lerp(A, B, ModifiedAlpha);
	}

	public static float InterpExpoOut(float A, float B, float Alpha)
    {
        float ModifiedAlpha = (Alpha == 1.0f) ? 1.0f : -Mathf.Pow(2.0f, -10.0f * Alpha) + 1.0f;
        return Mathf.Lerp(A, B, ModifiedAlpha);
    }
	public static float InterpExpoInOut(float A, float B, float Alpha)
    {
        return Mathf.Lerp(A, B, (Alpha < 0.5f) ?
            InterpExpoIn(0.0f, 1.0f, Alpha * 2.0f) * 0.5f :
            InterpExpoOut(0.0f, 1.0f, Alpha * 2.0f - 1.0f) * 0.5f + 0.5f);
    }
	public static float InterpSinIn(float A, float B, float Alpha)
	{
        float  ModifiedAlpha = -1.0f * Mathf.Cos(Alpha * Mathf.PI*0.5f) + 1.0f;
		return Mathf.Lerp(A, B, ModifiedAlpha);
	}
	public static float InterpSinOut(float A, float B, float Alpha)
    {
        float ModifiedAlpha = Mathf.Sin(Alpha * Mathf.PI*0.5f);
        return Mathf.Lerp(A, B, ModifiedAlpha);
    }
	public static float InterpSinInOut(float A, float B, float Alpha)
    {
        return Mathf.Lerp(A, B, (Alpha< 0.5f) ?
            InterpSinIn(0.0f, 1.0f, Alpha* 2.0f) * 0.5f :
            InterpSinOut(0.0f, 1.0f, Alpha* 2.0f - 1.0f) * 0.5f + 0.5f);
    }

    public static float InterpStep(float A, float B, float Alpha, int Steps)
	{
		if (Steps <= 1 || Alpha <= 0)
		{
			return A;
		}
		else if (Alpha >= 1)
		{
			return B;
		}

		float StepsAsFloat = Steps;
        float NumIntervals = StepsAsFloat - 1.0f;
        float ModifiedAlpha = Mathf.Floor(Alpha * StepsAsFloat) / NumIntervals;
        return Mathf.Lerp(A, B, ModifiedAlpha);
	}
}

/// <summary>
/// Select from two different noise generator and use the third as a mask and can use interpolation
/// </summary>
public class SelectNoiseModule : PBSNoiseGenerator
{
    PBSNoiseGenerator inputModule1;
    PBSNoiseGenerator inputModule2; 
    PBSNoiseGenerator selectModule;
    SelectInterpType interpolationType; 
    float falloff; 
    float threshold; 
    int numSteps;

    public SelectNoiseModule(PBSNoiseGenerator inputModule1, PBSNoiseGenerator inputModule2, PBSNoiseGenerator selectModule, SelectInterpType interpolationType, float falloff, float threshold, int numSteps)
	{
        this.inputModule1 = inputModule1;
        this.inputModule2 = inputModule2;
        this.selectModule = selectModule;
        this.falloff = falloff;
        this.threshold = threshold;
        this.interpolationType = interpolationType;
        this.numSteps = numSteps;
    }

    public override float GetNoise3D(Vector3 vector)
    {
		if (inputModule1==null && inputModule2==null && selectModule==null)
        {
            return 0.0f;
        }

        float control = selectModule.GetNoise3D(vector);

        if (interpolationType != SelectInterpType.None)
        {
            // outside lower falloff bound
            if (control <= (threshold - falloff))
            {
                return inputModule2.GetNoise3D(vector);
            }
            // outside upper falloff bound
            else if (control >= (threshold + falloff))
            {
                return inputModule1.GetNoise3D(vector);
            }
            // otherwise must be inside the threshold bounds, so smooth it
            else
            {
                switch (interpolationType)
                {
                    case SelectInterpType.CircularIn:
                        return Interpolation.InterpCircularIn(inputModule2.GetNoise3D(vector), inputModule1.GetNoise3D(vector), (control - (threshold - falloff) / (2.0f * falloff)));
                    case SelectInterpType.CircularInOut:
                        return Interpolation.InterpCircularInOut(inputModule2.GetNoise3D(vector), inputModule1.GetNoise3D(vector), (control - (threshold - falloff) / (2.0f * falloff)));
                    case SelectInterpType.CircularOut:
                        return Interpolation.InterpCircularOut(inputModule2.GetNoise3D(vector), inputModule1.GetNoise3D(vector), (control - (threshold - falloff) / (2.0f * falloff)));
                    case SelectInterpType.ExponentialIn:
                        return Interpolation.InterpExpoIn(inputModule2.GetNoise3D(vector), inputModule1.GetNoise3D(vector), (control - (threshold - falloff) / (2.0f * falloff)));
                    case SelectInterpType.ExponentialInOut:
                        return Interpolation.InterpExpoInOut(inputModule2.GetNoise3D(vector), inputModule1.GetNoise3D(vector), (control - (threshold - falloff) / (2.0f * falloff)));
                    case SelectInterpType.ExponentialOut:
                        return Interpolation.InterpExpoOut(inputModule2.GetNoise3D(vector), inputModule1.GetNoise3D(vector), (control - (threshold - falloff) / (2.0f * falloff)));
                    case SelectInterpType.SineIn:
                        return Interpolation.InterpSinIn(inputModule2.GetNoise3D(vector), inputModule1.GetNoise3D(vector), (control - (threshold - falloff) / (2.0f * falloff)));
                    case SelectInterpType.SineInOut:
                        return Interpolation.InterpSinInOut(inputModule2.GetNoise3D(vector), inputModule1.GetNoise3D(vector), (control - (threshold - falloff) / (2.0f * falloff)));
                    case SelectInterpType.SineOut:
                        return Interpolation.InterpSinOut(inputModule2.GetNoise3D(vector), inputModule1.GetNoise3D(vector), (control - (threshold - falloff) / (2.0f * falloff)));
                    case SelectInterpType.Step:
                        return Interpolation.InterpStep(inputModule2.GetNoise3D(vector), inputModule1.GetNoise3D(vector), (control - (threshold - falloff) / (2.0f * falloff)), numSteps);
                    case SelectInterpType.Linear:
                        return Mathf.Lerp(inputModule2.GetNoise3D(vector), inputModule1.GetNoise3D(vector), (control - (threshold - falloff) / (2.0f * falloff)));
                }
            }
        }

        // If there's no interpolation, easy mode
        if (control > threshold)
        {
            return inputModule1.GetNoise3D(vector);
        }
        else
        {
            return inputModule2.GetNoise3D(vector);
        }
    }
}