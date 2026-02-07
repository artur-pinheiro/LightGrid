using System.Collections.Generic;
using UnityEngine;

public class QueueSFXPlayer : MonoBehaviour {

    [SerializeField] private List<string> _effectQueue;

    public void PlayNext() {
        if ( _effectQueue.Count > 0 ) {
            string effect = _effectQueue[0];
            AudioManager.Instance.PlaySFX(effect);
            _effectQueue.RemoveAt(0);
            _effectQueue.Add(effect);
        }        
    }

}
