using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Hydraulic action algorithm
/// </summary>
public class HydraulicAction : MonoBehaviour
{
	[Range(0,1000)] [SerializeField]
	private int strength; // How many times to run the algorithm (strength of erosion).

	[SerializeField]
	private ComputeShader shader; // Compute shader to run on the GPU

	[Range(0.0f,0.1f)][SerializeField]
	private float newWaterPerIteration; //The amount of new rain that falls onto the terrain every iteration. 

	[Range(0.0f,0.5f)][SerializeField]
	private float solubilityCoefficient; //This value describes how susceptible the terrain is to erosion. The larger the value, the more material is dissolved. 

	[Range(0,1)][SerializeField]
	private float evaporationCoefficient; //This value describes how much water evaporates every iteration, and therefore how much sediment is deposited. A value of 0 produces no erosion whatsoever. 

	[Range(0,0.5f)][SerializeField]
	private float sedimentCapacityCoefficient; //This value describes the amount of material resting water can hold. Any excess sediment is then deposited. 

	[Range(0,1.0f)][SerializeField]
	private float temperatureWeightCoefficient; //This value describes how great of an effect the temperature map has on the hydraulic erosion simulation. A value of 0 means there is no effect. 

	[Range(0,1.0f)][SerializeField]
	private float rainWeightCoefficient; //This value describes how great of an effect the rain map has on the hydraulic erosion simulation. A value of 0 means there is no effect. 

	private int waterStepKernelHandle; //ID for the kernel of the water step shader program
	private int transportStepKernelHandle; //ID for the water transport step shader program
	private int evaporationStepKernelHandle; //ID for the evaporation step shader program

	private float[,] originalHeightmap; //The original uneroded heightmap
	private TerrainLayers terrainLayers; //The layered data representation of varying rock solubilities

	/// <summary>
	/// Erodes the specified terrain using the GPU and returns a new eroded heightmap
	/// </summary>
	/// <param name="chunk">Data of the current terrain to be eroded</param>
	public float[,] Erode(ChunkData chunk)
	{
		terrainLayers = chunk.terrainLayers; //get the layered data representation for this chunk

		waterStepKernelHandle = shader.FindKernel("CSWater"); //get the water step kernel handle
		transportStepKernelHandle = shader.FindKernel("CSTransport"); //get the transport step kernel handle
		evaporationStepKernelHandle = shader.FindKernel("CSEvaporation"); //get the evaporation step kernel handle

		originalHeightmap = chunk.GetHeightMap (); //store the current uneroded heightmap as the original heightmap
		float[,] heightMap = chunk.GetHeightMap(); //store the current heightmap
		int width = heightMap.GetLength (0); //get the width of the current heightmap

		float[,] waterMap = new float[width, width]; //create a new water map with the same dimensions as the terrain heightmap
		float[,] sedimentMap = new float[width, width]; //create a new sediment map with the same dimensions as the terrain heightmap

		float[,] temperatureMap = chunk.temperatureMap; //create a new temperature map with the same dimensions as the terrain heightmap
		float[,] precipitationMap = chunk.precipitationMap; //create a new rain map with the same dimensions as the terrain heightmap

		//only do erosion if strength is > 0
		if (strength > 0) 
		{
			System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch (); //create a stopwatch to time how long erosion takes
			stopWatch.Start ();

			//Execute the hydraulic erosion compute shader on the GPU for the specified number of iterations.
			for (int i = 0; i < strength; i++) 
			{
				RunOnGPU (heightMap, waterMap, sedimentMap, temperatureMap, precipitationMap);
			}

			stopWatch.Stop ();
			Debug.Log ("Hydraulic action of " + strength + " iterations took " + stopWatch.Elapsed); //display how long the erosion took
			stopWatch.Reset ();
		}

		return heightMap; //return the eroded heightmap
	}

	/// <summary>
	/// Runs the hydraulic erosion compute shader on the GPU, hence parallelising the erosion method
	/// </summary>
	/// <param name="heightMap">Heightmap to perform erosion on</param>
	/// <param name="waterMap">Water map to use in calculations</param>
	/// <param name="sedimentMap">Sediment map to use in calculations</param>
	/// <param name="temperatureMap">Temperature map to use in calculations</param>
	/// <param name="precipitationMap">Precipitation map to use in calculations</param>
	private void RunOnGPU (float[,] heightMap, float[,] waterMap, float[,] sedimentMap, float[,] temperatureMap, float[,] precipitationMap) 
	{
		//Create a read/writable buffer that contains the heightmap data and sends it to the GPU.
		//We need to specify the length of the buffer and the size of a single element. 
		//The buffer needs to be the same length as the heightmap, and each element in the heightmap is a 
		//single float which is 4 bytes long.
		ComputeBuffer b_heightMap = new ComputeBuffer (heightMap.Length, 4);
		//Set the initial data to be held in the buffer as the pre-generated heightmap
		b_heightMap.SetData (heightMap); 

		//create buffers for the other heightmaps, including an old water map
		ComputeBuffer b_old_waterMap = new ComputeBuffer (waterMap.Length, 4);
		b_old_waterMap.SetData (waterMap); 
		ComputeBuffer b_waterMap = new ComputeBuffer (waterMap.Length, 4);
		b_waterMap.SetData (waterMap); 
		ComputeBuffer b_sedimentMap = new ComputeBuffer (sedimentMap.Length, 4);
		b_sedimentMap.SetData (sedimentMap);

		//create buffers for the temperature and rain maps
		ComputeBuffer b_temperatureMap = new ComputeBuffer (temperatureMap.Length, 4);
		b_temperatureMap.SetData (temperatureMap);
		ComputeBuffer b_precipitationMap = new ComputeBuffer (precipitationMap.Length, 4);
		b_precipitationMap.SetData (precipitationMap);

		ComputeBuffer b_originalHeightMap = new ComputeBuffer (originalHeightmap.Length, 4);
		b_originalHeightMap.SetData (originalHeightmap);

		//create buffers for the layered data representation. Each layers height and solubility needs to be sent to the GPU
		ComputeBuffer b_layerHeights = new ComputeBuffer (terrainLayers.GetLayerHeights().Length, 4);
		b_layerHeights.SetData (terrainLayers.GetLayerHeights());
		ComputeBuffer b_layerSolubilities = new ComputeBuffer (terrainLayers.GetLayerSolubilities().Length, 4);
		b_layerSolubilities.SetData (terrainLayers.GetLayerSolubilities());

		//Set the parameters of the shader program that we'll need
		shader.SetInt ("width", heightMap.GetLength(0)); 
		shader.SetInt ("numberOfLayers", terrainLayers.GetNumberOfLayers ());
		shader.SetFloat ("waterStepCoef", newWaterPerIteration);
		shader.SetFloat ("solubilityConst", solubilityCoefficient);
		shader.SetFloat ("evaporationCoef", evaporationCoefficient);
		shader.SetFloat ("sedimentCapacityCoef", sedimentCapacityCoefficient);
		shader.SetFloat ("temperatureWeightCoef", temperatureWeightCoefficient);
		shader.SetFloat ("rainWeightCoef", rainWeightCoefficient);

		//Set the structured buffers of the shader program to the ComputeBuffers we have just defined
		shader.SetBuffer(waterStepKernelHandle, "heightMap", b_heightMap);
		shader.SetBuffer(transportStepKernelHandle, "heightMap", b_heightMap);
		shader.SetBuffer(evaporationStepKernelHandle, "heightMap", b_heightMap);
		shader.SetBuffer(waterStepKernelHandle, "waterMap", b_waterMap);
		shader.SetBuffer(transportStepKernelHandle, "waterMap", b_waterMap);
		shader.SetBuffer(evaporationStepKernelHandle, "waterMap", b_waterMap);
		shader.SetBuffer(waterStepKernelHandle, "oldWaterMap", b_old_waterMap);
		shader.SetBuffer(transportStepKernelHandle, "oldWaterMap", b_old_waterMap);
		shader.SetBuffer(waterStepKernelHandle, "sedimentMap", b_sedimentMap);
		shader.SetBuffer(transportStepKernelHandle, "sedimentMap", b_sedimentMap);
		shader.SetBuffer(evaporationStepKernelHandle, "sedimentMap", b_sedimentMap);

		shader.SetBuffer(waterStepKernelHandle, "originalHeightMap", b_originalHeightMap);
		shader.SetBuffer(waterStepKernelHandle, "layerHeights", b_layerHeights);
		shader.SetBuffer(waterStepKernelHandle, "layerSolubilities", b_layerSolubilities);

		shader.SetBuffer(evaporationStepKernelHandle, "temperatureMap", b_temperatureMap);
		shader.SetBuffer(waterStepKernelHandle, "precipitationMap", b_precipitationMap);

		//start by dispatching the water step
		shader.Dispatch (waterStepKernelHandle, 57, 19, 1);
		//transport the resting water
		shader.Dispatch (transportStepKernelHandle, 57, 19, 1);
		//finally, evaporate the water
		shader.Dispatch (evaporationStepKernelHandle, 57, 19, 1);

		//store the data from the heightmap buffer as the new eroded heightmap
		b_heightMap.GetData(heightMap);

		//store the data from both the watermap and sedimentmaps, ready for more iterations
		b_waterMap.GetData(waterMap);
		b_sedimentMap.GetData (sedimentMap);

		//dispose of the buffers
		b_heightMap.Dispose ();
		b_waterMap.Dispose ();
		b_sedimentMap.Dispose ();
		b_old_waterMap.Dispose ();
		b_originalHeightMap.Dispose ();
		b_layerHeights.Dispose ();
		b_layerSolubilities.Dispose ();
		b_temperatureMap.Dispose ();
		b_precipitationMap.Dispose ();
	}
}
