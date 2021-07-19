using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


/// <summary>
/// This object creates a hierarchy of water objects, as well as a water manager object that can be used to handle water properties during runtime.
/// This object destroys itself after loading.
/// </summary>

public class WaterGenerator : MonoBehaviour
{

    // Variables describing how water generates.
    [Header("Water Generation")]
    [Range(0,200)] public float waterRadius = 100;
    [Range(0,20)]  public float waterTileScale = 10;

    // Variables describing size and shape of water. Passed to Water Manager.
    [Header("Water Properties")]
    [Range(0,20)]public int waveCount = 20;
    [Range(0,4)]public float steepnessModifier = 1.5f;
    [Range(0.5f,5)]public float wavelengthModifier = 2;


    // Material used in water tiles. Passed to Water Manager.
    public Material waterMat;

    // Start is called before the first frame update
    void Start()
    {
        // Generate water object tree
        Transform Water = (new GameObject("Water")).transform;
        Transform waterManager = (new GameObject("WaterManager")).transform;
        Transform waterTiles = (new GameObject("WaveTiles")).transform;
        waterManager.SetParent(Water);
        waterTiles.SetParent(Water);

        // Create template water tile.
        Transform waterTileTemplate = TileGenerator.GenerateTile(10, waterMat);

        // Generate water tiles from template and add to hierarchy.
        var edgeCount = (int)(waterRadius / waterTileScale);
        var offset = new Vector3(edgeCount/2, 0, edgeCount/2);

        for (int i=0; i<edgeCount; i++)
        {
            for (int j=0; j<edgeCount; j++)
            {
                Instantiate(waterTileTemplate, new Vector3(i, 0, j), waterTileTemplate.rotation, waterTiles);
            }
        }

        // Scale Tiles to desired size.
        waterTiles.localScale = new Vector3(waterTileScale, 1, waterTileScale);

        // Setup WaterManager component and set its properties.
        waterManager.gameObject.AddComponent<WaterManager>().SetProperties(waterMat, waveCount, steepnessModifier, wavelengthModifier, waterTileScale);

        // Cleanup Objects
        Destroy(waterTileTemplate.gameObject);
        Destroy(gameObject);
    }
}
