using System.Collections.Generic;
using UnityEngine;

public class AudioSourcePool:MonoBehaviour {
    private Queue<AudioSource> availableAudioSources = new Queue<AudioSource>();

    private void CreateNewAudioSource() {
        GameObject newAudioSourceObject = new GameObject("AudioSource");
        newAudioSourceObject.transform.parent = transform;
        AudioSource newAudioSource = newAudioSourceObject.AddComponent<AudioSource>();
        newAudioSourceObject.SetActive(false);
        availableAudioSources.Enqueue(newAudioSource);
    }

    public AudioSource GetAudioSource() {
        if ( availableAudioSources.Count == 0 ) {
            CreateNewAudioSource();
        }

        AudioSource audioSource = availableAudioSources.Dequeue();
        audioSource.gameObject.SetActive(true);
        return audioSource;
    }

    public void ReturnAudioSource(AudioSource audioSource) {
        audioSource.gameObject.SetActive(false);
        availableAudioSources.Enqueue(audioSource);
    }
}
