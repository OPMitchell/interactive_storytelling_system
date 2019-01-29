using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Diffusive erosion algorithm.
/// </summary>
public class DiffusiveErosion : MonoBehaviour
{
	[Range(0,1000)] [SerializeField]
	private int strength; // How many times to run the algorithm (strength of erosion).

	[Range(0.0f,0.0005f)] [SerializeField]
	private float materialMovementAmount; // Amount of material to move per iteration.

	[SerializeField]
	private ComputeShader shader; // Compute shader to run on the GPU

	/// <summary>
	/// Performs the erosion algorithm with the specified parameters, returning the eroded heightmap
	/// </summary>
	/// <param name="heightMap">Heightmap to perform erosion on</param>
	public float[,] Erode(float[,] heightMap)
	{
		//Check whether erosion needs to be performed
		if (strength > 0) 
		{
			//Execute the diffusive erosion compute shader on the GPU.
			RunOnGPU (heightMap);
		}

		//Return the eroded heightmap.
		return heightMap;
	}

	/// <summary>
	/// Runs the diffusive erosion compute shader on the GPU, hence parallelising the erosion method
	/// </summary>
	/// <param name="heightMap">Heightmap to perform erosion on</param>
    private void RunOnGPU (float[,] heightMap) 
	{
		//Create a read/writable buffer that contains the heightmap data and sends it to the GPU.
		//We need to specify the length of the buffer and the size of a single element. 
		//The buffer needs to be the same length as the heightmap, and each element in the heightmap is a //single float which is 4 bytes long.
		ComputeBuffer buffer = new ComputeBuffer (heightMap.Length, 4);
		//Set the initial data to be held in the buffer as the pre-generated heightmap
		buffer.SetData (heightMap);

		int kernelHandle = shader.FindKernel("CSMain"); //get the handle for the main kernel of the shader

		shader.SetBuffer(kernelHandle,"newTerrain", buffer); //set the heightmap buffer
		 
		//initalise the parameters needed for the algorithm
		shader.SetFloat ("c", materialMovementAmount); 
		shader.SetInt ("width", heightMap.GetLength(0)); 

		//Setup a stopwatch to calculate the runtime
		System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch ();
		stopWatch.Start (); //Start stopwatch

		//Execute the thermal erosion compute shader on the GPU for the specified number of iterations.
		for (int i = 0; i < strength; i++)
		{
			shader.Dispatch (kernelHandle, 57, 19, 1);
		}

		stopWatch.Stop (); //Stop stopwatch
		Debug.Log("Diffusive Erosion of " + strength + " iterations took " + stopWatch.Elapsed); //print runtime to user
		stopWatch.Reset ();

		buffer.GetData(heightMap); //Receive the eroded heightmap data from the buffer
		buffer.Dispose(); //Dispose of the buffer 
	}

}
