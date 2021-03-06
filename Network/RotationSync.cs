﻿using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class RotationSync : NetworkBehaviour {
	
	public Transform XtargetTransform;
	public Transform YtargetTransform;
	public Transform ZtargetTransform;

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
			rotation = Quaternion.Euler(XtargetTransform.localEulerAngles.x,YtargetTransform.localEulerAngles.y,ZtargetTransform.localEulerAngles.z);
			if (hasAuthority) {
				Rpc_SendRotation(rotation.eulerAngles);
			} else {
				Cmd_SendRotation(rotation.eulerAngles);
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
				rotation = Quaternion.Euler(XtargetTransform.eulerAngles.x,YtargetTransform.eulerAngles.y,ZtargetTransform.eulerAngles.z);
				float _speed = Quaternion.Angle(rotation,receivedRot[0])/GetNetworkSendInterval();

				step = _speed * Time.fixedDeltaTime;
				updateTargetRot = false;
			}

			rotation = Quaternion.RotateTowards(rotation,receivedRot[0],step);
			
			XtargetTransform.localRotation = Quaternion.Euler(rotation.eulerAngles.x,XtargetTransform.localEulerAngles.y,XtargetTransform.localEulerAngles.z);
			YtargetTransform.localRotation = Quaternion.Euler(YtargetTransform.localEulerAngles.x,rotation.eulerAngles.y,YtargetTransform.localEulerAngles.z);
			ZtargetTransform.localRotation = Quaternion.Euler(ZtargetTransform.localEulerAngles.x,ZtargetTransform.localEulerAngles.y,rotation.eulerAngles.z);
			if(Quaternion.Angle(rotation,receivedRot[0]) < 0.1f){
				
				rotation = receivedRot[0];
				XtargetTransform.localRotation = Quaternion.Euler(rotation.eulerAngles.x,XtargetTransform.localEulerAngles.y,XtargetTransform.localEulerAngles.z);
				YtargetTransform.localRotation = Quaternion.Euler(YtargetTransform.localEulerAngles.x,rotation.eulerAngles.y,YtargetTransform.localEulerAngles.z);
				ZtargetTransform.localRotation = Quaternion.Euler(ZtargetTransform.localEulerAngles.x,ZtargetTransform.localEulerAngles.y,rotation.eulerAngles.z);
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