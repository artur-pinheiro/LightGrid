using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    [SerializeField] private int _availableLevels;
    
    private int _levelIndex;
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
        int nextLevel;
        if ( nextLevelIndex < 0 ) { //nextLevelIndex will be -1 if coming from the "Continue" button, indicating we just have to go to the next level ir order, otherwise we move to a specific level selected in the LevelsMenu
            if ( _levelIndex == _availableLevels - 1 )  // out of levels, loop to the first
                nextLevel = 0;
            else
                nextLevel = _levelIndex + 1;
        } else
            nextLevel = nextLevelIndex;

        SceneManager.UnloadSceneAsync("Stage" + _levelIndex.ToString("00"));
        SceneManager.LoadScene("Stage" + nextLevel.ToString("00"), LoadSceneMode.Additive);

        _levelIndex = nextLevel;
        _energizedLamps = 0;

        EventManager.OnLoadedNewLevel?.Invoke(_levelIndex, _availableLevels);
    }

}
