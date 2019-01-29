using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Normalise 
{
	/// <summary>
	/// Normalises the heightmap between 0 and 1.
	/// </summary>
	/// <returns>The normalised heightmap</returns>
	/// <param name="heightmap">Heightmap to normalise</param>
	public static float[,] NormaliseHeightmap(float[,] heightmap)
	{
		float dimension = heightmap.GetLength (0);

		//Loop through the heightmap and normalise each value
		for (int y = 0; y < dimension; y++) 
		{
			for (int x = 0; x < dimension; x++) 
			{
				heightmap [x, y] = Mathf.Lerp (0.0f, 1.0f, heightmap[x,y]);
			}
		}
		return heightmap;
	}
}
