using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all important information about a single "chunk" of terrain.
/// </summary>
public class ChunkData
{
	public GameObject terrain { get; set; } // The Unity Terrain object which generates and holds the mesh to display the terrain
	public TerrainLayers terrainLayers { get; set; } // A layered data representation of rocks with different solubilities
	public float[,] initialHeightMap { get; set; } // The initial, uneroded heightmap
	public float[,] temperatureMap { get; set; } // A heightmap specifying temperature across the terrain
	public float[,] precipitationMap { get; set; } // A heightmap specifying the amount of rainfall across the terrain
	public int dimension { get; set; } // The width of the heightmap

	/// <summary>
	/// Initializes a new instance of the ChunkData/> class.
	/// </summary>
	/// <param name="chunkDimension">The width of the heightmap</param>
	/// <param name="terrainHeight">The total height of the terrain</param>
	/// <param name="layers">A layered data representation of rocks with different solubilities</param>
	public ChunkData(int chunkDimension, int terrainHeight, TerrainLayers layers)
	{
		terrainLayers = layers;
		dimension = chunkDimension;
		InitialiseTerrain (terrainHeight);
	}

	/// <summary>
	/// Gets the height map.
	/// </summary>
	/// <returns>The terrain height map.</returns>
	public float[,] GetHeightMap()
	{
		return GetTerrainData().GetHeights(0,0, GetTerrainData().heightmapWidth, GetTerrainData().heightmapWidth);
	}

	/// <summary>
	/// Sets the height map.
	/// </summary>
	/// <param name="heightmap">The new terrain heightmap</param>
	public void SetHeightMap(float[,] heightmap)
	{
		GetTerrainData().SetHeights(0,0, heightmap);
	}

	/// <summary>
	/// Initialises the Unity terrain object
	/// </summary>
	/// <param name="terrainHeight">The total height of the terrain</param>
	private void InitialiseTerrain(int terrainHeight)
	{
		terrain = new GameObject ("Chunk"); //Create a new game object, visible to the user
		Terrain t = terrain.AddComponent<Terrain> (); //Add the Unity Terrain object
		t.terrainData = new TerrainData (); //Create a new TerrainData object to hold data about the terrain
		t.terrainData.alphamapResolution = dimension; //set the heightmap's alphamap resolution
		t.terrainData.heightmapResolution = dimension; //set the dimensions of the heightmap
		t.heightmapPixelError = 0; //set the pixel error to 0 (lower values = more polygons drawn, so better looking terrain)
		TerrainCollider tc = terrain.AddComponent<TerrainCollider> (); //Add a TerrainCollider object which allows the use of terrain manipulation tools
		tc.terrainData = t.terrainData; //Ensure that the TerrainData is shared between the Terrain object and its collider
		SetTerrainSize (t.terrainData, terrainHeight); //Set the size of the Terrain object
	}

	/// <summary>
	/// Sets the size of the Terrain object
	/// </summary>
	/// <param name="t">TerrainData associated with the chunk's terrain</param>
	/// <param name="terrainHeight">The total height of the terrain</param>
	private void SetTerrainSize(TerrainData t, int terrainHeight)
	{
		float scale = 1;
		t.size = new Vector3((t.heightmapWidth/2)*scale, terrainHeight*scale, (t.heightmapWidth/2)*scale);
	}

	/// <summary>
	/// Gets the TerrainData object associated with the chunk's terrain
	/// </summary>
	/// <returns>The terrain data.</returns>
	public TerrainData GetTerrainData()
	{
		return terrain.GetComponent<Terrain> ().terrainData;
	}

	/// <summary>
	/// Offsets the chunk's position to ensure that multiple chunks are visible on the terrain
	/// </summary>
	/// <param name="x">The x offset to shift by</param>
	/// <param name="y">The y offset to shift by</param>
	public void OffsetChunk(int x, int y)
	{
		TerrainData t = GetTerrainData ();
		float xPos = y * t.size.x;
		float yPos = x * t.size.z;
		terrain.transform.position = new Vector3 (xPos, 10, yPos);
	}
}