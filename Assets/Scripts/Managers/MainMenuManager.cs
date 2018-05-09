using UnityEngine;

public class MainMenuManager : MonoBehaviour {
    private bool isCreateGameMenuShown;
    
    public void ToggleCreateGameMenu() {
        if (isCreateGameMenuShown) {
            isCreateGameMenuShown = false;
            transform.Find("MainMenuButtons").gameObject.SetActive(true);
            transform.Find("CreateNewGamePanel").gameObject.SetActive(false);
        }
        else {
            isCreateGameMenuShown = true;
            transform.Find("CreateNewGamePanel").gameObject.SetActive(true);
            transform.Find("MainMenuButtons").gameObject.SetActive(false);
        }
    }
}
