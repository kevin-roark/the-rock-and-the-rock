using UnityEngine;
using System.Collections;

// useful scripting tutorial https://unity3d.com/learn/tutorials/topics/animation/animator-scripting?playlist=17099
// iTween docs http://itween.pixelplacement.com/documentation.php
public class DwayneCameraMovement : MonoBehaviour {
		
	Animator cameraAnim;
	public Animator characterAnim;
	bool hasCharacterAnim = false;

	public GameObject rockCamera;
	public GameObject wingsObject;

	GameObject[] otherDwaynes;
	GameObject[] allWings;

	int speedHash = Animator.StringToHash("Speed");
	float currentSpeed = 0.0f;
	bool hasStarted = false;

	public static bool testing = false;
	float riseSpeed = testing ? 3.0f : 6.0f;
	float lookAroundAfterRisingSpeed = 6.0f;
	float runToMountainSpeed = testing ? 5.0f : 45.0f;
	float lookUpAtMountainSpeed = testing ? 3.0f : 7.0f;
	float climbTheMountainSpeed = testing ? 5.0f : 35.0f;
	float walkToCircleSpeed = testing ? 5.0f : 17.5f;
	float turnToFaceEdgeOfMountainSpeed = 2.0f;
	float runToEdgeOfMountainSpeed = testing ? 5.0f : 16.0f;
	float jumpFromMountainSpeed = 1.5f;
	float fallFromMountainSpeed = testing ? 8.0f : 16.0f;
	float timeFallingWithWings = 6.0f;
	float flyToHeavenSpeed = testing ? 8.0f : 180.0f;

	void Start ()
	{
		cameraAnim = GetComponent<Animator>();

		otherDwaynes = GameObject.FindGameObjectsWithTag("OtherDwayne");
		allWings = new GameObject[otherDwaynes.Length + 1];

		// add wings to all dwaynes
		AddWingsToDwayne(GameObject.FindGameObjectWithTag("Player"), 0);
		for (int i = 0; i < otherDwaynes.Length; i++) {
			AddWingsToDwayne(otherDwaynes[i], i + 1);
		}
		SetWingsVisible(false);

		Events.instance.AddListener<GameFilmStartEvent>(StartGameFilm);
	}

	public void StartGameFilm (GameFilmStartEvent e) {
		if (hasStarted) return;
		hasStarted = true;
		StartCoroutine(StartAnimationChain());
	}

	void AddWingsToDwayne (GameObject dwayne, int idx) {
		GameObject wings = Object.Instantiate(wingsObject);
		wings.transform.parent = dwayne.transform;
		wings.transform.localRotation = new Quaternion();
		wings.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
		wings.transform.localPosition = new Vector3(0.0f, 1.056f, -0.238f);

		allWings[idx] = wings;
	}

	void SetWingsVisible (bool visible) {
		for (int i = 0; i < allWings.Length; i++) {
			allWings[i].transform.localScale = visible ? new Vector3(0.4f, 0.4f, 0.4f) : new Vector3(0.0f, 0.0f, 0.0f);
		}
	}

	IEnumerator StartAnimationChain () {
		// wait for a bit staring at ground, then wake the HELL up
		yield return new WaitForSeconds(1.5f);
		Rise();

		// after rising ends (should be smarter about this but whatever) let's start fuckin moving
		yield return new WaitForSeconds(riseSpeed);
		LookAround();

		yield return new WaitForSeconds(lookAroundAfterRisingSpeed);
		Events.instance.Raise(new DwayneStateChangeEvent(DwayneState.RunningInDesert));
		RunToMountain();

		// while we are running, zoom rock camera out
		StartCoroutine(LerpCameraFOV(rockCamera.GetComponent<Camera>(), 30.0f, runToMountainSpeed * 0.8f));

		// stop just short of the mountain, stop running animation, look up
		yield return new WaitForSeconds(runToMountainSpeed);
		currentSpeed = 0.0f;
		CancelInvoke("SometimesDive");
		characterAnim.SetTrigger("StopDive");
		yield return new WaitForSeconds(1.0f);
		LookUpAtMountain();

		// while looking up zoom camera in slightly
		StartCoroutine(LerpCameraFOV(rockCamera.GetComponent<Camera>(), 18.0f, lookUpAtMountainSpeed));

		// after looking up at the mountain, start climbing it
		yield return new WaitForSeconds(lookUpAtMountainSpeed + 1.0f);
		Events.instance.Raise(new DwayneStateChangeEvent(DwayneState.ClimbingMountain));
		ClimbTheMountain();

		// as we climb, zoom rock camera back in to a close level
		StartCoroutine(LerpCameraFOV(rockCamera.GetComponent<Camera>(), 7.25f, climbTheMountainSpeed * 0.8f));

		// ater climbing the mountain, walk to where your friends wait for you
		yield return new WaitForSeconds(climbTheMountainSpeed);
		characterAnim.SetTrigger("StopClimb");
		StartCoroutine(LerpCameraFOV(rockCamera.GetComponent<Camera>(), 20.0f, walkToCircleSpeed * 0.5f)); // zoom out for walk
		yield return new WaitForSeconds(1.0f);
		Events.instance.Raise(new DwayneStateChangeEvent(DwayneState.RunningToCircle));
		WalkToCircle();

		// after walking to circle, briefly wait then all sprint to edge of mountain
		yield return new WaitForSeconds(walkToCircleSpeed);
		Events.instance.Raise(new DwayneStateChangeEvent(DwayneState.ReachedCircle));
		currentSpeed = 0.0f;
		StartCoroutine(LerpCameraFOV(rockCamera.GetComponent<Camera>(), 7.25f, 4.0f)); // zoom back in for run to edge
		yield return new WaitForSeconds(5.0f);
		RotateOtherDwaynesToFaceEdgeOfMountain();
		yield return new WaitForSeconds(turnToFaceEdgeOfMountainSpeed);
		Events.instance.Raise(new DwayneStateChangeEvent(DwayneState.RunningToEdge));
		RunAllDwaynesToEdgeOfMountain();

		// after sprinting to edge... pause then fuckin jump
		yield return new WaitForSeconds(runToEdgeOfMountainSpeed);
		currentSpeed = 0.0f;
		setOtherDwaynesSpeed(0.0f);
		yield return new WaitForSeconds(1.8f);
		Events.instance.Raise(new DwayneStateChangeEvent(DwayneState.FallingFromEdge));
		JumpAllDwaynesFromEdgeOfMountain();
		SetDwaynesJumping(true);

		// after jumping set to falling
		yield return new WaitForSeconds(jumpFromMountainSpeed);
		SetDwaynesJumping(false);
		SetDwaynesFalling(true);

		// after falling gain wings and fuckin fly to the sky
		yield return new WaitForSeconds(fallFromMountainSpeed - timeFallingWithWings);
		SetWingsVisible(true);
		SetDwaynesFalling(false);
		Events.instance.Raise(new DwayneStateChangeEvent(DwayneState.FloatingToSky));
		yield return new WaitForSeconds(timeFallingWithWings);
		currentSpeed = 1.0f;
		setOtherDwaynesSpeed(1.0f);
		FlyAllDwaynesToHeaven();
	}
		
	void Update ()
	{
		hasCharacterAnim = characterAnim != null && characterAnim.isInitialized;

		if (hasCharacterAnim) {
			characterAnim.SetFloat(speedHash, currentSpeed);
		}
	}

	void Rise () {
		iTween.RotateTo(gameObject, iTween.Hash(
			"rotation", new Vector3(532.7f, 234.0f, 179.3f),
			"time", riseSpeed,
			"easeType", "easeInQuad"
		));
	}

	void LookAround () {
		iTween.RotateBy(gameObject, iTween.Hash(
			"amount", new Vector3(0, -0.12f, 0),
			"time", 1.25f,
			"easeType", "linear",
			"delay", 0.5f
		));
		iTween.RotateBy(gameObject, iTween.Hash(
			"amount", new Vector3(0, 0.2f, 0),
			"time", 2.0f,
			"delay", 2.25f,
			"easeType", "linear"
		));
		iTween.RotateBy(gameObject, iTween.Hash(
			"amount", new Vector3(0, -0.08f, 0),
			"time", 1.0f,
			"delay", 4.5f,
			"easeType", "linear"
		));
	}

	void RunToMountain () {
		currentSpeed = 1.0f;
		iTween.MoveTo(gameObject, iTween.Hash(
			"position", new Vector3(573.5f, 21.0f, 449.5f), 
			"time", runToMountainSpeed,
			"easeType", "linear"
		));

		InvokeRepeating("SometimesDive", 0.0f, 3.0f);
	}

	void SometimesDive () {
		if (hasCharacterAnim) {
			if (Random.value > 0.6) {
				characterAnim.SetTrigger("Dive");
			} else {
				characterAnim.SetTrigger("StopDive");
			}
		}
	}

	void LookUpAtMountain () {
		iTween.RotateTo(gameObject, iTween.Hash(
			"rotation", new Vector3(-55.0f, 421.0f, 2.1f),
			"time", lookUpAtMountainSpeed,
			"easeType", "linear"
		));
	}

	void ClimbTheMountain () {
		characterAnim.SetTrigger("Climb");
		iTween.MoveTo(gameObject, iTween.Hash(
			"position", new Vector3(621.0f, 407.2f, 482.6f),
			"time", climbTheMountainSpeed,
			"easeType", "linear"
		));
		iTween.RotateTo(gameObject, iTween.Hash(
			"rotation", new Vector3(0.0f, 423.0f, 0.0f),
			"time", climbTheMountainSpeed,
			"easeType", "easeInExpo"
		));
	}

	void WalkToCircle () {
		currentSpeed = 0.5f;
		iTween.MoveTo(gameObject, iTween.Hash(
			"position", new Vector3(680.5f, 407.15f, 517.0f),
			"time", walkToCircleSpeed,
			"easeType", "linear"
		));

		// move head once we get there
		iTween.RotateBy(gameObject, iTween.Hash(
			"amount", new Vector3(0, 0.1f, 0),
			"time", 1.5f,
			"delay", walkToCircleSpeed + 0.5f
		));
		iTween.RotateBy(gameObject, iTween.Hash(
			"amount", new Vector3(0, -0.2f, 0),
			"time", 2.5f,
			"delay", walkToCircleSpeed + 1.5f
		));
		iTween.RotateBy(gameObject, iTween.Hash(
			"amount", new Vector3(0, 0.1f, 0),
			"time", 1.0f,
			"delay", walkToCircleSpeed + 4.5f
		));
	}

	void RotateOtherDwaynesToFaceEdgeOfMountain () {
		for (int i = 0; i < otherDwaynes.Length; i++) {
			iTween.RotateTo(otherDwaynes[i], iTween.Hash(
				"rotation", new Vector3(0.0f, 423.0f, 0.0f),
				"time", turnToFaceEdgeOfMountainSpeed,
				"easeType", "easeInCubic"
			));
		}
	}

	void RunAllDwaynesToEdgeOfMountain () {
		currentSpeed = 1.0f;
		setOtherDwaynesSpeed(1.0f);

		RunDwayneToEdgeOfMountain(gameObject, 0.5f);

		for (int i = 0; i < otherDwaynes.Length; i++) {
			RunDwayneToEdgeOfMountain(otherDwaynes[i], 0.0f);
		}

		if (rockCamera != null) {
			iTween.MoveTo(rockCamera, iTween.Hash(
				"position", new Vector3(950.0f, 666.0f, 700.0f),
				"time", 2.0f
			));
		}
	}

	void RunDwayneToEdgeOfMountain (GameObject dwayne, float delay) {
		Vector3 position = new Vector3(
			813.0f + Random.value * 20.0f - 10.0f,
			dwayne.transform.position.y,
			614.0f + Random.value * 20.0f - 10.0f
		);

		float speed = runToEdgeOfMountainSpeed - delay - Random.value * (1.2f - delay);
		iTween.MoveTo(dwayne, iTween.Hash(
			"position", position,
			"time", speed,
			"delay", delay,
			"easeType", "linear"
		));
	}

	void JumpAllDwaynesFromEdgeOfMountain () {
		JumpDwayneFromEdgeOfMountain(gameObject);
		for (int i = 0; i < otherDwaynes.Length; i++) {
			JumpDwayneFromEdgeOfMountain(otherDwaynes[i]);
		}
	}

	void JumpDwayneFromEdgeOfMountain (GameObject dwayne) {
		// jump
		iTween.MoveBy(dwayne, iTween.Hash(
			"amount", new Vector3(53.0f + Random.value * 10.0f, 40.0f, 65.0f + Random.value * 10.0f),
			"time", jumpFromMountainSpeed,
			"easeType", "easeOutQuad"
		));

		// fall
		iTween.MoveBy(dwayne, iTween.Hash(
			"amount", new Vector3(15.0f + Random.value * 5.0f, -390.0f, 7.5f + Random.value * 2.5f),
			"time", fallFromMountainSpeed,
			"easeType", "easeInOutQuad",
			"delay", jumpFromMountainSpeed
		));
		iTween.RotateBy(dwayne, iTween.Hash(
			"amount", new Vector3(1.0f, 8.0f, -1.0f),
			"time", fallFromMountainSpeed,
			"delay", jumpFromMountainSpeed
		));
	}

	void FlyAllDwaynesToHeaven () {
		FlyDwayneToHeaven(gameObject);
		for (int i = 0; i < otherDwaynes.Length; i++) {
			FlyDwayneToHeaven(otherDwaynes[i]);
		}
	}

	void FlyDwayneToHeaven (GameObject dwayne) {
		iTween.MoveBy(dwayne, iTween.Hash(
			"amount", new Vector3(Random.value * 4.0f - 2.0f, 5000.0f, Random.value * 4.0f - 2.0f),
			"time", flyToHeavenSpeed,
			"easeType", "linear"
		));

//		iTween.RotateTo(dwayne, iTween.Hash(
//			"rotation", new Vector3(-0.2f, 3.52f, 0.0f),
//			"time", flyToHeavenSpeed * 0.25f,
//			"easeType", "easeOutCubic"
//		));

		// look at the rock
		CameraTarget target = dwayne.GetComponent<CameraTarget>();
		if (target) {
			target.ActivateForFrames(1200);
		}
	}

	void setOtherDwaynesSpeed (float speed) {
		for (int i = 0; i < otherDwaynes.Length; i++) {
			GameObject dwayne = otherDwaynes[i];
			Animator anim = dwayne.GetComponent<Animator>();
			if (anim) {
				anim.SetFloat(speedHash, speed);
			}
		}
	}

	void SetDwaynesFalling (bool falling) {
		SetDwaynesAnimationBool("Fall", falling);
	}

	void SetDwaynesJumping (bool jumping) {
		SetDwaynesAnimationBool("Jump", jumping);
	}

	void SetDwaynesAnimationBool (string name, bool value) {
		characterAnim.SetBool(name, value);

		for (int i = 0; i < otherDwaynes.Length; i++) {
			GameObject dwayne = otherDwaynes[i];
			Animator anim = dwayne.GetComponent<Animator>();
			if (anim) {
				anim.SetBool(name, value);
			}
		}
	}

	IEnumerator LerpCameraFOV (Camera cam, float fov, float speed) {
		float originalFOV = cam.fieldOfView;
		float totalTime = 0.0f;
		while (totalTime < speed) {
			totalTime = Mathf.Min(totalTime + Time.deltaTime, speed);
			float amt = Time.deltaTime / speed;
			cam.fieldOfView = Mathf.Lerp(originalFOV, fov, totalTime / speed);
			yield return null;
		}
	}
}
