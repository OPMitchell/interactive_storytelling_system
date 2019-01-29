using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A layer of terrain material with specified qualities
/// </summary>
[System.Serializable]
public class Layer
{
	[Tooltip("A layer of terrain material with specified qualities")]

	public string name; //name of the layer
	[Range(0,1)]
	public float solubility; //solubility of the layer for use in hydraulic erosion
	[Range(0,1)]
	public float maxHeight; //maximum height that the layer appears at
}

/// <summary>
/// A class that holds a collection of Layer objects, forming a layered data representation of a terrain
/// </summary>
public class TerrainLayers : MonoBehaviour 
{
	[SerializeField] private Layer[] Layers; //An array of Layer objects that comprise the terrain

	/// <summary>
	/// Gets the layers.
	/// </summary>
	/// <returns>The layers.</returns>
	public Layer[] GetLayers()
	{
		return Layers;
	}

	/// <summary>
	/// Gets the layer heights.
	/// </summary>
	/// <returns>The layer heights.</returns>
	public float[] GetLayerHeights()
	{
		Layer[] layers = GetLayers ();
		float[] layerHeights = new float[layers.Length];
		for (int i = 0; i < layers.Length; i++) //iterate through the array of Layers and get each one's maxHeight property
		{
			layerHeights [i] = layers [i].maxHeight;
		}
		return layerHeights;
	}

	/// <summary>
	/// Gets the layer solubilities.
	/// </summary>
	/// <returns>The layer solubilities.</returns>
	public float[] GetLayerSolubilities()
	{
		Layer[] layers = GetLayers ();
		float[] layerSols = new float[layers.Length];
		for (int i = 0; i < layers.Length; i++) //iterate through the array of Layers and get each one's solubility property
		{
			layerSols [i] = layers [i].solubility;
		}
		return layerSols;
	}

	/// <summary>
	/// Gets the number of layers.
	/// </summary>
	/// <returns>The number of layers.</returns>
	public int GetNumberOfLayers()
	{
		return Layers.Length;
	}
}

