using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;

    [SerializeField] private AudioTable _audioTable;

    private AudioSourcePool _layerSourcePool;
    private AudioSourcePool _effectsSourcePool;

    private Dictionary<string, AudioSource> _activeLayerSources;
    private Dictionary<string, Coroutine> _fadingLayers;
    private float _layerMusicVolume = 1f;
    private float _mainSFXVolume = 1f;

    private void Awake() {
        if ( Instance == null ) {
            Instance = this;
            DontDestroyOnLoad( gameObject );
        } else 
            Destroy(gameObject);

        _layerSourcePool = gameObject.AddComponent<AudioSourcePool>();
        _effectsSourcePool = gameObject.AddComponent<AudioSourcePool>();
        _activeLayerSources = new Dictionary<string, AudioSource>();
        _fadingLayers = new Dictionary<string, Coroutine>();
    }

    /// <summary>
    /// Plays the Sound Effect identified by key. Will set the pitch as a random number beetween pitchRange[0] and pitchRange[1] in case they exist
    /// </summary>
    /// <param name="key">String identifier of the sound effect</param>
    /// <param name="pitchRange">Array delimiting the possible values for the pitch</param>
    public void PlaySFX(string key, float[] pitchRange = null) {

        if ( string.IsNullOrEmpty(key) )
            return;

        Audio effect = _audioTable.effectsList.FirstOrDefault(audio => audio.key == key);
        if ( effect.key == key) {
            AudioSource effectsSource = _effectsSourcePool.GetAudioSource();
            effectsSource.volume = _mainSFXVolume * effect.volume;
            effectsSource.loop = false;

            if ( pitchRange != null && pitchRange.Length >= 2 ) {
                effectsSource.pitch = Random.Range(pitchRange[0], pitchRange[1]);
            } else {
                effectsSource.pitch = 1f;
            }

            effectsSource.PlayOneShot(effect.clip);
            StartCoroutine(StopSFX(key, effectsSource, effect.clip.length));
        }
    }

    /// <summary>
    /// Plays the layer immediately
    /// </summary>
    /// <param name="key">String identifier of the audio layer</param>
    /// <returns>True if successful or already playing</returns>
    public bool PlayLayer(string key) {

        if ( string.IsNullOrEmpty(key) )
            return false;

        if (_activeLayerSources.ContainsKey(key)) {
            return true;        
        }

        Audio music = _audioTable.layersList.FirstOrDefault(audio => audio.key == key);
        if ( music.key == key ) {
            AudioSource musicSource = _layerSourcePool.GetAudioSource();
            _activeLayerSources.Add(key, musicSource);

            musicSource.volume = _layerMusicVolume * music.volume;
            musicSource.loop = true;
            musicSource.clip = music.clip;
            musicSource.pitch = 1f;
            musicSource.Play();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Stops the layer immediately
    /// </summary>
    /// <param name="key">String identifier of the audio layer</param>
    public void StopLayer(string key) {
        if ( !_activeLayerSources.ContainsKey(key) ) {
            return;
        }

        if ( _fadingLayers.ContainsKey(key)) {
            StopCoroutine(_fadingLayers[key] );
            _fadingLayers.Remove(key);
        }

        _activeLayerSources[key].Stop();
        _layerSourcePool.ReturnAudioSource(_activeLayerSources[key]);
        _activeLayerSources.Remove(key);
    }

    /// <summary>
    /// Sets the global volume for the audio layers. Each layer will still be affected by their own volume values
    /// </summary>
    /// <param name="volume">New volume value</param>
    public void SetLayersVolume(float volume) {
        _layerMusicVolume = volume;
    }

    /// <summary>
    /// Sets the global volume for the sound effects. Each sfx will still be affected by their own volume values
    /// </summary>
    /// <param name="volume">New volume value</param>
    public void SetSFXVolumes(float volume) {
        _mainSFXVolume = volume;
    }

    /// <summary>
    /// Pause all layers
    /// </summary>
    public void PauseLayerSources() {
        foreach ( KeyValuePair<string,AudioSource> layerSource in _activeLayerSources ) {
            layerSource.Value.Pause();
        }        
    }

    /// <summary>
    /// Resume all layers
    /// </summary>
    public void ResumeLayerSources() {
        foreach ( KeyValuePair<string, AudioSource> layerSource in _activeLayerSources ) {
            layerSource.Value.Play();
        }
    }

    /// <summary>
    /// Fades out the layer for the duration time
    /// </summary>
    /// <param name="key">String identifier of the audio layer</param>
    /// <param name="duration">Duration of the fade</param>
    public void FadeOutLayer(string key, float duration) {
        if ( !_fadingLayers.ContainsKey(key) && _activeLayerSources.ContainsKey(key) ) {
            _fadingLayers.Add(key, StartCoroutine(FadeLayerRoutine(key, false, duration)));
        }
    }

    /// <summary>
    /// Fades out all layers for the duration time
    /// </summary>
    /// <param name="duration">Duration of the fade</param>
    public void FadeOutAllLayers(float duration) {
        foreach ( KeyValuePair<string,AudioSource> layer in _activeLayerSources ) {
            FadeOutLayer(layer.Key,duration);
        }
    }

    /// <summary>
    /// Fades in the layer for the duration time
    /// </summary>
    /// <param name="key">String identifier of the audio layer</param>
    /// <param name="duration">Duration of the fade</param>
    public void FadeInLayer(string key, float duration) {
        if ( !_fadingLayers.ContainsKey(key) && PlayLayer(key)) {
            _fadingLayers.Add(key, StartCoroutine(FadeLayerRoutine(key, true, duration)));
        }
    }

    IEnumerator FadeLayerRoutine(string key, bool fadeIn, float duration) {
        AudioSource audioSource = _activeLayerSources[key];
        float endVolume = fadeIn? audioSource.volume : 0f;
        float startVolume = fadeIn ? 0 : audioSource.volume;
        float time = 0f;
        while ( time < duration ) {
            audioSource.volume = Mathf.Lerp(startVolume, endVolume, time/ duration);
            time += Time.deltaTime;
            yield return null;
        }
        audioSource.volume = endVolume;
        if ( audioSource.volume <= 0) {
            StopLayer(key);
        } else {
            _fadingLayers.Remove(key);
        }
    }

    IEnumerator StopSFX(string key, AudioSource effectsSource, float duration) {
        yield return new WaitForSeconds(duration);
        _effectsSourcePool.ReturnAudioSource(effectsSource);
    }
}