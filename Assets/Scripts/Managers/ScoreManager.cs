using UnityEngine;

[System.Serializable]
public class ScoreData {
    public int score;
    public int currentLevel;
    public int unlockedLevels;
}

public class ScoreManager : MonoBehaviour {

    [Tooltip("Score added when the player finishes a level")]
    [SerializeField] private int _levelBaseScore;

    private ScoreData _scoreData;
    private int _tileClicks = 0;

    private void Awake() {
        EventManager.OnScoreLoaded += LoadScore;
        EventManager.OnClickTile += UpdateClickCount;
        EventManager.OnFinishedLevel += SetScore;
    }

    private void OnDestroy() {
        EventManager.OnScoreLoaded -= LoadScore;
        EventManager.OnClickTile -= UpdateClickCount;
        EventManager.OnFinishedLevel -= SetScore;
    }

    private void SetScore(int levelIndex) {
        int currentScore = Mathf.Max(_levelBaseScore/2, _levelBaseScore - _tileClicks * 10); // each click will reduce 10 points of score
        _scoreData.score = Mathf.Max(currentScore, _scoreData.score);
        _scoreData.currentLevel = levelIndex;
        if (levelIndex == _scoreData.unlockedLevels ) {
            _scoreData.unlockedLevels++;
        }
        _tileClicks = 0;
        EventManager.OnSetScore?.Invoke(_scoreData, currentScore); ;
    }

    private void UpdateClickCount() {
        _tileClicks++;
    }

    private void LoadScore(ScoreData scoreData) {
        _scoreData = scoreData;
    }
}
