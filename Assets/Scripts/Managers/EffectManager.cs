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
        EventManager.OnFinishedLevel += PlayEndLevelEffects;
        EventManager.OnSetScore += UpdateScoreValue;
    }

    private void OnDestroy() {
        EventManager.OnFinishedLevel -= PlayEndLevelEffects;
        EventManager.OnSetScore -= UpdateScoreValue;
    }

    private void UpdateScoreValue(ScoreData scoreData, int currentScore) {
        _levelScore = currentScore;
    }

    private void PlayEndLevelEffects() {
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
}
