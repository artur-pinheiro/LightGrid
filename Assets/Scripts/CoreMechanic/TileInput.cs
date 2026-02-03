using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TileInput : MonoBehaviour, IPointerClickHandler {

    public bool inputEnabled;
    private bool _isMoving;

    public UnityAction OnClickTile;
    public UnityAction OnDisabledClick;

    public void OnPointerClick(PointerEventData eventData) {
        if (inputEnabled) {
            if (!_isMoving) {
                OnClickTile?.Invoke();
                _isMoving = true;
            }
        } else {
            OnDisabledClick?.Invoke();
        }
    }

    public void EnableInput(bool enable) {
        inputEnabled = enabled;
    }

    public void ResetMovement() {
        _isMoving = false;
    }
}
