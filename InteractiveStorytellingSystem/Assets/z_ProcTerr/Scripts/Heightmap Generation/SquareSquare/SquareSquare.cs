using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SquareSquare 
{
	static int goalDimensionSize; //The algorithm is finished when this dimension/size is reached
	static float roughnessDimension; //The roughness of the terrain (0 very smooth, 1 very rough)

	/// <summary>
	/// Public method which generates and returns a heightmap made with the square square algorithm
	/// </summary>
	/// <returns>The height map</returns>
	/// <param name="size">value of n, where the size of the heightmap is 2^n+1</param>
	/// <param name="seed">Seed value to intialise the pseudo-random number generator with</param>
	/// <param name="roughness">The roughness of the terrain (0 very smooth, 1 very rough)</param>
	public static float[,] GenerateHeightMap(int size, int seed, float roughness)
	{
		SetParameters (size, seed, roughness);
		float[,] heightMap = SquareSquare_Main(); //Use square square to generate the heightmap

		NormaliseHeightMap (heightMap); //normalise

		float[,] nm = ResizeArray (heightMap, heightMap.GetLength (0) - 1, heightMap.GetLength (1) - 1); //ensure the heightmap is the same dimension as the Terrain object's heightmap
		return nm; //return the heightmap
	}

	/// <summary>
	/// Sets the parameters of the script
	/// </summary>
	/// <param name="size">value of n, where the size of the heightmap is 2^n+1</param>
	/// <param name="seed">Seed value to intialise the pseudo-random number generator with</param>
	/// <param name="roughness">The roughness of the terrain (0 very smooth, 1 very rough)</param>
	static void SetParameters(int size, int seed, float roughness)
	{
		goalDimensionSize = (int)Mathf.Pow (2, size) + 1;
		roughnessDimension = roughness;
		Random.InitState(seed);
	}

	/// <summary>
	/// Uses the square-square algorithm to generate and return a new terrain heightmap
	/// </summary>
	static float[,] SquareSquare_Main()
	{
		int currentDimensionSize = 3; //set the initial dimension size (should always start at 3)
		int dimensionIncrease = 1; //amount to increase the dimension by every iteration (should always start at 1)

		float[,] newHeightMap = null;
		float[,] oldHeightmap = new float[currentDimensionSize, currentDimensionSize]; //create an array to keep a record of the heightmap
		Reset (oldHeightmap);

		//repeat while the goal dimension size has not been reached
		while (currentDimensionSize < goalDimensionSize) 
		{
			newHeightMap = new float[currentDimensionSize + dimensionIncrease, currentDimensionSize + dimensionIncrease]; //create a new heightmap which is bigger than the old heightmap
			int squareDimension = currentDimensionSize - 1; //


			for (int y = 0; y < squareDimension; y++) //cycle through squares
			{
				for (int x = 0; x < squareDimension; x++) //cycle through squares
				{
					//set each corner of the new square to be a weighted average of its parent square's corners in the ratio 9:3:3:1, where the nearest corners get the highest weighting

					//top-left
					newHeightMap [(x * 2), (y * 2)] = (((9.0f * oldHeightmap [x, y]) + (3.0f * oldHeightmap [x + 1, y]) + (3.0f * oldHeightmap [x, y + 1]) + (1.0f * oldHeightmap [x + 1, y + 1])) / 16.0f) + RandAroundZero (roughnessDimension); //TL
					//top-right
					newHeightMap [(x * 2) + 1, (y * 2)] = (((9.0f * oldHeightmap [x+1, y]) + (3.0f * oldHeightmap [x, y]) + (3.0f * oldHeightmap [x+1, y + 1]) + (1.0f * oldHeightmap [x, y + 1])) / 16.0f) + RandAroundZero (roughnessDimension); //TR
					//bottom-left
					newHeightMap [(x * 2), (y * 2) + 1] = (((9.0f * oldHeightmap [x, y+1]) + (3.0f * oldHeightmap [x, y]) + (3.0f * oldHeightmap [x+1, y + 1]) + (1.0f * oldHeightmap [x + 1, y])) / 16.0f) + RandAroundZero (roughnessDimension); //BL
					//bottom-right
					newHeightMap [(x * 2) + 1, (y * 2) + 1] = (((9.0f * oldHeightmap [x+1, y+1]) + (3.0f * oldHeightmap [x + 1, y]) + (3.0f * oldHeightmap [x, y + 1]) + (1.0f * oldHeightmap [x, y])) / 16.0f) + RandAroundZero (roughnessDimension); //BR
				}
			}
				
			currentDimensionSize += dimensionIncrease; //increase the current dimension size by the dimension increase
			dimensionIncrease *= 2; //double the dimension increase for the next iteration
			roughnessDimension = roughnessDimension / 2.0f; //half the roughness dimension

			oldHeightmap = newHeightMap; //set the old heightmap to be the current heightmap, ready for the next iteration
		}
		return newHeightMap;
	}

	/// <summary>
	/// Sets all heightmap values randomly
	/// </summary>
	static void Reset(float[,] map)
	{
		for (int i = 0; i < map.GetLength (0); i++) 
		{
			for (int j = 0; j < map.GetLength (1); j++)
			{
				SetValue(map, i, j, Random.value);
			}
		}
	}
		
	static T[,] ResizeArray<T>(T[,] original, int rows, int cols)
	{
		var newArray = new T[rows, cols];
		int minRows = Mathf.Min (rows, original.GetLength (0));
		int minCols = Mathf.Min (cols, original.GetLength (1));
		for (int i = 0; i < minRows; i++)
			for (int j = 0; j < minCols; j++)
				newArray [i, j] = original [i, j];
		return newArray;
	}

	/// <summary>
	/// Sets the heightmap cell at the given coordinates to the specified value
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="val">value to set the cell to</param>
	static void SetValue(float[,] heightMap, int x, int y, float val)
	{
		heightMap [x, y] = val;
	}

	/// <summary>
	/// Gets the value of a specific heightmap cell
	/// </summary>
	/// <returns>The value.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	static float GetValue(float[,] heightmap, int x, int y)
	{
		if(x > -1 && x < heightmap.GetLength(0) && y > -1 && y < heightmap.GetLength(1))
			return heightmap [x, y];
		return 0.0f;
	}
		
	/// <summary>
	/// Finds the maximum value of the heightmap
	/// </summary>
	/// <returns>The maximum value of the heightmap</returns>
	static float FindMax(float[,] heightMap)
	{
		float Max = 0.0f;
		for (int i = 0; i < heightMap.GetLength (0); i++) 
		{
			for (int j = 0; j < heightMap.GetLength (1); j++)
			{
				float temp = GetValue (heightMap, i, j);
				if (temp > Max)
					Max = temp;
			}
		}
		return Max;
	}

	/// <summary>
	/// Finds the minimum value of the heightmap
	/// </summary>
	/// <returns>The minimum value of the heightmap</returns>
	static float FindMin(float[,] heightMap)
	{
		float Min = 1.0f;
		for (int i = 0; i < heightMap.GetLength (0); i++) 
		{
			for (int j = 0; j < heightMap.GetLength (1); j++)
			{
				float temp = GetValue (heightMap, i, j);
				if (temp < Min)
					Min = temp;
			}
		}
		return Min;
	}

	/// <summary>
	/// Normalises the heightmap using the min and max values
	/// </summary>
	static float[,] NormaliseHeightMap(float[,] heightMap)
	{
		float max = FindMax (heightMap);
		float min = FindMin (heightMap);
		for (int i = 0; i < heightMap.GetLength (0); i++) 
		{
			for (int j = 0; j < heightMap.GetLength (1); j++)
			{
				float oldValue = GetValue(heightMap, i, j);
				float newValue = (oldValue - min)/(max - min);
				SetValue (heightMap, i, j, newValue);
			}
		}
		return heightMap;
	}

	/// <summary>
	/// Offset the specified value
	/// </summary>
	/// <param name="value">The value to offset</param>
	/// <param name="spread">The spread value to use in the calculation</param>
	static float Offset(float value, float spread)
	{
		return value + RandAroundZero(spread);
	}

	/// <summary>
	/// Produces a random number that is no greater than 0+spread, and no less than 0+spread
	/// </summary>
	/// <returns>a random number</returns>
	/// <param name="spread">the amount of spread</param>
	static float RandAroundZero(float spread)
	{
		int r = 0;
		while (r == 0)
			r = Random.Range (-1, 2);
		return (spread * r)/5;
	}
}
