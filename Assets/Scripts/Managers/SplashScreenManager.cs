using UnityEngine;

public class SplashScreenManager : MonoBehaviour {
	private bool started;
	public GameStages switchStage;
	public GeneralManager GeneralManager;

	void Update () {
		if (Input.anyKeyDown && !started) {
			started = true;
			Database Database = new Database().Load(GeneralManager.SetStatus);
			GeneralManager.Database = Database;
			switchStage.SwitchToStage(switchStage.SplashScreenStage, switchStage.MainMenuStage);
		}
	}

	
}
