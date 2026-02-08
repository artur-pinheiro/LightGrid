using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    [SerializeField] private int _availableLevels;
    
    private int _levelIndex;
    private int _totalLamps;
    private int _energizedLamps;
    private bool _firstTimeLoad = true;

    void Awake() {
        EventManager.OnCountLamps += SetLampsCount;
        EventManager.OnEnergizeLamp += UpdateEnergizedLamps;
        EventManager.OnNewLevelSelected += LoadNextLevel;
        EventManager.OnScoreLoaded += SelectFirstLevel;
    }

    private void OnDestroy() {
        EventManager.OnCountLamps -= SetLampsCount;
        EventManager.OnEnergizeLamp -= UpdateEnergizedLamps;
        EventManager.OnNewLevelSelected -= LoadNextLevel;
        EventManager.OnScoreLoaded -= SelectFirstLevel;
    }

    private void SelectFirstLevel(ScoreData score) {
        if (_firstTimeLoad) {
            LoadNextLevel(score.currentLevel);
            _firstTimeLoad = false;
        }
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

        string currentScene = "Stage" + _levelIndex.ToString("00");
        if ( IsSceneLoaded(currentScene) ) 
            SceneManager.UnloadSceneAsync(currentScene);
        SceneManager.LoadScene("Stage" + nextLevel.ToString("00"), LoadSceneMode.Additive);

        _levelIndex = nextLevel;
        _energizedLamps = 0;

        EventManager.OnLoadedNewLevel?.Invoke(_levelIndex, _availableLevels);
    }

    public bool IsSceneLoaded(string sceneName) {
        for ( int i = 0; i < SceneManager.sceneCount; i++ ) {
            Scene scene = SceneManager.GetSceneAt(i);
            if ( scene.name == sceneName && scene.isLoaded ) {
                return true;
            }
        }
        return false;
    }
}
