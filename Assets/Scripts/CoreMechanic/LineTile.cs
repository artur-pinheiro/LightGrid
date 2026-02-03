using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(TileInput))]
[RequireComponent(typeof(TileSprite))]
[RequireComponent(typeof(TileRotator))]
public class LineTile : MonoBehaviour {

    [SerializeField] private LineElementSO _lineElement;
    private bool _energized;
    private int _rotations;

}
