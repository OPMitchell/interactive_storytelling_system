using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Texture setting that contains information regarding where and where not a texture can appear on the terrain.
/// </summary>
[System.Serializable]
public class TextureSetting
{
	[Tooltip("The texture you want to be placed")]
	public Texture2D Texture; //The texture to be used

	[Range(0,1)]
	public float MaxHeightAsAPercentage; //the highest altitude the texture can appear at
	[Range(0,1)]
	public float MinHeightAsAPercentage; //the lowest altitude the texture can appear at
	[Range(0,90)]
	public float MinSteepness; //the greatest steepness the texture can appear at
	[Range(0,90)]
	public float MaxSteepness; //the lowest steepness the texture can appear at
	[Range(0,1)]
	public float MinRainfall; //the lowest average rainfall the texture can appear at
	[Range(0,1)]
	public float MaxRainfall; //the greatest average rainfall the texture can appear at
	[Range(0,1)]
	public float MinTemperature; //the lowest temperature the texture can appear at
	[Range(0,1)]
	public float MaxTemperature; //the highest temperature the texture can appear at

	/// <summary>
	/// Initializes a new instance of the TextureSetting class.
	/// </summary>
	public TextureSetting(float minH, float maxH, float minS, float maxS, float minT, float maxT, float minM, float maxM, Texture2D t)
	{
		MinHeightAsAPercentage = minH;
		MaxHeightAsAPercentage = maxH;
		MinSteepness = minS;
		MaxSteepness = maxS;
		MinTemperature = minT;
		MaxTemperature = maxT;
		MinRainfall = minM;
		MaxRainfall = maxM;
		Texture = t;
	}
}
	
/// <summary>
/// Texture manager that contains a collection of all texture settings and applies them to the terrain mesh.
/// </summary>
public class TextureManager : MonoBehaviour 
{
	[SerializeField] private TextureMethod textureMethod; //The texturing method to use (terrain, temperature, or rainfall)
	[SerializeField] private TextureSetting[] Textures; //The collection of TextureSetting objects to use for the terrain texture method.

	/// <summary>
	/// Textures the chunk according to the user's textureMethod preference and their texture settings.
	/// </summary>
	/// <param name="chunk">The ChunkData object of the terrain to texture</param>
	public void TextureChunk(ChunkData chunk)
	{
		TerrainData terrainData = chunk.GetTerrainData (); //Get information about the terrain to texture

		if ((int)textureMethod == 1) 
		{
			terrainData.splatPrototypes = CreateSplatPrototypes (Textures); //terrain texture method
		}
		else if ((int)textureMethod == 2) 
		{
			terrainData.splatPrototypes = CreateSplatPrototypes (CreateHeatTextureSetting()); //temperature texture method
		}
		else if ((int)textureMethod == 3) 
		{
			terrainData.splatPrototypes = CreateSplatPrototypes (CreateRainTextureSetting()); //rainfall texture method
		}

		terrainData.RefreshPrototypes (); //Reloads all the values of the available prototypes in the TerrainData Object.
		terrainData.SetAlphamaps (0, 0, CreateSplatmap(chunk)); //Assign all splat values in the given map area.
	}

	/// <summary>
	/// Creates the splatma for the given terrain chunk
	/// </summary>
	/// <returns>The splatmap as a 3D float array</returns>
	/// <param name="chunk">ChunkData object of terrain</param>
	private float[,,] CreateSplatmap(ChunkData chunk)
	{
		TerrainData terrainData = chunk.GetTerrainData (); //Get information about the terrain to texture
		int splatLengths = terrainData.splatPrototypes.Length; //Get how many textures there are to use
		int alphaMapResolution = terrainData.alphamapResolution;
		int alphaMapHeight = terrainData.alphamapResolution;
		int alphaMapWidth = terrainData.alphamapResolution;

		var splatMap = new float[alphaMapResolution, alphaMapResolution, splatLengths]; //Create an object to store the splatmap
		var heights = terrainData.GetHeights(0, 0, alphaMapWidth, alphaMapHeight);  //Create an object to store the height at each point of the terrain

		//Iterate through the alphamap (same resolution as terrain heightmap) and apply each texture according to some rules.
		for (int i = 0; i < terrainData.alphamapHeight; i++) 
		{
			for (int j = 0; j < terrainData.alphamapWidth; j++) 
			{
				var splatWeights = new float[splatLengths];
				float normalizedX = (float)i / ((float)terrainData.alphamapWidth - 1f);
				float normalizedZ = (float)j / ((float)terrainData.alphamapHeight - 1f);

				float steepness = terrainData.GetSteepness (normalizedX, normalizedZ); //get the steepness at the current point of the terrain

				switch((int)textureMethod) //identify which texture method to use
				{
					case 1:
						CreateTerrainSplatWeights (splatWeights, chunk, i, j, steepness, heights); //Texture the terrain based on texture settings defined by the user
						break;

					case 2:
						CreateHeatSplatWeights (splatWeights, chunk, i, j); //Texture the terrain based on the temperature map
						break;

					case 3:
						CreateRainSplatWeights (splatWeights, chunk, i, j); //Texture the terrain based on the rainfall map
						break;
				}
					
				var totalWeight = splatWeights.Sum ();	//sum all the splat weights,
				for (int w = 0; w < splatLengths; w++) 	//Loop through each splatWeight
				{
					splatWeights[w] /= totalWeight;	//Normalize so that sum of all texture weights = 1
					splatMap[j, i, w] = splatWeights[w]; //Assign value to splatmap
				}
			}
		}

		return splatMap;
	}
		
	/// <summary>
	/// Creates the splat prototypes.
	/// </summary>
	/// <returns>The splat prototypes.</returns>
	/// <param name="t">Collection of texture settings</param>
	private SplatPrototype[] CreateSplatPrototypes(TextureSetting[] t)
	{
		SplatPrototype[] a = new SplatPrototype[t.Length];
		for (int i = 0; i < t.Length; i++) //iterate through all textures and create a splat prototype for each one
		{
			SplatPrototype x = new SplatPrototype ();
			x.texture = t[i].Texture;
			x.tileSize = new Vector2 (6, 6);
			a [i] = x;
		}
		return a;
	}

	/// <summary>
	/// Creates the terrain splat weights based on the user's texture settings.
	/// </summary>
	/// <param name="splatWeights">Splat weights.</param>
	/// <param name="chunk">ChunkData object of terrain</param>
	/// <param name="y">y value of current point in terrain alphamap</param>
	/// <param name="x">x value of current point in terrain alphamap</param>
	/// <param name="steepness">Steepness.</param>
	/// <param name="heights">Heights.</param>
	private void CreateTerrainSplatWeights(float[] splatWeights, ChunkData chunk, int y, int x, float steepness, float[,] heights)
	{
		//iterate through all textures settings
		for (var q = 0; q < Textures.Length; q++) 
		{
			if (isCellSteepnessValid (Textures [q], steepness) //check if steepness at x,y is within settings
				&& isCellTemperatureValid(Textures[q], chunk.temperatureMap[x,y]) //check if temperature at x,y is within settings
				&& isCellRainfallValid(Textures[q], chunk.precipitationMap[x,y]) //check if rainfall amount at x,y is within settings
				&& isCellHeightValid(Textures[q], heights[x,y], chunk.initialHeightMap[x,y])) //check if altitude at x,y is within settings
			{
				//if all of those are true, then assign a weight of 1.0f so that the texture shows at x,y
				splatWeights [q] = 1.0f; 
				break;
			}
		}
	}

	/// <summary>
	/// Creates the terrain splat weights based on the temperature.
	/// </summary>
	/// <param name="splatWeights">Splat weights.</param>
	/// <param name="chunk">ChunkData object of terrain</param>
	/// <param name="y">y value of current point in terrain alphamap</param>
	/// <param name="x">x value of current point in terrain alphamap</param>
	private void CreateHeatSplatWeights(float[] splatWeights, ChunkData chunk, int y, int x)
	{
		float temp = (chunk.temperatureMap [x, y]); //get the temperature at x,y
		splatWeights [0] = temp; //Set the red (hot) texture to the value of the temperature (1.0 is highest = all red)
		splatWeights [1] = 1.0f - temp; //set the blue (cold) texture to the value of 1-temperature (1.0 is highest = all blue)
		//so if temperature is 0.5 then equal blend between red and blue textures.
	}

	/// <summary>
	/// Creates the terrain splat weights based on the amount of rainfall.
	/// </summary>
	/// <param name="splatWeights">Splat weights.</param>
	/// <param name="chunk">ChunkData object of terrain</param>
	/// <param name="y">y value of current point in terrain alphamap</param>
	/// <param name="x">x value of current point in terrain alphamap</param>
	private void CreateRainSplatWeights(float[] splatWeights, ChunkData chunk, int y, int x)
	{
		float rain = (chunk.precipitationMap [x, y]); //get the amount of rainfall at x,y
		splatWeights [0] = rain; //Set the dark-blue (heavy rain) texture to the value of the rainfall (1.0 is highest = all dark-blue)
		splatWeights [1] = 1.0f - rain; //set the light-blue (light rain) texture to the value of 1-rain (1.0 is highest = all light-blue)
		//so if rainfall amount is 0.5 then equal blend between dark-blue and light-blue textures.
	}

	/// <summary>
	/// Checks if the cell's height is between the given setting's min/max
	/// </summary>
	/// <returns>true if height is valid, otherwise false</returns>
	/// <param name="setting">The texture setting to compare against</param>
	/// <param name="height">The height of the current cell</param>
	/// <param name="initialHeight">The initial height of the current cell before erosion</param>
	private bool isCellHeightValid(TextureSetting setting, float height, float initialHeight)
	{
		if (height >= initialHeight * setting.MinHeightAsAPercentage)
			return true;
		return false;
	}

	/// <summary>
	/// Checks if the cell's steepness is between the given setting's min/max
	/// </summary>
	/// <returns>true if steepness is valid, otherwise false</returns>
	/// <param name="setting">The texture setting to compare against</param>
	/// <param name="steepness">The steepness of the current cell</param>
	private bool isCellSteepnessValid(TextureSetting setting, float steepness)
	{
		if (steepness <= setting.MaxSteepness && steepness >= setting.MinSteepness)
			return true;
		return false;
	}

	/// <summary>
	/// Checks if the cell's temperature is between the given setting's min/max
	/// </summary>
	/// <returns>true if temperature is valid, otherwise false</returns>
	/// <param name="setting">The texture setting to compare against</param>
	/// <param name="temperature">The temperature of the current cell</param>
	private bool isCellTemperatureValid(TextureSetting setting, float temperature)
	{
		if (temperature <= setting.MaxTemperature && temperature >= setting.MinTemperature)
			return true;
		return false;
	}

	/// <summary>
	/// Checks if the cell's rainfall amount is between the given setting's min/max
	/// </summary>
	/// <returns>true if rainfall amount is valid, otherwise false</returns>.</returns>
	/// <param name="setting">The texture setting to compare against</param>
	/// <param name="rainfall">The rainfall amount of the current cell</param>
	private bool isCellRainfallValid(TextureSetting setting, float rainfall)
	{
		if (rainfall <= setting.MaxRainfall && rainfall >= setting.MinRainfall)
			return true;
		return false;
	}

	/// <summary>
	/// Creates the temperature-based texture setting preset.
	/// </summary>
	/// <returns>The heat texture setting.</returns>
	private TextureSetting[] CreateHeatTextureSetting()
	{
		TextureSetting[] HeatTextures = new TextureSetting[2];
		HeatTextures [0] = new TextureSetting (0.0f, 1.0f, 0.0f, 90.0f, 0.0f, 1.0f, 0.0f, 1.0f, (Texture2D)Resources.Load ("Textures/cold"));
		HeatTextures [1] = new TextureSetting (0.0f, 1.0f, 0.0f, 90.0f, 0.0f, 1.0f, 0.0f, 1.0f, (Texture2D)Resources.Load ("Textures/hot"));
		return HeatTextures;
	}

	/// <summary>
	/// Creates the rainfall-based texture setting preset.
	/// </summary>
	/// <returns>The rainfall texture setting.</returns>
	private TextureSetting[] CreateRainTextureSetting()
	{
		TextureSetting[] HeatTextures = new TextureSetting[2];
		HeatTextures [0] = new TextureSetting (0.0f, 1.0f, 0.0f, 90.0f, 0.0f, 1.0f, 0.0f, 1.0f, (Texture2D)Resources.Load ("Textures/cold"));
		HeatTextures [1] = new TextureSetting (0.0f, 1.0f, 0.0f, 90.0f, 0.0f, 1.0f, 0.0f, 1.0f, (Texture2D)Resources.Load ("Textures/colder"));
		return HeatTextures;
	}
}

/// <summary>
/// Texture method to use.
/// </summary>
public enum TextureMethod
{
	Terrain = 1,
	Heat = 2,
	Rain = 3,
};
