using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (ChunkGenerator))]
public class ProcTerrGUI : Editor 
{
	public override void OnInspectorGUI()
	{
		ChunkGenerator c = (ChunkGenerator)target;

		DrawDefaultInspector ();
		if (GUILayout.Button ("Generate Terrain")) //Generate a new terrain with no erosion
		{
			c.Generate ();
		}
		if (GUILayout.Button ("Erode Terrain")) //Erode the existing terrain
		{
			c.ApplyErosion ();
		}
		if (GUILayout.Button ("Generate and Erode Terrain"))  //Generate and erode a new terrain
		{
			c.Generate ();
			c.ApplyErosion ();
		}
		if (GUILayout.Button ("Retexture Terrain")) //Rerun the texturing process
		{
			c.TextureAllChunks();
		}
	}

}
