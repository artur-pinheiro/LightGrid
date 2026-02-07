using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Audio {    
    public string key;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume;
}

[CreateAssetMenu(fileName = "AudioTable", menuName = "AudioTable", order = 1)]
public class AudioTable : ScriptableObject {
    public List<Audio> layersList;
    public List<Audio> effectsList;
}
