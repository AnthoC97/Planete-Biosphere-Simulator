using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Add two noise generator together
/// </summary>
public class AddNoiseModule : PBSNoiseGenerator
{
    PBSNoiseGenerator inputModule1;
    PBSNoiseGenerator inputModule2;
    PBSNoiseGenerator maskModule;
    float threshold;

    public AddNoiseModule(PBSNoiseGenerator inputModule1, PBSNoiseGenerator inputModule2, PBSNoiseGenerator maskModule, float threshold)
    {
        this.inputModule1 = inputModule1;
        this.inputModule2 = inputModule2;
        this.maskModule = maskModule;
        this.threshold = threshold;
    }

    public override float GetNoise3D(Vector3 vector)
    {
        if (inputModule1==null || inputModule2==null)
        {
            return 0.0f;
        }

        float modifier = 1.0f;
        if (maskModule!=null)
        {
            float mask = maskModule.GetNoise3D(vector);
            if (mask >= threshold)
            {
                modifier = mask;
            }
            else
            {
                return inputModule1.GetNoise3D(vector);
            }
        }

        return modifier * (inputModule1.GetNoise3D(vector) + inputModule2.GetNoise3D(vector));
    }
}
