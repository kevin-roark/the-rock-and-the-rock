using UnityEngine;
using System.Collections;

public class CameraWork : MonoBehaviour {

	public Camera dwayneCamera;
	public Camera rockCamera;

	bool isCameraOnDwayne = true;

	void Update () {
		// haha literally any input
		if (Input.anyKeyDown) {
			this.SwapCameras();
		}
	}

	void SwapCameras () {
		isCameraOnDwayne = !isCameraOnDwayne;

		dwayneCamera.enabled = isCameraOnDwayne;
		rockCamera.enabled = !isCameraOnDwayne;
	}
}
