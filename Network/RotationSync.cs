using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class RotationSync : NetworkBehaviour {
	
	public Transform targetTransform;

	private List<Quaternion> receivedRot = new List<Quaternion>();
	
	private bool updateTargetRot = true;
	private float step = 0;
	
	private bool playData = false;
	
	private float lastUpdateTime = 0;
	
	private Quaternion rotation;
	
	
	
	// Update is called once per frame
	void FixedUpdate () {
		if (isLocalPlayer) {
			if(Time.time < (lastUpdateTime + GetNetworkSendInterval())){
				return;
			}
			if (hasAuthority) {
				Rpc_SendRotation(targetTransform.eulerAngles);
			} else {
				Cmd_SendRotation(targetTransform.eulerAngles);
			}
			lastUpdateTime = Time.time;
		} else {
			
			if(receivedRot.Count >= 2){
				playData = true;
			}
			if(receivedRot.Count == 0){
				playData = false;
			}
			
			if(!playData){
				return;
			}
			
			if(updateTargetRot){
				rotation = targetTransform.rotation;
				float _speed = Quaternion.Angle(rotation,receivedRot[0])/GetNetworkSendInterval();

				step = _speed * Time.fixedDeltaTime;
				updateTargetRot = false;
			}

			rotation = Quaternion.RotateTowards(rotation,receivedRot[0],step);
			
			targetTransform.rotation = rotation;

			if(Quaternion.Angle(rotation,receivedRot[0]) < 0.1f){
				
				rotation = receivedRot[0];
				targetTransform.rotation = rotation;
				updateTargetRot = true;
				receivedRot.RemoveAt(0);
				
			}
			
			
		}
	}
	
	[Command]
	void Cmd_SendRotation(Vector3 _rotation){
		if (hasAuthority) {
			receivedRot.Add(Quaternion.Euler(_rotation));
		}
	}
	
	[ClientRpc]
	void Rpc_SendRotation(Vector3 _rotation){
		if (!hasAuthority && !isLocalPlayer) {

			receivedRot.Add(Quaternion.Euler(_rotation));
		}
	}
	
	
}