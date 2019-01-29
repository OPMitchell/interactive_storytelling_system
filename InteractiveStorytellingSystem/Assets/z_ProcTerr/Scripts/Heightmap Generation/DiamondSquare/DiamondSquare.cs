using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DiamondSquare
{
	static float[,] heightMap; //array of the heightmap to generate and return
	static int size; //value of n, where the size of the heightmap is 2^n+1
	static int newSize; //size of the heightmap (2^n+1)
	static float initialSpread; //Initial spread value
	static float spreadReductionRate; //Value to multiply the spread by after each iteration

	/// <summary>
	/// Public method which generates and returns a heightmap made with the diamond-square algorithm
	/// </summary>
	/// <returns>The height map</returns>
	/// <param name="pSize">value of n, where the size of the heightmap is 2^n+1</param>
	/// <param name="seed">Seed value to intialise the pseudo-random number generator with</param>
	/// <param name="pInitialSpread">Initial spread value</param>
	/// <param name="pSpreadReductionRate">Value to multiply the spread by after each iteration</param>
	public static float[,] GenerateHeightMap(int pSize, int seed, float pInitialSpread, float pSpreadReductionRate)
	{
		SetParameters (pSize, seed, pInitialSpread, pSpreadReductionRate);
		ds_displace_main ();
		return heightMap;
	}

	/// <summary>
	/// Sets the parameters of the script
	/// </summary>
	/// <param name="pSize">value of n, where the size of the heightmap is 2^n+1</param>
	/// <param name="seed">Seed value to intialise the pseudo-random number generator with</param>
	/// <param name="pInitialSpread">Initial spread value</param>
	/// <param name="pSpreadReductionRate">Value to multiply the spread by after each iteration</param>
	static void SetParameters(int pSize, int seed, float pInitialSpread, float pSpreadReductionRate)
	{
		size = pSize;
		initialSpread = pInitialSpread;
		spreadReductionRate = pSpreadReductionRate;
		Random.InitState(seed);
		newSize = (int)Mathf.Pow (2, size) + 1;
		heightMap = new float[newSize, newSize];
	}

	/// <summary>
	/// Sets the heightmap cell at the given coordinates to the specified value
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	/// <param name="val">value to set the cell to</param>
	static void SetValue(int x, int y, float val)
	{
		heightMap [x, y] = val;
	}
		
	/// <summary>
	/// Gets the value of a specific heightmap cell
	/// </summary>
	/// <returns>The value.</returns>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	static float GetValue(int x, int y)
	{
		//check that given coordinates are valid before returning value
		if(x > -1 && x < heightMap.GetLength(0) && y > -1 && y < heightMap.GetLength(1))
			return heightMap [x, y];
		return 0.0f;
	}

	/// <summary>
	/// Sets all heightmap values randomly
	/// </summary>
	static void Reset()
	{
		for (int i = 0; i < heightMap.GetLength (0); i++) 
		{
			for (int j = 0; j < heightMap.GetLength (1); j++)
			{
				SetValue(i, j, Random.value);
			}
		}
	}

	/// <summary>
	/// The square step sets the midpoint of each quad to be the average of its 4 corners
	/// </summary>
	/// <param name="lx">left edge of the current quad</param>
	/// <param name="rx">right edge of the current quad</param>
	/// <param name="by">bottom edge of the current quad</param>
	/// <param name="ty">top edge of the current quad</param>
	/// <param name="spread">current spread value</param>
	static void SquareStep(int lx, int rx, int by, int ty, float spread)
	{
		int cx = Midpoint(lx, rx); //get the index of the center of the current quadrilateral on the x-axis
		int cy = Midpoint(by, ty); //get the index of the center of the current quadrilateral on the y-axis

		float bl = GetValue(lx, by); //get the value of the bottom-left corner of the current quadrilateral
		float br = GetValue(rx, by); //get the value of the bottom-right corner of the current quadrilateral
		float tl = GetValue(lx, ty); //get the value of the top-left corner of the current quadrilateral
		float tr = GetValue(rx, ty); //get the value of the top-right corner of the current quadrilateral

		float centre = AverageOf4 (bl, br, tl, tr); //Calculate the average of the 4 corners of the current quad

		SetValue (cx, cy, centre); //set the centre of the quad to the average of the 4 corners
	}

	/// <summary>
	/// The diamond step sets the midpoint of each diamond to the average of its 4 corners
	/// </summary>
	/// <param name="lx">left edge of the current quad</param>
	/// <param name="rx">right edge of the current quad</param>
	/// <param name="by">bottom edge of the current quad</param>
	/// <param name="ty">top edge of the current quad</param>
	/// <param name="spread">current spread value</param>
	/// <param name="quadWidth">width of the current quad</param>
	static void DiamondStep(int lx, int rx, int by, int ty, float spread, int quadWidth)
	{
		int cx = Midpoint(lx, rx); //get the index of the center of the current quadrilateral on the x-axis
		int cy = Midpoint(by, ty); //get the index of the center of the current quadrilateral on the y-axis

		float bl = GetValue(lx, by); //get the value of the bottom-left corner of the current quadrilateral
		float br = GetValue(rx, by); //get the value of the bottom-right corner of the current quadrilateral
		float tl = GetValue(lx, ty); //get the value of the top-left corner of the current quadrilateral
		float tr = GetValue(rx, ty); //get the value of the top-right corner of the current quadrilateral
		float centre = GetValue(cx, cy); //get the value at the centre the current quadrilateral

		float centreLeft = GetValue(cx - quadWidth, cy); //get the value at the point exactly half a quad-width to the left
		float centreTop = GetValue(cx, cy + quadWidth); //get the value at the point exactly half a quad-width above
		float centreRight = GetValue(cx + quadWidth, cy); //get the value at the point exactly half a quad-width to the right
		float centreBottom = GetValue(cx, cy - quadWidth); //get the value at the point exactly half a quad-width below

		float left = AverageOf4 (tl, bl, centre, centreLeft); //Calculate the centre of the left-diamond to be the average of its 4 corner points
		float top = AverageOf4 (tl, tr, centre, centreTop);  //Calculate the centre of the top-diamond to be the average of its 4 corner points
		float right = AverageOf4 (tr, br, centre, centreRight);  //Calculate the centre of the right-diamond to be the average of its 4 corner points
		float bottom = AverageOf4 (bl, br, centre, centreBottom);  //Calculate the centre of the bottom-diamond to be the average of its 4 corner points
		 
		SetValue (lx, cy, Offset (left, spread)); //set the left-diamond
		SetValue (cx, ty, Offset (top, spread)); //set the top-diamond
		SetValue (rx, cy, Offset (right, spread)); //set the right-diamond
		SetValue (cx, by, Offset (bottom, spread)); //set the bottom-diamond
	}

	/// <summary>
	/// Repeatedly subdivides the heightmap into more quadrilaterals and calls the displacement method on each quad
	/// </summary>
	static void ds_displace_main()
	{
		Reset(); //Set all heightmap values to random floats
		float spread = initialSpread; //set the spread equal to the initial spread

		//repeatedly subdivide the lattice into 2^n quads until size is reached
		for (int iteration = 0; iteration < size; iteration++) 
		{
			int quads = (int)Mathf.Pow (2, iteration); //calculate how many quads are in the lattice at the current iteration, remember 2^0 = 1
			int quadWidth = (newSize - 1) / quads; //calculate the width of each quad as the overall size of the heightmap-1, divided by the number of quads

			//iterate through all the quads and displace them using the square and diamond steps
			for (int quadx = 0; quadx < quads; quadx++)  
			{
				for (int quady = 0; quady < quads; quady++)
				{
					int leftx = quadWidth * quadx; //get the index of the left edge of the current quad
					int rightx = leftx + quadWidth; //get the index of the right edge of the current quad
					int bottomy = quadWidth * quady; //get the index of the bottom edge of the current quad
					int topy = bottomy + quadWidth; //get the index of the top edge of the current quad

					SquareStep (leftx, rightx, bottomy, topy, spread); //displace the midpoints of the current quad with the square-step
					DiamondStep (leftx, rightx, bottomy, topy, spread, quadWidth); //displace the midpoints of the current quad with the diamond-step
				}
			}
			spread *= spreadReductionRate; //once all quads are evaluated, reduce the spread
		}
		NormaliseHeightMap (); //normalise the heightmap before returning it
	}

	/// <summary>
	/// Find the midpoint between 2 coordinates
	/// </summary>
	/// <param name="x">The x coordinate.</param>
	/// <param name="y">The y coordinate.</param>
	static int Midpoint(int x, int y)
	{
		return (x + y) / 2;
	}

	/// <summary>
	/// Averages 2 numbers
	/// </summary>
	/// <returns>The average of 2 numbers</returns>
	/// <param name="a">first number</param>
	/// <param name="b">second number</param>
	static float AverageOf2(float a, float b)
	{
		return (a + b) / 2.0f;
	}

	/// <summary>
	/// Averages 3 numbers
	/// </summary>
	/// <returns>The average of 3 numbers</returns>
	/// <param name="a">first number</param>
	/// <param name="b">second number</param>
	/// <param name="c">third number</param>
	static float AverageOf3(float a, float b, float c)
	{
		return (a + b + c) / 3.0f;
	}

	/// <summary>
	/// Averages 4 numbers
	/// </summary>
	/// <returns>The average of 4 numbers</returns>
	/// <param name="a">first number</param>
	/// <param name="b">second number</param>
	/// <param name="c">third number</param>
	/// <param name="d">fourth number</param>
	static float AverageOf4(float a, float b, float c, float d)
	{
		if (d == 0.0f)
			return AverageOf3 (a, b, c);
		return (a + b + c + d) / 4.0f;
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
		return (spread * r);
	}

	/// <summary>
	/// Finds the maximum value of the heightmap
	/// </summary>
	/// <returns>The maximum value of the heightmap</returns>
	static float FindMax()
	{
		float Max = 0.0f;
		for (int i = 0; i < heightMap.GetLength (0); i++) 
		{
			for (int j = 0; j < heightMap.GetLength (1); j++)
			{
				float temp = GetValue (i, j);
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
	static float FindMin()
	{
		float Min = 1.0f;
		for (int i = 0; i < heightMap.GetLength (0); i++) 
		{
			for (int j = 0; j < heightMap.GetLength (1); j++)
			{
				float temp = GetValue (i, j);
				if (temp < Min)
					Min = temp;
			}
		}
		return Min;
	}
		
	/// <summary>
	/// Normalises the heightmap using the min and max values
	/// </summary>
	static void NormaliseHeightMap()
	{
		float max = FindMax ();
		float min = FindMin ();
		for (int i = 0; i < heightMap.GetLength (0); i++) 
		{
			for (int j = 0; j < heightMap.GetLength (1); j++)
			{
				float oldValue = GetValue(i, j);
				float newValue = (oldValue - min)/(max - min);
				SetValue (i, j, newValue);
			}
		}
	}

}
