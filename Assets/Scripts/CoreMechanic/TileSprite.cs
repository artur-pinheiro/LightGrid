using UnityEngine;

public class TileSprite : MonoBehaviour {

    private LineElementSO _lineElement;

    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private SpriteRenderer _iconRenderer;

    public void SetLineElement(LineElementSO lineElement) {
        _lineElement = lineElement;
    }

    public void SetSprites(bool energized) {

        if ( _renderer == null) {
            Debug.LogError($"Missing Renderer on Tile: {gameObject.name}");
            return;
        }

        if ( energized ) {
            _renderer.sprite = _lineElement.onSprite;
            if ( _iconRenderer != null )
                _iconRenderer.sprite = _lineElement.iconOnSprite;
        } else {
            _renderer.sprite = _lineElement.offSprite;
            if ( _iconRenderer != null )
                _iconRenderer.sprite = _lineElement.iconOffSprite;
        }    
    }
}
