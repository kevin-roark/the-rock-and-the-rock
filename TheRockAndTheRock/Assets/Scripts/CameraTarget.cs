using UnityEngine;
using System.Collections;

public class CameraTarget : MonoBehaviour {

	public Transform target;
	public bool active = true;

	private int deactivateInFrames = -1;

	void Update() {
		// Rotate the camera every frame so it keeps looking at the target 
		if (active) {
			transform.LookAt(target);
		}

		if (deactivateInFrames > 0) {
			deactivateInFrames -= 1;
			if (deactivateInFrames == 0) {
				active = false;
				deactivateInFrames = -1;
			}
		}
	}

	public void ActivateForFrames (int frames) {
		active = true;
		deactivateInFrames = frames;
	}
}
