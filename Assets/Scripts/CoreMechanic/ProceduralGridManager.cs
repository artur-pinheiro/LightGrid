
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ProceduralLineGrid))]
public class ProceduralGridManager : MonoBehaviour  {

    private ProceduralLineGrid _proceduralLineGrid;
    private Dictionary<Vector2Int, LineTile> _tileGrid = new Dictionary<Vector2Int, LineTile>();
    private Vector2Int _energySourceIndex;

    private void Awake() {
        EventManager.OnFinishRotatingTile += ActivateLines;
        EventManager.OnNewLevelSelected += HideGrid;
    }

    private void OnDestroy() {
        EventManager.OnFinishRotatingTile -= ActivateLines;
        EventManager.OnNewLevelSelected -= HideGrid;
    }

    void Start() {
        _proceduralLineGrid = GetComponent<ProceduralLineGrid>();
        _tileGrid = _proceduralLineGrid.GenerateHexGrid();
        _energySourceIndex = _proceduralLineGrid.EnergySourceIndex;
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
        }

        return result;
    }

    private void HideGrid(int nextSceneIndex) {
        gameObject.SetActive(false);
    }

}
