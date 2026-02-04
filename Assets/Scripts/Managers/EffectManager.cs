using UnityEngine;

public class EffectManager : MonoBehaviour {

    [SerializeField] private ParticleSystem[] _endLevelParticleSystems;
    [SerializeField] private Animator _cameraAnimator;

    [SerializeField] private float _endLevelDelay;

    void Awake() {
        EventManager.OnFinishedLevel += PlayEndLevelEffects;
    }

    private void OnDestroy() {
        EventManager.OnFinishedLevel -= PlayEndLevelEffects;
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
    }
}
