using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PBSNoiseScript : MonoBehaviour
{
    public virtual PBSNoiseGenerator GetNoiseGenerator()
    {
        return new PBSNoiseGenerator();
    }
}
