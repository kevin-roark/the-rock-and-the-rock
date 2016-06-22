using UnityEngine;
using System.Collections;

// useful scripting tutorial https://unity3d.com/learn/tutorials/topics/animation/animator-scripting?playlist=17099
// iTween docs http://itween.pixelplacement.com/documentation.php
public class DwayneCameraMovement : MonoBehaviour {

	Animator cameraAnim;
	public Animator characterAnim;
	bool hasCharacterAnim = false;

	public GameObject rockCamera;

	GameObject[] otherDwaynes;

	int speedHash = Animator.StringToHash("Speed");
	float currentSpeed = 0.0f;

	static bool testing = true;
	float riseSpeed = testing ? 1.0f : 7.0f;
	float runToMountainSpeed = testing ? 5.0f : 45.0f;
	float lookUpAtMountainSpeed = testing ? 2.0f : 7.0f;
	float climbTheMountainSpeed = testing ? 10.0f : 45.0f;
	float walkToCircleSpeed = testing ? 5.0f : 20.0f;
	float turnToFaceEdgeOfMountainSpeed = 5.0f;
	float runToEdgeOfMountainSpeed = testing ? 6.0f : 15.0f;

	void Start ()
	{
		cameraAnim = GetComponent<Animator>();

		otherDwaynes = GameObject.FindGameObjectsWithTag("OtherDwayne");

		StartCoroutine(StartAnimationChain());
	}

	IEnumerator StartAnimationChain () {
		// wait for a bit staring at ground, then wake the HELL up
		yield return new WaitForSeconds(2.0f);
		Rise();

		// after rising ends (should be smarter about this but whatever) let's start fuckin moving
		yield return new WaitForSeconds(riseSpeed + 3.0f);
		currentSpeed = 1.0f;
		RunToMountain();
		InvokeRepeating("SometimesDive", 0.0f, 3.0f);

		// stop just short of the mountain, stop running animation, look up
		yield return new WaitForSeconds(runToMountainSpeed);
		currentSpeed = 0.0f;
		CancelInvoke("SometimesDive");
		yield return new WaitForSeconds(1.0f);
		LookUpAtMountain();

		// after looking up at the mountain, start climbing it
		yield return new WaitForSeconds(lookUpAtMountainSpeed + 1.0f);
		ClimbTheMountain();

		// ater climbing the mountain, walk to where your friends wait for you
		yield return new WaitForSeconds(climbTheMountainSpeed);
		characterAnim.SetTrigger("StopClimb");
		yield return new WaitForSeconds(1.0f);
		currentSpeed = 0.5f;
		WalkToCircle();

		// after walking to circle, briefly wait then all sprint to edge of mountain
		yield return new WaitForSeconds(walkToCircleSpeed);
		currentSpeed = 0.0f;
		yield return new WaitForSeconds(3.0f);
		RotateOtherDwaynesToFaceEdgeOfMountain();
		yield return new WaitForSeconds(turnToFaceEdgeOfMountainSpeed);
		currentSpeed = 1.0f;
		setOtherDwaynesSpeed(1.0f);
		RunAllDwaynesToEdgeOfMountain();
	}
		
	void Update ()
	{
		hasCharacterAnim = characterAnim != null && characterAnim.isInitialized;

		if (hasCharacterAnim) {
			characterAnim.SetFloat(speedHash, currentSpeed);
		}
	}

	void Rise () {
		//cameraAnim.SetTrigger("WakeUp");

		iTween.RotateTo(gameObject, iTween.Hash(
			"rotation", new Vector3(532.7f, 234.0f, 179.3f),
			"time", riseSpeed,
			"easeType", "easeInQuad"
		));
	}

	void RunToMountain () {
		iTween.MoveTo(gameObject, iTween.Hash(
			"position", new Vector3(569.0f, 21.0f, 447.0f), 
			"time", runToMountainSpeed,
			"easeType", "linear"
		));
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
		iTween.MoveTo(gameObject, iTween.Hash(
			"position", new Vector3(680.5f, 407.15f, 517.0f),
			"time", walkToCircleSpeed,
			"easeType", "linear"
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
		RunDwayneToEdgeOfMountain(gameObject, 0.5f);

		for (int i = 0; i < otherDwaynes.Length; i++) {
			RunDwayneToEdgeOfMountain(otherDwaynes[i], 0.0f);
		}

		if (rockCamera != null) {
			iTween.MoveTo(rockCamera, iTween.Hash(
				"position", new Vector3(860.0f, rockCamera.transform.position.y + 70.0f, 620.0f),
				"time", runToEdgeOfMountainSpeed,
				"easeType", "easeInQuad"
			));
		}
	}

	void RunDwayneToEdgeOfMountain (GameObject dwayne, float delay) {
		Vector3 position = new Vector3(
			813.0f + Random.value * 20.0f - 10.0f,
			dwayne.transform.position.y,
			614.0f + Random.value * 20.0f - 10.0f
		);

		float speed = runToEdgeOfMountainSpeed - Random.value * 1.2f;

		iTween.MoveTo(dwayne, iTween.Hash(
			"position", position,
			"time", speed,
			"delay", delay,
			"easeType", "linear"
		));
	}

	void setOtherDwaynesSpeed(float speed) {
		for (int i = 0; i < otherDwaynes.Length; i++) {
			GameObject dwayne = otherDwaynes[i];
			Animator anim = dwayne.GetComponent<Animator>();
			if (anim) {
				anim.SetFloat(speedHash, speed);
			}
		}
	}
}
