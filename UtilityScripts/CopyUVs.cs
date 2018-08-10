using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CustomTools
{
	
	public class CopyUVs : MonoBehaviour {

		public void Copy(){
			Mesh mesh = GetComponent<MeshFilter> ().sharedMesh;
			mesh.uv = (Vector2[])mesh.uv2.Clone ();

		}
	}


	[CustomEditor(typeof(CopyUVs))]
	public class ObjectBuilderEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			CopyUVs myScript = (CopyUVs)target;
			if(GUILayout.Button("Copy UVs"))
			{
				myScript.Copy();
			}
		}
	}
}
