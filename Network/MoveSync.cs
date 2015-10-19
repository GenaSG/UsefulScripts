using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class MoveSync : NetworkBehaviour {

	public CharacterController characterController;
	public float overrideThreshold = 0.1f;
	public bool authoritativeCheck = false;

	private List<Vector3> receivedPos = new List<Vector3>();

	private bool updateTargetPos = true;
	private float step = 0;

	private bool playData = false;

	private float lastUpdateTime = 0;

	private Vector3 position;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (isLocalPlayer) {
			if(Time.time < (lastUpdateTime + GetNetworkSendInterval())){
				return;
			}
			if (hasAuthority) {
				Rpc_SendPosition(characterController.transform.localPosition);
			} else {
				Cmd_SendPosition(characterController.transform.localPosition);
			}
			lastUpdateTime = Time.time;
		} else {

			if(receivedPos.Count >= 2){
				playData = true;
			}
			if(receivedPos.Count == 0){
				playData = false;
			}

			if(!playData){
				return;
			}

			if(updateTargetPos){
				position = characterController.transform.localPosition;
				float _speed = Vector3.Distance(position,receivedPos[0])/GetNetworkSendInterval();

				step = _speed * Time.fixedDeltaTime;
				updateTargetPos = false;
			}

			position = Vector3.MoveTowards(position,receivedPos[0],step);

			characterController.Move(position - characterController.transform.localPosition);


			if (hasAuthority && authoritativeCheck) {
				if(Vector3.Distance(characterController.transform.localPosition,position) > overrideThreshold){
					Rpc_OverridePosition(characterController.transform.localPosition);
				}
			}

			if(Vector3.Distance(position,receivedPos[0]) < 0.001f){

				position = receivedPos[0];
				characterController.transform.localPosition = position;
				updateTargetPos = true;
				receivedPos.RemoveAt(0);
 
			}


		}
	}

	[Command]
	void Cmd_SendPosition(Vector3 _position){
		if (hasAuthority) {
			receivedPos.Add(_position);
		}
	}

	[ClientRpc]
	void Rpc_SendPosition(Vector3 _position){
		if (!hasAuthority && !isLocalPlayer) {
			receivedPos.Add(_position);
		}
	}

	[ClientRpc]
	void Rpc_OverridePosition(Vector3 _position){
		if (!hasAuthority) {
			characterController.transform.localPosition = _position;
		}
	}

}
