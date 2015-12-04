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
	[System.Serializable]
	public struct BoneInfo
	{
		public Transform bone;
		public string newBoneName;
		public Vector3 offset;
		public Vector3 offsetAngles;
		public Transform newParent;
		public string pathToNewParent;
		public bool preserveRootRotation;
	}

	public BoneInfo[] boneInfo;
	public AnimationClip clip;
	private bool record = false;
	private float time = 0;
	private Curves[] curves;

	void StartRecording(){
		record = true;
		time = 0;
		curves = new Curves[boneInfo.Length];
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


		for (int i = 0; i < boneInfo.Length; i ++) {
			if (boneInfo[i].preserveRootRotation) {
				boneInfo[i].newParent.rotation = transform.rotation;
			}

			Vector3 _targetPosition = boneInfo[i].newParent.InverseTransformPoint(boneInfo[i].bone.position) + boneInfo[i].offset;
			Quaternion _targetRotation = Quaternion.LookRotation(boneInfo[i].newParent.InverseTransformDirection(boneInfo[i].bone.forward),boneInfo[i].newParent.InverseTransformDirection(boneInfo[i].bone.up)) * Quaternion.Euler(boneInfo[i].offsetAngles);

			curves [i].bonePosX.AddKey (time,_targetPosition.x);
			curves [i].bonePosY.AddKey (time,_targetPosition.y);
			curves [i].bonePosZ.AddKey (time,_targetPosition.z);

			curves [i].boneRotX.AddKey (time,_targetRotation.x);
			curves [i].boneRotY.AddKey (time,_targetRotation.y);
			curves [i].boneRotZ.AddKey (time,_targetRotation.z);
			curves [i].boneRotW.AddKey (time,_targetRotation.w);

			if(boneInfo[i].newBoneName.Length==0){
				boneInfo[i].newBoneName = boneInfo[i].bone.name;
			}

			clip.SetCurve(boneInfo[i].pathToNewParent +"/"+ boneInfo[i].newBoneName,typeof(Transform),"localPosition.x",curves [i].bonePosX);
			clip.SetCurve(boneInfo[i].pathToNewParent +"/"+ boneInfo[i].newBoneName,typeof(Transform),"localPosition.y",curves [i].bonePosY);
			clip.SetCurve(boneInfo[i].pathToNewParent + "/"+ boneInfo[i].newBoneName,typeof(Transform),"localPosition.z",curves [i].bonePosZ);

			
			clip.SetCurve(boneInfo[i].pathToNewParent+"/"+ boneInfo[i].newBoneName,typeof(Transform),"localRotation.x",curves [i].boneRotX);
			clip.SetCurve(boneInfo[i].pathToNewParent+"/"+ boneInfo[i].newBoneName,typeof(Transform),"localRotation.y",curves [i].boneRotY);
			clip.SetCurve(boneInfo[i].pathToNewParent+"/"+ boneInfo[i].newBoneName,typeof(Transform),"localRotation.z",curves [i].boneRotZ);
			clip.SetCurve(boneInfo[i].pathToNewParent+"/"+ boneInfo[i].newBoneName,typeof(Transform),"localRotation.w",curves [i].boneRotW);
			

			clip.EnsureQuaternionContinuity ();

		}
		time += Time.deltaTime;
		record = false;
	}
}
