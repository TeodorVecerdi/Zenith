using TMPro;
using UnityEngine;

public class GeneralManager : MonoBehaviour {
    [Header("Create Game References")] public TMP_InputField SaveName;
    public TMP_InputField SaveSeed;
    public GameManager GameManager;
    public GameStages GameStages;


    [Header("Loading")]
    public Transform LoadingTransform;
    public bool LoadingShouldChangeStatusText;
    public TMP_Text StatusText;
    public string LoadingStatusText;
    
    public Database Database;

    private void Update() {
        if (LoadingShouldChangeStatusText) {
            LoadingShouldChangeStatusText = false;
            StatusText.text = LoadingStatusText;
        }
    }
    
    public void CreateGame() {
        if (string.IsNullOrEmpty(SaveName.text) || string.IsNullOrEmpty(SaveSeed.text))
            return;
        GameManager.GameName = SaveName.text;
        GameManager.GameSeed = int.Parse(SaveSeed.text);
        GameStages.SwitchToStage(GameStages.MainMenuStage, GameStages.GameStage);
        GameManager.Initialize(Database);
        GameManager.Create();
    }

    public void SetStatus(string status) {
        LoadingStatusText = status;
        LoadingShouldChangeStatusText = true;
    }

    public void StartLoading() {
        LoadingTransform.localScale = Vector3.one;
    }

    public void EndLoading() {
        LoadingTransform.localScale = Vector3.zero;
    }

}