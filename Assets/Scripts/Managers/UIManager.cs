using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    [SerializeField] private GameObject _inGameUI;
    [SerializeField] private GameObject _endLevelMenu;
    [SerializeField] private LevelsMenu _levelsMenu;
    [SerializeField] private Button _levelsMenuReturnButton;

    void Awake() {
        EventManager.OnScoreLoaded += UpdateLevelsMenu;
        EventManager.OnShowEndLevelUI += ShowEndLevelUI;
    }

    private void OnDestroy() {
        EventManager.OnScoreLoaded -= UpdateLevelsMenu;
        EventManager.OnShowEndLevelUI -= ShowEndLevelUI;
    }

    private void UpdateLevelsMenu(ScoreData scoreData) {
        _levelsMenu.UpdateUnlockedLevels(scoreData.unlockedLevels, scoreData.currentLevel);
    }

    private void ShowEndLevelUI() {
        _inGameUI.SetActive(false);
        _endLevelMenu.SetActive(true);
    }
    
    public void ShowLevelsMenu(bool isCurrentLevelComplete) {

        _levelsMenuReturnButton.gameObject.SetActive(!isCurrentLevelComplete);
        _levelsMenuReturnButton.interactable = !isCurrentLevelComplete;
        if ( isCurrentLevelComplete ) {
            _levelsMenu.UpdateUnlockedLevels();
        }
        _levelsMenu.gameObject.SetActive(true);
    }
}
