using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterManager : MonoBehaviour
{

    private Material waterMat;

    // Wave Generation Variables
    [Range(0,20)]public int waveCount = 20;
    [Range(0,4)]public float steepnessModifier = 1.5f;
    [Range(0.5f,5)]public float wavelengthModifier = 2;

    // Keep scale info for wave formation.
    private float waterTileScale;


    // Stored wave values for use in shader
    // Values are [ slope.x, slope.z, amplitude, wavelength ]
    Vector4[] normWaves = new Vector4[20];


    // Start is called before the first frame update
    void Start()
    {
        // Update slider variables
        waterMat.SetInt("_waveCount", waveCount);
        waterMat.SetFloat("_steepnessModifier", steepnessModifier);
        waterMat.SetFloat("_wavelengthModifier", wavelengthModifier);

        SetupWaves();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateWaves();
    }

    // Passed variables from water generator.
    // Allows for setting water properties prior to initialization.
    public void SetProperties(Material wMat, int wvCount, float steepnessMod, float wavelengthMod, float scale){
        waterMat = wMat;
        waveCount = wvCount;
        steepnessModifier = steepnessMod;
        wavelengthModifier = wavelengthMod;
        waterTileScale = scale;
    }

    // Generates wave variables, then updates the material to reflect the new values.
    // Waves are also normalized in scale to prevent self-intersections.
    void SetupWaves()
    {
        Vector4[] waves = new Vector4[100];
        for (int i=0;i<20;i++)
        {
            waves[i] = GenerateWave();
        }
        normWaves = NormalizeWaveAmps(waves);

        UpdateWaves();
        waterMat.SetVectorArray("_waves", normWaves);
    }

    // Update the material with water properties.
    void UpdateWaves()
    {
        waterMat.SetInt("_waveCount", waveCount);
        waterMat.SetFloat("_steepnessModifier", steepnessModifier);
        waterMat.SetFloat("_wavelengthModifier", wavelengthModifier);
    }
    
    // Creates a single randomly generated wave.
    Vector4 GenerateWave()
    {
        float theta = Random.Range(0,Mathf.PI * 2);
        return new Vector4(
            Mathf.Cos(theta),
            Mathf.Sin(theta),
            Random.Range(.001f,2),
            Random.Range(1f,10f)
        );
    }

    // Scales water amplitudes down to prevent self-intersection.
    Vector4[] NormalizeWaveAmps(Vector4[] waves)
    {
        Vector4[] tmp = waves;
        float ampSum = 0;
        for (int i=0;i<tmp.Length;i++)
        {
            ampSum += tmp[i].z;
        }
        if (ampSum > 1/waterTileScale)
        {
            for (int i=0;i<tmp.Length;i++)
            {
                tmp[i].z = tmp[i].z / (ampSum * waterTileScale);
            }
        }
        return tmp;
    }
}
