using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//NOT INCLUDED IN THE FINAL TOOL

public static class Perlin 
{
	private static int dimension;
	private static float[,] heightmap;
	public static float minHeight { get; set; }
	public static float maxHeight{ get; set; }

	public static float[,] GenerateHeightMap(int size, int seed, float offsetX, float offsetY, float lac, float per, float sca)
	{
		dimension = (int)Mathf.Pow (2, size)+1;
		Random.InitState (seed);

		heightmap = new float[dimension, dimension];

		float scale = sca;
		int octaves = 7;
		float persistance = per;
		float lacunarity = lac;

		maxHeight = float.MinValue;
		minHeight = float.MaxValue;

		for (int y = 0; y < dimension; y++) 
		{
			for (int x = 0; x < dimension; x++) 
			{
				float amplitude = 1.0f;
				float frequency = 1.0f;
				float heightValue = 0.0f;

				for (int o = 0; o < octaves; o++) 
				{
					float xCoord = (float)(((float)x+offsetX) / scale * frequency);
					float yCoord = (float)(((float)y+offsetY) / scale * frequency);

					float pVal = Mathf.PerlinNoise (xCoord, yCoord) *2.0f -1.0f;
					heightValue += pVal * amplitude;

					amplitude *= persistance;
					frequency *= lacunarity;
				}
				if (heightValue > maxHeight)
					maxHeight = heightValue;
				else if (heightValue < minHeight)
					minHeight = heightValue;
				SetValue (heightmap, x, y, heightValue);
			}
		}

		NormaliseHeightmap (heightmap);
		return heightmap;
	}

	private static void SetValue(float[,] heightmap, int x, int y, float value)
	{
		heightmap [x, y] = value;
	}
		
	public static float[,] NormaliseHeightmap(float[,] heightmap)
	{
		float dimension = heightmap.GetLength (0);
		float minHeight = float.MaxValue;
		float maxHeight = float.MinValue;

		for (int y = 0; y < dimension; y++) 
		{
			for (int x = 0; x < dimension; x++) 
			{
				float val = heightmap [x, y];
				if (val < minHeight)
					minHeight = val;
				else if (val > maxHeight)
					maxHeight = val;
			}
		}

		for (int y = 0; y < dimension; y++) 
		{
			for (int x = 0; x < dimension; x++) 
			{
				heightmap [x, y] = Mathf.InverseLerp (minHeight, maxHeight, heightmap[x,y]);
			}
		}
		return heightmap;
	}

}
