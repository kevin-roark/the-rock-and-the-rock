using UnityEngine;
using System.Collections;

public class CameraWork : MonoBehaviour {

	public Camera dwayneCamera;
	public Camera rockCamera;

	bool isCameraOnDwayne = true;
	bool isActive = false;

	void Start () {
		Events.instance.AddListener<GameFilmStartEvent>(StartGameFilm);
		Events.instance.AddListener<PauseEvent>(HandlePause);
	}

	void Update () {
		if (!isActive) return;
		
		// haha literally any input
		if (Input.anyKeyDown) {
			this.SwapCameras();
		}
	}

	void StartGameFilm (GameFilmStartEvent e) {
		isActive = true;
	}

	void HandlePause (PauseEvent e) {
		isActive = !e.isPaused;
	}

	void SwapCameras () {
		isCameraOnDwayne = !isCameraOnDwayne;

		dwayneCamera.enabled = isCameraOnDwayne;
		rockCamera.enabled = !isCameraOnDwayne;
	}
}
