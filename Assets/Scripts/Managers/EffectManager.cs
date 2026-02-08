using System;
using UnityEngine;

public class EffectManager : MonoBehaviour {

    [SerializeField] private ParticleSystem[] _endLevelParticleSystems;
    [SerializeField] private Animator _cameraAnimator;
    [SerializeField] private ScoreSliderController _sliderController;

    [SerializeField] private float _endLevelDelay;
    [SerializeField] private float _scoreAnimationDuration;

    private int _levelScore;

    void Awake() {
        EventManager.OnLoadedNewLevel += ResetCamera;
        EventManager.OnFinishedLevel += PlayEndLevelEffects;
        EventManager.OnSetScore += UpdateScoreValue;
    }

    private void OnDestroy() {
        EventManager.OnLoadedNewLevel -= ResetCamera;
        EventManager.OnFinishedLevel -= PlayEndLevelEffects;
        EventManager.OnSetScore -= UpdateScoreValue;
    }

    private void UpdateScoreValue(int scoreRecord, int currentScore) {
        _levelScore = currentScore;
    }

    private void PlayEndLevelEffects(int levelIndex) {
        foreach (var effect in _endLevelParticleSystems) {
            effect.Play(); 
        }
        _cameraAnimator.Play("cameraBounce");

        Invoke(nameof(ShowEndLevelUI),_endLevelDelay);
    }

    private void ShowEndLevelUI() {
        _cameraAnimator.Play("cameraDecend");
        EventManager.OnShowEndLevelUI?.Invoke();
        _sliderController.AnimateSlider(_levelScore, _scoreAnimationDuration);
    }

    private void ResetCamera(int currentLevel, int maxLevelsNumber) {
        _cameraAnimator.Play("cameraAscend");
    }
}
