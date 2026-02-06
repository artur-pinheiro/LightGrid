using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    [SerializeField] private int _levelIndex;

    private int _totalLamps;
    private int _energizedLamps;

    void Awake() {
        EventManager.OnCountLamps += SetLampsCount;
        EventManager.OnEnergizeLamp += UpdateEnergizedLamps;
    }

    private void OnDestroy() {
        EventManager.OnCountLamps -= SetLampsCount;
        EventManager.OnEnergizeLamp -= UpdateEnergizedLamps;
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

    public void LoadNextLevel() {
        SceneManager.LoadScene("Stage01");
    }

}
