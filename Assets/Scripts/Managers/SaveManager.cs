using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour {
    private string filePath;

    private ScoreData _score;

    private void Awake() {
        filePath = Application.persistentDataPath + "/scores.json";

        EventManager.OnSetScore += StoreScoreData;
    }

    private void OnDestroy() {
        EventManager.OnSetScore -= StoreScoreData;
        SaveScoreData();
    }

    private void Start() {
        _score = LoadScores();
        EventManager.OnScoreLoaded?.Invoke(_score);
    }

    public void StoreScoreData(ScoreData newScoreData, int currentScore) {
        _score = newScoreData;
    }

    public void SaveScoreData() {
        string json = JsonUtility.ToJson(_score, true);
        File.WriteAllText(filePath, json);
    }

    public ScoreData LoadScores() {
        if ( File.Exists(filePath) ) {
            string json = File.ReadAllText(filePath);
            return JsonUtility.FromJson<ScoreData>(json);
        }
        return new ScoreData();
    }

}
