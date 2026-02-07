using System.Collections.Generic;
using UnityEngine;

public class LineGrid : MonoBehaviour {

    [SerializeField] private Vector2Int _gridDimensions;
    [SerializeField] private float _hexagonalTileRadius = 1f;


    private Dictionary<Vector2Int, LineTile> _tileGrid = new Dictionary<Vector2Int, LineTile>();
    private Vector2Int _energySourceIndex;
    private List<Vector2Int> _wifiTilesIndexes = new List<Vector2Int>();

    private void Awake() {
        EventManager.OnFinishRotatingTile += ActivateLines;
        EventManager.OnNewLevelSelected += HideGrid;
    }

    private void OnDestroy() {
        EventManager.OnFinishRotatingTile -= ActivateLines;
        EventManager.OnNewLevelSelected -= HideGrid;
    }

    void Start() {
        BuildGrid();
    }

    private void BuildGrid() {
        _tileGrid.Clear();

        int lampsCount = 0;
        int childIndex = 0;
        int cols = Mathf.RoundToInt(_gridDimensions.x);
        int rows = Mathf.RoundToInt(_gridDimensions.y);

        if ( transform.childCount < cols * rows ) {
            Debug.LogError("Not enough children to fill the grid!");
            return;
        }

        // Collect all children
        var allTiles = new List<Transform>();
        for ( int i = 0; i < transform.childCount; i++ )
            allTiles.Add(transform.GetChild(i));

        // Sort children by local position (top-to-bottom, then left-to-right)
        allTiles.Sort((a, b) => {
            int yCompare = -a.localPosition.y.CompareTo(b.localPosition.y);
            // negative so higher y comes first (top row)
            if ( yCompare != 0 ) return yCompare;
            return a.localPosition.x.CompareTo(b.localPosition.x);
        });

        //Checks if the grid is 4 or 6
        if ( allTiles[0].TryGetComponent(out LineTile lineTile) ) {
            if ( lineTile.LineElement.neighborConnections.Count != 4 &&
                lineTile.LineElement.neighborConnections.Count != 6 ) {
                Debug.LogError($"Unsupported grid type: {lineTile.LineElement.neighborConnections.Count} edges");
                return;
            }
        }

        bool isSquare = lineTile.LineElement.neighborConnections.Count == 4;

        for ( int y = 0; y < rows; y++ ) {
            for ( int x = 0; x < cols; x++ ) {
                Transform tileTransform = allTiles[childIndex];
                LineTile tile = tileTransform.GetComponent<LineTile>();

                if ( tile == null ) {
                    Debug.LogError($"Child {tileTransform.name} does not have a LineTile component!");
                }

                // Store tiles in dictionary keyed by coordinates
                Vector2Int newIndex;

                if ( isSquare ) {
                    // Cartesian coordinates
                    newIndex = new Vector2Int(x, y);
                } else {
                    // Flat-top axial coordinates from local position
                    float width = _hexagonalTileRadius * 2f;                  // hex width
                    float height = Mathf.Sqrt(3f) * _hexagonalTileRadius;      // hex height

                    // Convert Unity local position to axial (q,r)
                    float tileX = tileTransform.localPosition.x;
                    float tileY = tileTransform.localPosition.y;

                    int q = Mathf.RoundToInt(tileX / (width * 0.75f));
                    int r = Mathf.RoundToInt((tileY - (q * height / 2f)) / height);

                    newIndex = new Vector2Int(q, r);
                }

                _tileGrid[newIndex] = tile;

                switch ( tile.LineElement.lineType ) {
                    case LineType.Source:
                        _energySourceIndex = newIndex;
                        break;
                    case LineType.Wifi:
                        _wifiTilesIndexes.Add(newIndex);
                        break;
                    case LineType.Lamp:
                        lampsCount++;
                        break;
                    default:
                        break;                                   
                }

                childIndex++;
            }
        }
        EventManager.OnCountLamps?.Invoke(lampsCount);
    }

    private void ActivateLines() {
        BFS(_energySourceIndex);
    }

    private void GetDirections(LineTile tile, out Vector2Int[] directions, out int[] opposite) {
        if ( tile.LineElement.neighborConnections.Count == 4 ) {
            // Square grid (row/col coordinates)                    0
            directions = new Vector2Int[] {//                      top
                new Vector2Int(0, -1), // 0 = Top                  ___  
                new Vector2Int(1, 0),  // 1 = Right        3 left |___|  right 1   
                new Vector2Int(0, 1),  // 2 = Bottom              bottom
                new Vector2Int(-1, 0)  // 3 = Left                  2
            };

            opposite = new int[] { 2, 3, 0, 1 };
        } else if ( tile.LineElement.neighborConnections.Count == 6 ) {
            // Hex grid (axial coordinates, Top = 0, clockwise)
            directions = new Vector2Int[] {//                                    0                                        
                new Vector2Int(0, 1),  // 0 = Top                               top
                new Vector2Int(1, 0),  // 1 = Top-Right                         ___ 
                new Vector2Int(1, -1),   // 2 = Bottom-Right        5 Top-Left /   \ Top-Right 1
                new Vector2Int(0, -1),   // 3 = Bottom           4 Bottom-Left \___/ Bottom-Right 2
                new Vector2Int(-1, 0),  // 4 = Bottom-Left                    Bottom
                new Vector2Int(-1, +1)   // 5 = Top-Left                          3
            };

            opposite = new int[] { 3, 4, 5, 0, 1, 2 };
        } else {
            throw new System.Exception($"Unsupported tile type with {tile.LineElement.neighborConnections.Count} edges");
        }
    }

    public List<LineTile> BFS(Vector2Int startIndex) {

        var visitedTiles = new HashSet<Vector2Int>();
        var result = new List<LineTile>();
        var indexQueue = new Queue<Vector2Int>();

        indexQueue.Enqueue(startIndex);
        visitedTiles.Add(startIndex);

        foreach ( KeyValuePair<Vector2Int, LineTile> tile in _tileGrid ) {
            tile.Value.Energize(false);
        }

        while ( indexQueue.Count > 0 ) {
            var currentIndex = indexQueue.Dequeue();
            var currentTile = _tileGrid[currentIndex];
            result.Add(currentTile);
            currentTile.Energize(true);

            // Auto-detect directions/opposites
            GetDirections(currentTile, out var directions, out var oppositeConnections);

            bool[] neighborConnections = currentTile.GetCurrentNeighbors();

            for ( int i = 0; i < directions.Length; i++ ) {
                if ( !neighborConnections[i] ) continue; // no connection on this side

                var neighborPos = currentIndex + directions[i];

                if ( !_tileGrid.TryGetValue(neighborPos, out var neighborTile) ) continue;

                // Check wether the neighbor tile is turned towards the current tile
                if ( !neighborTile.GetCurrentNeighbors()[oppositeConnections[i]] ) continue;

                if ( !visitedTiles.Contains(neighborPos) ) {
                    visitedTiles.Add(neighborPos);
                    indexQueue.Enqueue(neighborPos);
                }
            }

            //Jump over to the wifi tiles
            if ( currentTile.LineElement.lineType == LineType.Wifi ) {
                for ( int i = 0; i < _wifiTilesIndexes.Count; i++ ) {
                    if ( _wifiTilesIndexes[i] == currentIndex) continue; 

                    if ( !visitedTiles.Contains(_wifiTilesIndexes[i]) ) {
                        visitedTiles.Add(_wifiTilesIndexes[i]);
                        indexQueue.Enqueue(_wifiTilesIndexes[i]);
                    }
                }
            }
        }

        return result;
    }

    private void HideGrid(int nextSceneIndex) {
        gameObject.SetActive(false);
    }

}
