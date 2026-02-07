using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    [SerializeField] private int _levelIndex;

    private int _totalLamps;
    private int _energizedLamps;

    void Awake() {
        EventManager.OnCountLamps += SetLampsCount;
        EventManager.OnEnergizeLamp += UpdateEnergizedLamps;
        EventManager.OnNewLevelSelected += LoadNextLevel;
    }

    private void OnDestroy() {
        EventManager.OnCountLamps -= SetLampsCount;
        EventManager.OnEnergizeLamp -= UpdateEnergizedLamps;
        EventManager.OnNewLevelSelected -= LoadNextLevel;
    }

    private void SetLampsCount(int count) {
        _totalLamps = count;
    }

    private void UpdateEnergizedLamps(bool hasEnergized) {
        _energizedLamps = Mathf.Max(0, _energizedLamps + (hasEnergized ? 1 : -1));

        if(_energizedLamps == _totalLamps ) {
            FinishLevel();
        }
    }

    private void FinishLevel() {
        EventManager.OnFinishedLevel?.Invoke(_levelIndex);
    }

    public void LoadNextLevel(int nextLevelIndex) {
        int nextLevel = nextLevelIndex >= 0 ? nextLevelIndex : _levelIndex + 1;

        SceneManager.UnloadSceneAsync("Stage" + _levelIndex.ToString("00"));
        SceneManager.LoadScene("Stage" + nextLevel.ToString("00"), LoadSceneMode.Additive);

        _levelIndex = nextLevel;
        _energizedLamps = 0;

        EventManager.OnLoadedNewLevel?.Invoke(_levelIndex);
    }

}
