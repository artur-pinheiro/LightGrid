using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour {
    private string filePath;

    private void Awake() {
        filePath = Application.persistentDataPath + "/scores.json";

        EventManager.OnSetScore += SaveScore;
    }

    private void OnDestroy() {
        EventManager.OnSetScore -= SaveScore;
    }

    private void Start() {
        EventManager.OnScoreLoaded?.Invoke(LoadScores());
    }

    public void SaveScore(ScoreData newScoreData, int currentScore) {
        string json = JsonUtility.ToJson(newScoreData, true);
        System.IO.File.WriteAllText(filePath, json);
    }

    public ScoreData LoadScores() {
        if ( System.IO.File.Exists(filePath) ) {
            string json = System.IO.File.ReadAllText(filePath);
            return JsonUtility.FromJson<ScoreData>(json);
        }
        return new ScoreData();
    }

}
