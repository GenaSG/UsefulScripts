using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AnimReparent : MonoBehaviour {
	public Transform[] bones;
	public string[] bonesNames;
	public Vector3[] offset;
	public Vector3[] offsetAngles;
	public Transform parent;
	public string pathToParent;
	public bool preserveParentAngles = false;
	public AnimationClip clip;
	private bool record = false;
	private float time = 0;


	void StartRecording(){
		record = true;
		time = 0;

#if UNITY_EDITOR
		if(pathToParent.Length==0){
			pathToParent = AnimationUtility.CalculateTransformPath (parent, parent.root);
		}
#endif
	}

	void Record(){
		record = true;
	}


	// Update is called once per frame
	void LateUpdate () {
		if (!record) {
			return;
		}

		if (preserveParentAngles) {
			parent.rotation = parent.root.rotation;
		}

		for (int i = 0; i < bones.Length; i ++) {
			AnimationCurve bonePosX = new AnimationCurve();
			AnimationCurve bonePosY = new AnimationCurve();
			AnimationCurve bonePosZ = new AnimationCurve();
			
			AnimationCurve boneRotX = new AnimationCurve();
			AnimationCurve boneRotY = new AnimationCurve();
			AnimationCurve boneRotZ = new AnimationCurve();
			AnimationCurve boneRotW = new AnimationCurve();

			Vector3 _offset = Vector3.zero;
			Vector3 _offsetAngles = Vector3.zero;
			if((offset.Length-1)>=i){
				_offset = offset[i];
			}
			if((offsetAngles.Length-1)>=i){
				_offsetAngles = offsetAngles[i];
			}

			Vector3 _targetPosition = parent.InverseTransformPoint(bones[i].position) + _offset;
			Quaternion _targetRotation = Quaternion.LookRotation(parent.InverseTransformDirection(bones[i].forward),parent.InverseTransformDirection(bones[i].up)) * Quaternion.Euler(_offsetAngles);

			bonePosX.AddKey (time,_targetPosition.x);
			bonePosY.AddKey (time,_targetPosition.y);
			bonePosZ.AddKey (time,_targetPosition.z);

			boneRotX.AddKey (time,_targetRotation.x);
			boneRotY.AddKey (time,_targetRotation.y);
			boneRotZ.AddKey (time,_targetRotation.z);
			boneRotW.AddKey (time,_targetRotation.w);

			string _boneName = bones[i].name;
			if((bonesNames.Length-1)>=i){
				_boneName = bonesNames[i];
			}

			clip.SetCurve(pathToParent+"/"+_boneName,typeof(Transform),"localPosition.x",bonePosX);
			clip.SetCurve(pathToParent+"/"+_boneName,typeof(Transform),"localPosition.y",bonePosY);
			clip.SetCurve(pathToParent+"/"+_boneName,typeof(Transform),"localPosition.z",bonePosZ);

			
			clip.SetCurve(pathToParent+"/"+_boneName,typeof(Transform),"localRotation.x",boneRotX);
			clip.SetCurve(pathToParent+"/"+_boneName,typeof(Transform),"localRotation.y",boneRotY);
			clip.SetCurve(pathToParent+"/"+_boneName,typeof(Transform),"localRotation.z",boneRotZ);
			clip.SetCurve(pathToParent+"/"+_boneName,typeof(Transform),"localRotation.w",boneRotW);
			

			clip.EnsureQuaternionContinuity ();

		}
		time += Time.deltaTime;
		record = false;
	}
}
