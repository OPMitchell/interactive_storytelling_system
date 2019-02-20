using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour 
{
	private HeightmapGenerationManager manager; //reference to the heightmapgenerationmanager script
	private TerrainLayers layers; //reference to the layered data representation

	public Transform terrainHolder { get; set; } //Parent object to hold terrain objects
	public ChunkData[,] chunks { get; set; } //Collection of chunkData objects to store info on different chunks

	private const int numberOfChunks = 1; //number of chunks to generate (future work on this tool will allow more than 1 chunk to be generated)
	private const int genMethodForTempAndRainfall = 1; //gen method to use for temp and rainfall maps
	private const float RoughnessForTempAndRainfall = 0.5f; //Roughness to use for temp and rainfall maps

	/// <summary>
	/// Generate the entire terrain and textures it
	/// </summary>
	public void Generate()
	{
		//DeleteOldTerrain (); //Delete any existing terrains in the scene
		manager = GetComponent<HeightmapGenerationManager> (); //get reference to heightmapgenerationmanager script
		layers = GetComponent<TerrainLayers> (); //get reference to layered data representation

		int dimension = (int)Mathf.Sqrt (numberOfChunks); //Calculate the dimension of the grid of chunks (cannot be more than 1 at this time)

		chunks = new ChunkData[dimension, dimension];
		//iterate through all initialised chunks and create and texture terrain for each one
		for (int y = 0; y < dimension; y++) 
		{
			for (int x = 0; x < dimension; x++) 
			{
				int chunkDimension = (int)Mathf.Pow (2, manager.GetChunkSize()) + 1; //dimension of each chunk's heightmap = 2^n+1
				chunks [x, y] = new ChunkData (chunkDimension, manager.terrainHeight, layers); //create the chunkData object for this terrain
				float xOffset = (float)x * chunkDimension; //calculate how far to offset the chunk in the x-axis to avoid overlapping terrain chunks
				float yOffset = (float)y * chunkDimension; //calculate how far to offset the chunk in the y-axis to avoid overlapping terrain chunks
				chunks [x,y].terrain.transform.SetParent (terrainHolder); //set the chunk's parent object to be the terrain-holder object (helps keep hierarchy tidy)

				float[,] terrainMap = Normalise.NormaliseHeightmap (ApplyHeightCurve(manager.GenerateHeightMap(xOffset, yOffset))); //Generate the terrain heightmap
				float[,] temperatureMap = Normalise.NormaliseHeightmap (manager.GenerateHeatMap(xOffset, yOffset, genMethodForTempAndRainfall, RoughnessForTempAndRainfall)); //Generate the heatmap
				float[,] precipitationMap = Normalise.NormaliseHeightmap (manager.GenerateRainMap(xOffset, yOffset, genMethodForTempAndRainfall, RoughnessForTempAndRainfall)); //Generate the rainmap

				//set the heightmap, originalheightmap, heatmap, and rainmap values
				chunks [x, y].SetHeightMap(terrainMap); 
				chunks [x, y].initialHeightMap = terrainMap;
				chunks [x, y].temperatureMap = temperatureMap;
				chunks [x, y].precipitationMap = precipitationMap;

				chunks[x,y].OffsetChunk(x,y); //offset the chunk to avoid overlapping terrain chunks

				//GetComponent<TextureManager> ().TextureChunk(chunks[x,y]); //Texture the chunk (Terrain object's alphamap)
			}
		}
	}

	/// <summary>
	/// Loops through all the chunks in the terrain, applies erosion to each one, then rextextured them
	/// </summary>
	public void ApplyErosion()
	{
		int dimension = (int)Mathf.Sqrt (numberOfChunks);
		//iterate through all chunks
		for (int y = 0; y < dimension; y++) 		{
			for (int x = 0; x < dimension; x++) 
			{
				ErosionManager.ApplyErosion(this, chunks[x,y]); //apply erosion
				//GetComponent<TextureManager> ().TextureChunk(chunks[x,y]); //retexture
			}
		}
	}
		
	/// <summary>
	/// Removes existing terrain from the scene-view
	/// </summary>
	private void DeleteOldTerrain()
	{
		DestroyImmediate (GameObject.Find ("Terrain"));
		terrainHolder = new GameObject ("Terrain").transform;
	}

	/// <summary>
	/// Applies a mathematical curve to the heightmap
	/// </summary>
	/// <returns>The curve to apply</returns>
	/// <param name="heightMap">The adjusted heightmap</param>
	private float[,] ApplyHeightCurve(float[,] heightMap)
	{
		AnimationCurve curve = GetComponent<HeightmapGenerationManager> ().GetCurve (); //get the curve
		//Iterate through all the heightmap's values
		for (int y = 0; y < heightMap.GetLength(1); y++) 
		{
			for (int x = 0; x < heightMap.GetLength(0); x++) 
			{
				heightMap[x,y] = curve.Evaluate (heightMap [x, y]); //evaluate the curve and update the heightmap
			}
		}
		return heightMap;
	}

	/// <summary>
	/// Iterates through all chunks and textures them
	/// </summary>
	public void TextureAllChunks()
	{
		foreach (ChunkData c in chunks) 
		{
			//GetComponent<TextureManager> ().TextureChunk (c);
		}
	}

}
