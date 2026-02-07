using UnityEngine;

public class SimpleSFXPlayer : MonoBehaviour {
    public string audioKey;

    public void Play() {
        AudioManager.Instance.PlaySFX(audioKey);
    }
}
