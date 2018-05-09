using UnityEngine;

public class GameStages : MonoBehaviour {

	public GameObject[] SplashScreenStage;
	public GameObject[] MainMenuStage;
	public GameObject[] GameStage;

	public void SwitchToStage(GameObject[] from, GameObject[] to) {
		foreach (var disable in from) {
			disable.SetActive(false);
		}
		foreach (var enable in to) {
			enable.SetActive(true);
		}
	}
}
