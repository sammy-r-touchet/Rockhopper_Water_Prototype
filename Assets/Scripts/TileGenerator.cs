using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// Creates a single water tile to create a prefab from.
/// Tile is a unit square in size and shape.
/// Tile 
/// </summary>

public class TileGenerator : MonoBehaviour
{

    public static Transform GenerateTile( int tileSideEdges, Material tileMat)
    {
        var tileEdgeLength = 1.0f / tileSideEdges;

        // Generate tile object and components.
        var tileObject = new GameObject();
        tileObject.name = "New Tile";
        var meshRenderer = tileObject.AddComponent<MeshRenderer>();
        var meshFilter = tileObject.AddComponent<MeshFilter>();

        // Apply the tile material.
        meshRenderer.sharedMaterial = tileMat;

        // Generate the tile's mesh.
        var mesh = new Mesh();
        mesh.vertices = GenerateVerts(tileSideEdges, tileEdgeLength);
        mesh.triangles = GenerateTris(tileSideEdges);
        mesh.uv = GenerateUVs(mesh.vertices);
        meshFilter.mesh = mesh;

        return tileObject.transform;
    }

    // Generates a vertices list based on the number of edges.
    private static Vector3[] GenerateVerts( int tileSideEdges, float tileEdgeLength ) {
        
        // Array for storing vertices.
        var outputVerts = new Vector3[(tileSideEdges+1)*(tileSideEdges+1)];

        // Offset vector so vertices are centered around the origin.
        var offset = new Vector3(-0.5f, 0, -0.5f);

        // Generate each vertex and add to the array.
        for (int i=0; i<=tileSideEdges; i++)
        {
            for (int j=0; j<=tileSideEdges; j++)
            {
                outputVerts[ i * (tileSideEdges + 1) + j ] = new Vector3(j * tileEdgeLength, 0, i * tileEdgeLength) + offset;
            }
        }

        return outputVerts;
    }

    // Generates a triangles list based on the number of edges.
    private static int[] GenerateTris( int tileSideEdges ) {

        // Array for storing triangles
        var outputTris = new int[tileSideEdges * tileSideEdges * 6];

        // Counter to keep track of current location in array;
        var index = 0;

        // Generates two triangles per grid square.
        for (int i=0; i<tileSideEdges; i++)
        {
            for (int j=0; j<tileSideEdges; j++)
            {
                // Triangle 1
                outputTris[index] = i * (tileSideEdges + 1) + j;
                outputTris[index + 2] = outputTris[index] + 1;
                outputTris[index + 1] = outputTris[index + 2] + tileSideEdges + 1;
                // Triangle 2
                outputTris[index + 3] = outputTris[index];
                outputTris[index + 4] = outputTris[index] + tileSideEdges + 1;
                outputTris[index + 5] = outputTris[index + 4] + 1;

                index += 6;
            }
        }

        return outputTris;
    }

    // Generates UVs based on the vertex locations.
    private static Vector2[] GenerateUVs( Vector3[] vertices )
    {

        // Array for storing UV values.
        var outputUVs = new Vector2[vertices.Length];

        // UV positions correlate to offset vertex positions because tiles are 1x1 by default.
        for (int i=0; i<vertices.Length; i++)
        {
            outputUVs[i] = new Vector2( vertices[i].x, vertices[i].z ) + 0.5f * Vector2.one;
        }

        return outputUVs;

    }

}
