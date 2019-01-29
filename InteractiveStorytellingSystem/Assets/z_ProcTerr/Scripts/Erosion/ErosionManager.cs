using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Receives a chunkData object and performs erosion operations on its heightmap.
/// </summary>
public static class ErosionManager 
{
	/// <summary>
	/// Applies all 3 erosion methods to the chunk's heightmap.
	/// </summary>
	/// <param name="generator">The ChunkGenerator object which holds the array of chunkData objects</param>
	/// <param name="c">The chunkData object to perform erosion on</param>
	public static void ApplyErosion (ChunkGenerator generator, ChunkData c) 
	{
		ThermalErosion (generator, c); //Apply thermal erosion
		DiffusiveErosion (generator, c); //Apply diffusive erosion
		HydraulicErosion (generator, c); //Apply hydraulic erosion
	}

	/// <summary>
	/// Calls the thermal erosion method to perform erosion on the chunkData's heightmap
	/// </summary>
	/// <param name="generator">The ChunkGenerator object which holds the array of chunkData objects</param>
	/// <param name="c">The chunkData object to perform erosion on</param>
	private static void ThermalErosion(ChunkGenerator generator, ChunkData c)
	{
		c.SetHeightMap(generator.GetComponent<ThermalErosion> ().Erode (c.GetHeightMap()));
	}

	/// <summary>
	/// Calls the diffusive erosion method to perform erosion on the chunkData's heightmap
	/// </summary>
	/// <param name="generator">The ChunkGenerator object which holds the array of chunkData objects</param>
	/// <param name="c">The chunkData object to perform erosion on</param>
	private static void DiffusiveErosion(ChunkGenerator generator, ChunkData c)
	{
		c.SetHeightMap(generator.GetComponent<DiffusiveErosion> ().Erode (c.GetHeightMap()));
	}

	/// <summary>
	/// Calls the hydraulic erosion method to perform erosion on the chunkData's heightmap
	/// </summary>
	/// <param name="generator">The ChunkGenerator object which holds the array of chunkData objects</param>
	/// <param name="c">The chunkData object to perform erosion on</param>
	private static void HydraulicErosion(ChunkGenerator generator, ChunkData c)
	{
		c.SetHeightMap(generator.GetComponent<HydraulicAction> ().Erode (c));
		/*
   			The hydraulic erosion algorithm requires the entire ChunkData object, not just the
  		 	terrain heightmap. This is because it needs access to the heat and rainfall maps too.
		*/

	}
}
