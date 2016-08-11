using UnityEngine;
using System.Collections;

public class ShowPanels : MonoBehaviour {

	public GameObject optionsPanel;							//Store a reference to the Game Object OptionsPanel 
	public GameObject optionsTint;							//Store a reference to the Game Object OptionsTint 
	public GameObject menuPanel;							//Store a reference to the Game Object MenuPanel 
	public GameObject pausePanel;							//Store a reference to the Game Object PausePanel 
	public GameObject goodbyePanel;

	void Start () {
		Events.instance.AddListener<PauseEvent>(HandlePause);
		Events.instance.AddListener<DwayneStateChangeEvent>(HandleDwayneState);
	}

	//Call this function to activate and display the Options panel during the main menu
	public void ShowOptionsPanel()
	{
		optionsPanel.SetActive(true);
		optionsTint.SetActive(true);
	}

	//Call this function to deactivate and hide the Options panel during the main menu
	public void HideOptionsPanel()
	{
		optionsPanel.SetActive(false);
		optionsTint.SetActive(false);
	}

	//Call this function to activate and display the main menu panel during the main menu
	public void ShowMenu()
	{
		menuPanel.SetActive (true);
	}

	//Call this function to deactivate and hide the main menu panel during the main menu
	public void HideMenu()
	{
		menuPanel.SetActive (false);
	}
	
	//Call this function to activate and display the Pause panel during game play
	public void ShowPausePanel()
	{
		pausePanel.SetActive (true);
		optionsTint.SetActive(true);
	}

	//Call this function to deactivate and hide the Pause panel during game play
	public void HidePausePanel()
	{
		pausePanel.SetActive (false);
		optionsTint.SetActive(false);
	}

	void HandlePause (PauseEvent e) {
		if (e.isPaused) {
			ShowPausePanel();
		} else {
			HidePausePanel();
		}
	}

	void HandleDwayneState (DwayneStateChangeEvent e) {
		if (e.state == DwayneState.FloatingToSky) {
			StartCoroutine(ShowCredits());
		}
	}

	IEnumerator ShowCredits () {
		// after 30 seconds of flying we show the credits
		yield return new WaitForSeconds (24.0f);

		goodbyePanel.SetActive(true);
	}
}
