using UnityEngine;

[RequireComponent(typeof(GridMenuAnimator))]
public class LevelsMenu : MonoBehaviour {

    private GridMenuAnimator _menuAnimator;

    private int _nextLevelIndex = -1;

    private void Awake() {
        _menuAnimator ??= GetComponent<GridMenuAnimator>();
        _menuAnimator.OnFinishedAnimation += MoveToLevel;
    }

    private void OnEnable() {
        _menuAnimator ??= GetComponent<GridMenuAnimator>();
        _menuAnimator.OpenMenu();
    }


    public void OnCloseMenu(int levelIndex) {
        _nextLevelIndex = levelIndex;
        _menuAnimator.CloseMenu();
    }

    private void MoveToLevel(bool openedMenu) {
        if (!openedMenu) {
            if (_nextLevelIndex >= 0) {
                //move to the next level
            } else {
                gameObject.SetActive(false);
            }
        }
    }
}
