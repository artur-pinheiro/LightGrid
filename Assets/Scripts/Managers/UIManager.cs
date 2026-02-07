using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    [SerializeField] private GameObject _inGameUI;
    [SerializeField] private GameObject _endLevelMenu;
    [SerializeField] private LevelsMenu _levelsMenu;
    [SerializeField] private Button _levelsMenuReturnButton;

    void Awake() {
        EventManager.OnLoadedNewLevel += UpdateLevelsMenu; 
        EventManager.OnLoadedNewLevel += ShowIngamelUI;
        EventManager.OnShowEndLevelUI += ShowEndLevelUI;

        UpdateLevelsMenu(0);
    }

    private void OnDestroy() {
        EventManager.OnLoadedNewLevel -= UpdateLevelsMenu; 
        EventManager.OnLoadedNewLevel -= ShowIngamelUI;
        EventManager.OnShowEndLevelUI -= ShowEndLevelUI;
    }

    private void UpdateLevelsMenu(int currentLevel) {
        _levelsMenu.SetCurrentLevelButton(currentLevel);
    }

    private void ShowEndLevelUI() {
        _inGameUI.SetActive(false);
        _endLevelMenu.SetActive(true);
        _levelsMenu.UpdateUnlockedLevels();
    }

    private void ShowIngamelUI(int currentLevel) {
        _inGameUI.SetActive(true);
        _levelsMenu.gameObject.SetActive(false);
        if ( _endLevelMenu.activeSelf ) {
            _endLevelMenu.GetComponent<Animator>().Play("Hide");
            Invoke(nameof(HideEndLevelUI),1.5f);
        }
    }

    private void HideEndLevelUI() {
        _endLevelMenu.SetActive(false);
    }

    public void ShowLevelsMenu(bool isCurrentLevelComplete) {

        _levelsMenuReturnButton.gameObject.SetActive(!isCurrentLevelComplete);
        _levelsMenuReturnButton.interactable = !isCurrentLevelComplete;
        //_levelsMenuReturnButton.GetComponent<CanvasGroup>().alpha = isCurrentLevelComplete ? 0 : 1;
        _levelsMenu.gameObject.SetActive(true);
    }
}
