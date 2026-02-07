using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GridAudioPlayer : MonoBehaviour {

    [SerializeField]private List<string> _backgroundTracks;

    public UnityEvent OnPlayClickEffect;
    public UnityEvent OnPlayFinishEffect;


    void Start() {
        EventManager.OnLoadedNewLevel += PlayNextTrack;
        EventManager.OnClickTile += PlayClickEffect;
        EventManager.OnFinishedLevel += PlayFinishEffect;

        PlayNextTrack(0,0);
    }

    private void OnDestroy() {
        EventManager.OnLoadedNewLevel -= PlayNextTrack;
        EventManager.OnClickTile -= PlayClickEffect;
        EventManager.OnFinishedLevel -= PlayFinishEffect;
    }


    private void PlayFinishEffect(int level) {
        OnPlayFinishEffect?.Invoke();
    }

    private void PlayClickEffect() {
        OnPlayClickEffect?.Invoke();
    }

    private void PlayNextTrack(int currentLevel, int totalLevels) {
        if ( _backgroundTracks.Count > 0 ) {
            string track = _backgroundTracks[0];
            AudioManager.Instance.FadeOutAllLayers(0.5f);
            AudioManager.Instance.FadeInLayer(track,0.5f);
            _backgroundTracks.RemoveAt(0);
            _backgroundTracks.Add(track);
        }
    }
}
