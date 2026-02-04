using UnityEngine;

public class TileSprite : MonoBehaviour {

    private LineElementSO _lineElement;

    //We set them separatedly so we can have other sprites that will not be affected
    [SerializeField] private SpriteRenderer _renderer;

    public void SetLineElement(LineElementSO lineElement) {
        _lineElement = lineElement;
    }

    public void SetSprites(bool energized) {
        if ( energized ) {
            _renderer.sprite = _lineElement.onSprite;
        } else {
            _renderer.sprite = _lineElement.offSprite;
        }    
    }
}
