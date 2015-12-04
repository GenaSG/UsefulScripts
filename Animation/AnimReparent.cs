using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AnimReparent : MonoBehaviour {
	public struct Curves
	{
		public AnimationCurve bonePosX;
		public AnimationCurve bonePosY;
		public AnimationCurve bonePosZ;
		
		public AnimationCurve boneRotX;
		public AnimationCurve boneRotY;
		public AnimationCurve boneRotZ;
		public AnimationCurve boneRotW;
	}
	
	public Transform[] bones;
	public string[] bonesNames;
	public Vector3[] offset;
	public Vector3[] offsetAngles;
	public Transform[] parents;
	public string[] pathsToParent;
	public bool[] preserveParentAngles;
	public AnimationClip clip;
	private bool record = false;
	private float time = 0;
	private Curves[] curves;

	void StartRecording(){
		record = true;
		time = 0;
		curves = new Curves[bones.Length];
		for (int i = 0; i < curves.Length; i ++) {
			curves [i].bonePosX = new AnimationCurve ();
			curves [i].bonePosY = new AnimationCurve ();
			curves [i].bonePosZ = new AnimationCurve ();
			
			curves [i].boneRotX = new AnimationCurve ();
			curves [i].boneRotY = new AnimationCurve ();
			curves [i].boneRotZ = new AnimationCurve ();
			curves [i].boneRotW = new AnimationCurve ();
		}
	}

	void Record(){
		record = true;
	}


	// Update is called once per frame
	void LateUpdate () {
		if (!record) {
			return;
		}


		for (int i = 0; i < bones.Length; i ++) {
			if (preserveParentAngles[i]) {
				parents[i].rotation = parents[i].root.rotation;
			}

			Vector3 _offset = Vector3.zero;
			Vector3 _offsetAngles = Vector3.zero;
			if((offset.Length-1)>=i){
				_offset = offset[i];
			}
			if((offsetAngles.Length-1)>=i){
				_offsetAngles = offsetAngles[i];
			}

			Vector3 _targetPosition = parents[i].InverseTransformPoint(bones[i].position) + _offset;
			Quaternion _targetRotation = Quaternion.LookRotation(parents[i].InverseTransformDirection(bones[i].forward),parents[i].InverseTransformDirection(bones[i].up)) * Quaternion.Euler(_offsetAngles);

			curves [i].bonePosX.AddKey (time,_targetPosition.x);
			curves [i].bonePosY.AddKey (time,_targetPosition.y);
			curves [i].bonePosZ.AddKey (time,_targetPosition.z);

			curves [i].boneRotX.AddKey (time,_targetRotation.x);
			curves [i].boneRotY.AddKey (time,_targetRotation.y);
			curves [i].boneRotZ.AddKey (time,_targetRotation.z);
			curves [i].boneRotW.AddKey (time,_targetRotation.w);

			string _boneName = bones[i].name;
			if((bonesNames.Length-1)>=i){
				_boneName = bonesNames[i];
			}

			clip.SetCurve(pathsToParent [i] +"/"+_boneName,typeof(Transform),"localPosition.x",curves [i].bonePosX);
			clip.SetCurve(pathsToParent [i] +"/"+_boneName,typeof(Transform),"localPosition.y",curves [i].bonePosY);
			clip.SetCurve(pathsToParent [i] + "/"+_boneName,typeof(Transform),"localPosition.z",curves [i].bonePosZ);

			
			clip.SetCurve(pathsToParent [i]+"/"+_boneName,typeof(Transform),"localRotation.x",curves [i].boneRotX);
			clip.SetCurve(pathsToParent [i]+"/"+_boneName,typeof(Transform),"localRotation.y",curves [i].boneRotY);
			clip.SetCurve(pathsToParent [i]+"/"+_boneName,typeof(Transform),"localRotation.z",curves [i].boneRotZ);
			clip.SetCurve(pathsToParent [i]+"/"+_boneName,typeof(Transform),"localRotation.w",curves [i].boneRotW);
			

			clip.EnsureQuaternionContinuity ();

		}
		time += Time.deltaTime;
		record = false;
	}
}
