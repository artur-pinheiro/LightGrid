using UnityEngine;

public class UIManager : MonoBehaviour {

    [SerializeField] private GameObject _endLevelMenu;
    [SerializeField] private LevelsMenu _levelsMenu;

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
        _endLevelMenu.SetActive(true);
    }
    
    public void ShowLevelsMenu(bool isCurrentLevelComplete) {
        if ( isCurrentLevelComplete ) {
            _levelsMenu.UpdateUnlockedLevels();
        }
        _levelsMenu.gameObject.SetActive(true);
    }
}
