using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class AudioScript : MonoBehaviour {

	public AudioSource rockSource;
	public AudioSource breathingSource;

	private enum AudioState { Off, FadingOut, FadingIn, On }

	private AudioState rockState = AudioState.Off;
	private AudioState breathingState = AudioState.Off;

	private float resetTime = 0.01f;	// Very short time used to fade in near instantly without a click
	private float fadeSpeed = 0.2f;
	private bool hasReachedCircle = false;

	void Awake ()  {
		Events.instance.AddListener<PauseEvent>(handlePause);
		Events.instance.AddListener<GameFilmStartEvent>(handleGameStart);
		Events.instance.AddListener<DwayneStateChangeEvent>(handleDwayneState);

		setState(rockSource, AudioState.On);
		setState(breathingSource, AudioState.Off);
	}

	void Update () {
		updateSource(rockSource, rockState);
		updateSource(breathingSource, breathingState);
	}

	void updateSource (AudioSource source, AudioState state) {
		switch (state) {
		case AudioState.Off:
			break;
		case AudioState.On:
			break;

		case AudioState.FadingIn:
			source.volume = Mathf.Min(source.volume + fadeSpeed * Time.deltaTime, 1.0f);
			if (source.volume >= 0.99f) {
				setState(source, AudioState.On);
			}
			break;

		case AudioState.FadingOut:
			source.volume = Mathf.Max(source.volume - fadeSpeed * Time.deltaTime, 0.0f);
			if (source.volume <= 0.01f) {
				setState(source, AudioState.Off);
			}
			break;
		}
	}

	void setState (AudioSource source, AudioState state) {
		if (source == rockSource) {
			rockState = state;
		} else if (source == breathingSource) {
			breathingState = state;
		}

		switch (state) {
		case AudioState.Off:
			source.volume = 0.0f;
			source.Pause();
			break;

		case AudioState.FadingIn:
			source.Play();
			break;

		case AudioState.FadingOut:
			if (!source.isPlaying) {
				source.Play();
			}
			break;
			
		case AudioState.On:
			source.volume = 1.0f;
			if (!source.isPlaying) {
				source.Play();
			}
			break;
		}
	}

	void handleGameStart (GameFilmStartEvent e) {
		setState(rockSource, AudioState.FadingOut);
		setState(breathingSource, AudioState.FadingIn);
	}

	void handlePause (PauseEvent e) {
		if (e.isPaused) {
			setState(rockSource, AudioState.On);
			setState(breathingSource, AudioState.Off);
		} else {
			if (!hasReachedCircle) {
				setState(rockSource, AudioState.FadingOut);
			}
			setState(breathingSource, AudioState.FadingIn);
		}
	}

	void handleDwayneState (DwayneStateChangeEvent e) {
		if (e.state == DwayneState.ReachedCircle) {
			hasReachedCircle = true;
			setState(rockSource, AudioState.FadingIn);
		}
	}
}
