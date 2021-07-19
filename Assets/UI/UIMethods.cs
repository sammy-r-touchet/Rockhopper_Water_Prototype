using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMethods : MonoBehaviour
{
    
    // Cache the WaterManager object to prevent multiple Find() calls.
    private WaterManager _wm;
    private WaterManager wm {
        get {
            if (_wm) {
                return _wm;
            } else {
                _wm = (WaterManager)FindObjectOfType(typeof(WaterManager));
                return _wm;
            }
        }
    }


    public void UpdateWaterWaveCount(float count) 
    {
        if (wm) wm.waveCount = (int)count;
    }

    public void UpdateWaterWaveSteepness(float steep)
    {
        if (wm) wm.steepnessModifier = steep;
    }

    public void UpdateWaterWavelength(float wl)
    {
        if (wm) wm.wavelengthModifier = wl;
    }

}
