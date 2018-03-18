using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

namespace UsefullScripts{

	public class PlayerController : NetworkBehaviour {
		public Camera camera;
		public AudioListener audioListener;
		public UnityStandardAssets.Characters.FirstPerson.FirstPersonController firstPersonController;

		// Use this for initialization
		void Start () {
	
		}
	
		// Update is called once per frame
		void Update () {
			if (isLocalPlayer) {
				firstPersonController.enabled = true;
				camera.enabled = true;
				audioListener.enabled = true;
			}
		}
	}
}
