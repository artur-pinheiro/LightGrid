using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridMenuAnimator))]
public class LevelsMenu : MonoBehaviour {

    private GridMenuAnimator _menuAnimator;

    private int _maxAvailableLevels;
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

    private void MoveToLevel(bool openedMenu) {
        if (!openedMenu) {
            if (_selectedLevelIndex >= 0) {
                EventManager.OnNewLevelSelected?.Invoke(_selectedLevelIndex);
            } else {
                gameObject.SetActive(false);
            }
        }
    }

    public void setLevelsNumber(int levelsNUmber) {
        _maxAvailableLevels = levelsNUmber;
    }

    public void OnCloseMenu(int levelIndex) {
        _selectedLevelIndex = levelIndex;
        _menuAnimator.CloseMenu();
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

        if ( _currentLevel == _unlockedLevels && _unlockedLevels < _maxAvailableLevels-1 ) {
            _unlockedLevels++;
            for ( int i = 0; i < _levelButtons.Length; i++ ) {
                _levelButtons[i].interactable = i <= _unlockedLevels;
            }
        }        

        _levelButtons[_currentLevel].interactable = false;
    }
}
