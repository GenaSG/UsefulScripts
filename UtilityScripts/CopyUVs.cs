using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CustomTools
{
	
	public class CopyUVs : MonoBehaviour {

		public void Copy(){
			Mesh mesh = GetComponent<MeshFilter> ().sharedMesh;
			mesh.uv = mesh.uv2;
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
