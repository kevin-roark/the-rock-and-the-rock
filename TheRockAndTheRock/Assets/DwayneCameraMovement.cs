using UnityEngine;
using System.Collections;

// useful scripting tutorial https://unity3d.com/learn/tutorials/topics/animation/animator-scripting?playlist=17099
// iTween docs http://itween.pixelplacement.com/documentation.php
public class DwayneCameraMovement : MonoBehaviour {

	Animator cameraAnim;
	public Animator characterAnim;
	bool hasCharacterAnim = false;

	int speedHash = Animator.StringToHash("Speed");
	float currentSpeed = 0.0f;

	static bool testing = true;
	float riseSpeed = testing ? 1.0f : 7.0f;
	float runToMountainSpeed = testing ? 10.0f : 45.0f;
	float lookUpAtMountainSpeed = 7.0f;

	void Start ()
	{
		cameraAnim = GetComponent<Animator>();

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
}
