using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class SaveManager : MonoBehaviour {
    private string filePath;

    private ScoreData _score;

    private void Awake() {
        filePath = Application.persistentDataPath + "/scores.json";

        EventManager.OnSetScore += StoreScoreData;
        EventManager.OnLoadedNewLevel += UpdateCurrentLevel;
        EventManager.OnUnlockedNewLevel += UpdateUnlockedLevels;
    }

    private void OnDestroy() {
        EventManager.OnSetScore -= StoreScoreData;
        EventManager.OnLoadedNewLevel -= UpdateCurrentLevel;
        EventManager.OnUnlockedNewLevel -= UpdateUnlockedLevels;
    }    

    private void Start() {
        _score = LoadScores();
        EventManager.OnScoreLoaded?.Invoke(_score);
    }

    private async void OnApplicationPause(bool pauseStatus) {
        if ( pauseStatus ) {
            await SaveGameAsync();
        }
    }

    private async void UpdateCurrentLevel(int currentLevel, int availableLevels) {
        _score.currentLevel = currentLevel;
        await SaveGameAsync();
    }

    private async void UpdateUnlockedLevels(int unlockedLevels) {
        _score.unlockedLevels = unlockedLevels;
        await SaveGameAsync();
    }

    public async void StoreScoreData(int newScoreRecord, int currentScore) {
        _score.score = newScoreRecord;
        await SaveGameAsync();
    }


    public async Task SaveGameAsync() {

        string json = JsonUtility.ToJson(_score, true);
        try {
            // WriteAllTextAsync writes the file asynchronously without blocking the main thread
            await File.WriteAllTextAsync(filePath, json);
            Debug.Log($"Game saved successfully to {filePath}");
        } catch ( System.Exception ex ) {
            Debug.LogError($"Failed to save game: {ex.Message}");
        }
    }

    public ScoreData LoadScores() {
        if ( File.Exists(filePath) ) {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<ScoreData>(json);
        }
        return new ScoreData();
    }

}
