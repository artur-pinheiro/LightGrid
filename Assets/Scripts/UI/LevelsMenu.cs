using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridMenuAnimator))]
public class LevelsMenu : MonoBehaviour {

    private GridMenuAnimator _menuAnimator;

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
                //move to the next level
            } else {
                gameObject.SetActive(false);
            }
        }
    }

    public void UpdateUnlockedLevels(int unlockedLevels, int currentLevel) {
        _levelButtons = GetComponentsInChildren<Button>();
        for ( int i = 0; i < _levelButtons.Length; i++ ) {
            _levelButtons[i].interactable = i < unlockedLevels;
        }
        _levelButtons[currentLevel].GetComponentInChildren<Image>().color = Color.yellow;
        _levelButtons[currentLevel].interactable = false;
        _unlockedLevels = unlockedLevels;
    }

    public void UpdateUnlockedLevels() {
        _levelButtons[++_unlockedLevels].interactable = true;
    }
}
