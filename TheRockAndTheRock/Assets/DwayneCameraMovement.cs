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

	static bool testing = true;
	float riseSpeed = testing ? 1.0f : 7.0f;
	float runToMountainSpeed = testing ? 1.0f : 45.0f;
	float lookUpAtMountainSpeed = testing ? 1.0f : 7.0f;
	float climbTheMountainSpeed = testing ? 1.0f : 45.0f;
	float walkToCircleSpeed = testing ? 1.0f : 20.0f;
	float turnToFaceEdgeOfMountainSpeed = 1.0f;
	float runToEdgeOfMountainSpeed = testing ? 1.0f : 15.0f;
	float jumpFromMountainSpeed = 1.5f;
	float fallFromMountainSpeed = testing ? 8.0f : 24.0f;
	float timeFallingWithWings = 2.0f;
	float flyToHeavenSpeed = testing ? 8.0f : 60.0f;

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

		// after sprinting to edge... pause then fuckin jump
		yield return new WaitForSeconds(runToEdgeOfMountainSpeed);
		currentSpeed = 0.0f;
		setOtherDwaynesSpeed(0.0f);
		yield return new WaitForSeconds(3.0f);
		JumpAllDwaynesFromEdgeOfMountain();

		// after falling gain wings and fuckin fly to the sky
		yield return new WaitForSeconds(jumpFromMountainSpeed + fallFromMountainSpeed - timeFallingWithWings);
		SetWingsVisible(true);
		yield return new WaitForSeconds(timeFallingWithWings);
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
			"amount", new Vector3(0.0f, -390.0f, 0.0f),
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
			"amount", new Vector3(Random.value * 20.0f - 10.0f, 1000.0f, Random.value * 20.0f - 10.0f),
			"time", flyToHeavenSpeed,
			"easeType", "linear"
		));
		iTween.RotateTo(dwayne, iTween.Hash(
			"rotation", new Vector3(),
			"time", flyToHeavenSpeed,
			"easeType", "easeOutCubic"
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
