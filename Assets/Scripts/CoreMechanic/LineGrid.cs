using System.Collections.Generic;
using UnityEngine;

public class LineGrid : MonoBehaviour {

    [SerializeField] private Vector2Int _gridDimensions;

    private Dictionary<Vector2Int, LineTile> _tileGrid = new Dictionary<Vector2Int, LineTile>();
    private Vector2Int _energySourceIndex;
    private List<Vector2Int> _wifiTilesIndexes = new List<Vector2Int>();

    void Start() {
        BuildGrid();
    }

    private void BuildGrid() {
        _tileGrid.Clear();

        int childIndex = 0;
        int cols = Mathf.RoundToInt(_gridDimensions.x);
        int rows = Mathf.RoundToInt(_gridDimensions.y);

        if ( transform.childCount < cols * rows ) {
            Debug.LogWarning("Not enough children to fill the grid!");
            return;
        }

        for ( int y = 0; y < rows; y++ ) {
            for ( int x = 0; x < cols; x++ ) {
                Transform child = transform.GetChild(childIndex);
                LineTile tile = child.GetComponent<LineTile>();

                if ( tile == null ) {
                    Debug.LogError($"Child {child.name} does not have a LineTile component!");
                }

                // Store tiles in dictionary keyed by coordinates
                var newIndex = new Vector2Int(x, y);
                _tileGrid[newIndex] = tile;

                switch ( tile.LineElement.lineType ) {
                    case LineType.Source:
                        _energySourceIndex = newIndex;
                        break;
                    case LineType.Wifi:
                        _wifiTilesIndexes.Add(newIndex);
                        break;
                    default:
                        break;                                   
                }

                childIndex++;
            }
        }
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
                new Vector2Int(0, -1),  // 0 = Top                              top
                new Vector2Int(1, -1),  // 1 = Top-Right                       ____ 
                new Vector2Int(1, 0),   // 2 = Bottom-Right        5 Top-Left /    \ Top-Right 1
                new Vector2Int(0, 1),   // 3 = Bottom           4 Bottom-Left \____/ Bottom-Right 2
                new Vector2Int(-1, 1),  // 4 = Bottom-Left                    Bottom
                new Vector2Int(-1, 0)   // 5 = Top-Left                          3
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

        while ( indexQueue.Count > 0 ) {
            var currentIndex = indexQueue.Dequeue();
            var currentTile = _tileGrid[currentIndex];
            result.Add(currentTile);

            // Auto-detect directions/opposites
            GetDirections(currentTile, out var directions, out var oppositeConnections);

            for ( int i = 0; i < directions.Length; i++ ) {
                if ( !currentTile.LineElement.neighborConnections[i] ) continue; // no connection on this side

                var neighborPos = currentIndex + directions[i];

                if ( !_tileGrid.TryGetValue(neighborPos, out var neighborTile) ) continue;

                // Check wether the neighbor tile is turned towards the current tile
                if ( !neighborTile.LineElement.neighborConnections[oppositeConnections[i]] ) continue;

                if ( !visitedTiles.Contains(neighborPos) ) {
                    visitedTiles.Add(neighborPos);
                    indexQueue.Enqueue(neighborPos);
                }
            }
        }

        return result;
    }


}
