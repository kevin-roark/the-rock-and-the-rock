using UnityEngine;
using System.Collections;

public class MainDwayneMovement : MonoBehaviour {

	static int RUNNING_TO_MOUNTAIN = 0;
	static int CLIMBING_MOUNTAIN = 1;
	static int WAITING_ON_MOUNTAIN = 2;
	static int RUNNING_FROM_MOUNTAIN = 3;
	static int FALLING_FROM_MOUNTAIN = 4;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	static int nextState (int state) {
		return RUNNING_TO_MOUNTAIN;
	}
}
