using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]

public class VideoController : MonoBehaviour {

	public bool playAudio = true;

	MovieTexture movie;
	AudioSource movieAudio;

	void Start () {
		movie = GetComponent<Renderer>().material.mainTexture as MovieTexture;
		if (!movie.isPlaying) {
			movie.Play();
		}

		if (playAudio) {
			movieAudio = GetComponent<AudioSource>();
			movieAudio.clip = movie.audioClip;
			if (!movieAudio.isPlaying) {
				movieAudio.Play();
			}
		}
	}

	void Update () {

	}
}
