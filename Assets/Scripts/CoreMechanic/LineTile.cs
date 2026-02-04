using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TileInput))]
[RequireComponent(typeof(TileSprite))]
[RequireComponent(typeof(TileRotator))]
public class LineTile : MonoBehaviour {

    [SerializeField] private LineElementSO _lineElement;
    private bool _energized;
    private int _rotations = 0;

    private TileInput _tileInput;
    private TileSprite _tileSprite;
    private TileRotator _tileRotator;

    public LineElementSO LineElement { get => _lineElement; }

    private void Awake() {
        _tileInput = GetComponent<TileInput>();
        _tileSprite = GetComponent<TileSprite>();
        _tileRotator = GetComponent<TileRotator>();

        _tileInput.OnClickTile += _tileRotator.AnimateRotation;
        _tileInput.OnClickTile += UpdateRotationCount;
        _tileRotator.OnFinishedRotating += _tileInput.ResetMovement;

        _tileSprite.SetLineElement(_lineElement);
        if( _lineElement.lineType == LineType.Source || _lineElement.lineType == LineType.Empty ) {
            _tileInput.EnableInput(false);
        } else
            _tileInput.EnableInput(true);
    }

    private void UpdateRotationCount() {
        _rotations++;
    }

    public void Energize(bool isEnergized) {
        _energized = isEnergized;
        _tileSprite.SetSprites(_energized);

        if ( _lineElement.lineType == LineType.Lamp ) {
            EventManager.OnEnergizeLamp?.Invoke(_energized);
        }
    }

    public bool[] GetCurrentNeighbors() { // Cyclic Array Rotation of the Neighbors array based on the current Rotation count

        int size = LineElement.neighborConnections.Count;
        bool[] result = new bool[size];
        int rotationAmount = ((_rotations % size) + size) % size; // normalize shift (positive, negative, or large) into the valid range [0, size-1]

        for ( int i = 0; i < size; i++ ) {
            result[(i + rotationAmount) % size] = LineElement.neighborConnections[i]; // move the value at i to i + rotationAmount and wraps it back into the valid range if it acceeds the list size
        }
        return result;
    }
}
