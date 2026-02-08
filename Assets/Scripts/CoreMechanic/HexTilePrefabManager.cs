using System.Collections.Generic;
using UnityEngine;


public class HexTilePrefabManager : MonoBehaviour {

    [SerializeField] private LineTile _sourcePrefab;
    [SerializeField] private LineTile _lampPrefab;
    [SerializeField] private LineTile _emptyPrefab;
    [SerializeField] private List<LineTile> _tilePrefabs;

    private Dictionary<int, GameObject> _canonicalMaskPrefabs;

    void Awake() {
        GenerateDictionary();
    }

    private void GenerateDictionary() {
        _canonicalMaskPrefabs = new Dictionary<int, GameObject>();

        foreach ( var tilePrefab in _tilePrefabs ) {
            int mask = EncodeConnections(tilePrefab.LineElement.neighborConnections.ToArray());
            int canonicalMask = GetCanonicalMask(mask);

            if ( !_canonicalMaskPrefabs.ContainsKey(canonicalMask) ) {
                _canonicalMaskPrefabs.Add(canonicalMask, tilePrefab.gameObject);
            } else {
                Debug.LogWarning($"Duplicate canonical pattern detected for prefab {tilePrefab.gameObject.name}");
            }
        }
    }
       

    private int EncodeConnections(bool[] connections) {
        int mask = 0;
        for ( int i = 0; i < connections.Length; i++ ) {
            if ( connections[i] )
                mask |= (1 << i);
        }
        return mask;
    }

    private int RotateMask(int mask, int rot) {
        rot = rot % 6;
        int lowerBits = mask & ((1 << rot) - 1);
        int rotated = (mask >> rot) | (lowerBits << (6 - rot));
        rotated &= 0b111111; // mask to 6 bits
        return rotated;
    }

    private int GetCanonicalMask(int mask) {
        int minMask = mask;
        for ( int rot = 1; rot < 6; rot++ ) {
            int rotated = RotateMask(mask, rot);
            if ( rotated < minMask )
                minMask = rotated;
        }
        return minMask;
    }

    public GameObject GetPrefabForConnections(bool[] connections) {
        int mask = EncodeConnections(connections);
        int canonicalMask = GetCanonicalMask(mask);

        if ( _canonicalMaskPrefabs.TryGetValue(canonicalMask, out GameObject prefab) ) {
            return prefab;
        } else {
            Debug.LogWarning("No prefab found for pattern");
            return null;
        }
    }

    public GameObject GetSourcePrefab() {
        return _sourcePrefab.gameObject;
    }

    public GameObject GetLampPrefab() {
        return _lampPrefab.gameObject;
    }

    public GameObject GetEmptyPrefab() {
        return _emptyPrefab.gameObject;
    }
}

