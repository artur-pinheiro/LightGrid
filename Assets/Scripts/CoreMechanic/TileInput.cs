using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TileInput : MonoBehaviour, IPointerClickHandler {

    private bool _inputEnabled;
    private bool _inputStopped;
    private bool _isMoving;

    public UnityAction OnClickTile;
    public UnityAction OnDisabledClick;

    private void Awake() {
        EventManager.OnFinishedLevel += StopInput;
    }
    private void OnDestroy() {
        EventManager.OnFinishedLevel -= StopInput;
    }

    public void OnPointerClick(PointerEventData eventData) {
        if ( _inputStopped ) return;

        if (_inputEnabled) {
            if (!_isMoving) {
                OnClickTile?.Invoke();
                EventManager.OnClickTile?.Invoke();
                _isMoving = true;
            }
        } else {
            OnDisabledClick?.Invoke();
        }
    }

    public void EnableInput(bool enable) {
        _inputEnabled = enable;
    }

    private void StopInput(int levelIndex) {
        _inputStopped = true;
    }

    public void ResetMovement() {
        _isMoving = false;
    }
}
