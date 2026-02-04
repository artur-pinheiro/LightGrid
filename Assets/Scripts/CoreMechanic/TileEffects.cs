using UnityEngine;

public class TileEffects : MonoBehaviour {
    [SerializeField] private ParticleSystem[] _endLevelParticleSystems;

    void Awake() {
        EventManager.OnFinishedLevel += PlayEndLevelEffects;
    }

    private void OnDestroy() {
        EventManager.OnFinishedLevel -= PlayEndLevelEffects;
    }

    private void PlayEndLevelEffects() {
        foreach ( var effect in _endLevelParticleSystems ) {
            effect.Play();
        }
    }
}
