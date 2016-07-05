using UnityEngine;
using System.Collections;

public class SceneFading : MonoBehaviour {

	public Texture2D fadeOutTexture;
	public float fadeSpeed = 5.0f;

	private int drawDepth = -1000; // z-index, lower better
	private float alpha = 1.0f;
	private int fadeDir = -1; // -1 -> fade in | 1 -> fade out

	void OnGUI () {
		alpha = Mathf.Clamp01(alpha + fadeDir * fadeSpeed * Time.deltaTime);

		GUI.color = new Color (GUI.color.r, GUI.color.g, GUI.color.b, alpha);
		GUI.depth = drawDepth;
		GUI.DrawTexture(new Rect (0, 0, Screen.width, Screen.height), fadeOutTexture);
	}

	public float BeginFade (int direction) {
		fadeDir = direction;
		return fadeSpeed;
	}

	void OnLevelWasLoaded (int levelIndex) {
		BeginFade(-1);
	}
}
