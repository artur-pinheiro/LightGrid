
using UnityEngine;

public class UIManager : MonoBehaviour {

    [SerializeField] private GameObject _endLevelMenu;

    void Awake() {
        EventManager.OnShowEndLevelUI += ShowEndLevelUI;
    }

    private void OnDestroy() {
        EventManager.OnShowEndLevelUI -= ShowEndLevelUI;
    }

    private void ShowEndLevelUI() {
        _endLevelMenu.SetActive(true);
    }

    public void OnNextLevel() { }

    public void OnOpenLevelMenu() { }
}
