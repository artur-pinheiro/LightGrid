using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridMenuAnimator))]
public class LevelsMenu : MonoBehaviour {

    private GridMenuAnimator _menuAnimator;

    private int _currentLevel;
    private int _unlockedLevels;
    private int _selectedLevelIndex = -1;
    private Button[] _levelButtons;

    private void Awake() {
        _menuAnimator ??= GetComponent<GridMenuAnimator>();
        _menuAnimator.OnFinishedAnimation += MoveToLevel;
    }

    private void OnEnable() {
        _menuAnimator ??= GetComponent<GridMenuAnimator>();
        _menuAnimator.OpenMenu();
    }

    public void OnCloseMenu(int levelIndex) {
        _selectedLevelIndex = levelIndex;
        _menuAnimator.CloseMenu();
    }

    private void MoveToLevel(bool openedMenu) {
        if (!openedMenu) {
            if (_selectedLevelIndex >= 0) {
                EventManager.OnNewLevelSelected?.Invoke(_selectedLevelIndex);
            } else {
                gameObject.SetActive(false);
            }
        }
    }

    public void SetCurrentLevelButton(int currentLevel) {
        _levelButtons ??= GetComponentsInChildren<Button>();
        _levelButtons[_currentLevel].GetComponentInChildren<Image>().color = Color.white;
        _levelButtons[_currentLevel].interactable = true;
        _currentLevel = currentLevel;
        _levelButtons[_currentLevel].GetComponentInChildren<Image>().color = Color.yellow;
        _levelButtons[_currentLevel].interactable = false;
    }

    public void UpdateUnlockedLevels() {
        _levelButtons ??= GetComponentsInChildren<Button>();
        _unlockedLevels++;
        for ( int i = 0; i < _levelButtons.Length; i++ ) {
            _levelButtons[i].interactable = i <= _unlockedLevels;
        }

        _levelButtons[_currentLevel].interactable = false;
    }
}
