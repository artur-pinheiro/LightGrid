using System.Collections.Generic;
using UnityEngine;

public class GridElement {
    public bool[] connections = new bool[6];

    public GridElement() {
        for ( int i = 0; i < 6; i++ )
            connections[i] = false;
    }

    // Connect this tile to neighbor in direction dir (0-5)
    public void Connect(int dir) {
        connections[dir] = true;
    }
}
[RequireComponent(typeof(HexTilePrefabManager))]
public class ProceduralLineGrid : MonoBehaviour {    

    public int gridWidth = 10;
    public int gridHeight = 10;
    public float hexRadius = 0.259f;
    public int numberOfLamps = 2;

    private Dictionary<Vector2Int, LineTile> _tileGrid = new Dictionary<Vector2Int, LineTile>();
    private Dictionary<Vector2Int, Vector2> _indexPositionGrid = new Dictionary<Vector2Int, Vector2>();
    private Dictionary<Vector2Int, GridElement> _connectionsGrid = new Dictionary<Vector2Int, GridElement>();

    private HexTilePrefabManager _prefabManager;
    private Vector2Int _energySourceIndex;
    HashSet<Vector2Int> _lampIndexes = new HashSet<Vector2Int>();

    static readonly Vector2Int[] directions = new Vector2Int[]{ //               0                                        
                new Vector2Int(0, 1),  // 0 = Top                               top
                new Vector2Int(1, 0),  // 1 = Top-Right                         ___ 
                new Vector2Int(1, -1),   // 2 = Bottom-Right        5 Top-Left /   \ Top-Right 1
                new Vector2Int(0, -1),   // 3 = Bottom           4 Bottom-Left \___/ Bottom-Right 2
                new Vector2Int(-1, 0),  // 4 = Bottom-Left                    Bottom
                new Vector2Int(-1, +1)   // 5 = Top-Left                         3
    };
    static readonly int[] opposite = new int[] { 3, 4, 5, 0, 1, 2 };

    public Vector2Int EnergySourceIndex { get => _energySourceIndex; }

    public Dictionary<Vector2Int, LineTile> GenerateHexGrid() {

        _prefabManager = GetComponent<HexTilePrefabManager>();

        float hexWidth = 2f * hexRadius;
        float hexHeight = Mathf.Sqrt(3f) * hexRadius;
        float horizontalSpacing = hexWidth * 0.75f;

        for ( int x = 0; x < gridWidth; x++ ) {
            for ( int y = 0; y < gridHeight; y++ ) {
                // Calculate position for flat-topped hex
                float posX = x * horizontalSpacing;
                float posY = y * hexHeight + (x % 2) * (hexHeight / 2f);

                Vector3 position = new Vector3(posX, posY, 0);

                // Flat-top axial coordinates from local position
                float width = hexRadius * 2f;                  // hex width
                float height = Mathf.Sqrt(3f) * hexRadius;      // hex height

                // Convert Unity local position to axial (q,r)
                int q = Mathf.RoundToInt(posX / (width * 0.75f));
                int r = Mathf.RoundToInt((posY - (q * height / 2f)) / height);

                var newIndex = new Vector2Int(q, r);

                _indexPositionGrid[newIndex] = position;
            }
        }

        // Place root element randomly. Since it can only have one connection we will default it to the top neighbor, and actually start from there
        List<Vector2Int> gridHexCoordinates = new List<Vector2Int>(_indexPositionGrid.Keys);
        Vector2Int rootIndex = gridHexCoordinates[Random.Range(0, gridHexCoordinates.Count)];
        while ( !gridHexCoordinates.Contains(rootIndex + directions[0]) ) {
             rootIndex = gridHexCoordinates[Random.Range(0, gridHexCoordinates.Count)];
        }
        _energySourceIndex = rootIndex;
        rootIndex = rootIndex + directions[0];

        // Place Lamp elements randomly (avoid duplicates & root)        
        while ( _lampIndexes.Count < numberOfLamps ) {
            Vector2Int candidate = gridHexCoordinates[Random.Range(0, gridHexCoordinates.Count)];
            if ( candidate != rootIndex && candidate != _energySourceIndex)
                _lampIndexes.Add(candidate);
        }

        // For each Lamp element, find path from root using BFS and set connections
        foreach ( var lampIndex in _lampIndexes ) {
            var path = GetShortestPath(rootIndex, lampIndex);
            if ( path == null ) continue; // No path found (should not happen in hex grid)

            // Build path connections
            for ( int i = 0; i < path.Count - 1; i++ ) {
                Vector2Int currentIndex = path[i];
                Vector2Int nextIndex = path[i + 1];

                // Determine direction from current to next
                int direction = GetDirectionBetweenTiles(currentIndex, nextIndex);
                int oppositeDirection = opposite[direction];

                if ( !_connectionsGrid.ContainsKey(currentIndex) )
                    _connectionsGrid[currentIndex] = new GridElement();
                if ( !_connectionsGrid.ContainsKey(nextIndex) )
                    _connectionsGrid[nextIndex] = new GridElement();

                _connectionsGrid[currentIndex].Connect(direction);
                _connectionsGrid[nextIndex].Connect(oppositeDirection);
            }
            //connect first tile of the path with the root source tile
            _connectionsGrid[_energySourceIndex + directions[0]].Connect(opposite[0]);
        }

        //Instantiate all the prefabs on the grid
        foreach ( KeyValuePair<Vector2Int,Vector2> indexPosition in _indexPositionGrid ) {
            GameObject prefabToInstantiate;
            if ( indexPosition.Key == _energySourceIndex ) {
                prefabToInstantiate = _prefabManager.GetSourcePrefab();
            } else if (_lampIndexes.Contains(indexPosition.Key) ) {
                prefabToInstantiate = _prefabManager.GetLampPrefab();
            } else if ( _connectionsGrid.ContainsKey(indexPosition.Key) ) {
                prefabToInstantiate = _prefabManager.GetPrefabForConnections(_connectionsGrid[indexPosition.Key].connections);
            } else
                prefabToInstantiate = _prefabManager.GetEmptyPrefab();

            GameObject hexTile = Instantiate(prefabToInstantiate);
            hexTile.transform.position = indexPosition.Value;
            hexTile.transform.rotation = Quaternion.identity;
            hexTile.transform.parent = transform;
            _tileGrid[indexPosition.Key] = hexTile.GetComponent<LineTile>();
        }
        transform.position = new Vector3(-1.74f, -2.17f, 0);
        EventManager.OnCountLamps?.Invoke(numberOfLamps);
        return _tileGrid;
    }

    // BFS to find shortest path between two axial coordinates within grid
    private List<Vector2Int> GetShortestPath(Vector2Int startIndex, Vector2Int goalIndex) {
        Queue<Vector2Int> indexQueue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        HashSet<Vector2Int> visitedTiles = new HashSet<Vector2Int>();

        indexQueue.Enqueue(startIndex);
        visitedTiles.Add(_energySourceIndex); //prevent passing through the source
        foreach ( var lampIndex in _lampIndexes ) { //prevent passing through a lamp
            if ( lampIndex != goalIndex )
                visitedTiles.Add(lampIndex);
        }
        visitedTiles.Add(startIndex);

        while ( indexQueue.Count > 0 ) {
            var currentIndex = indexQueue.Dequeue();
            if ( currentIndex == goalIndex )
                return ReconstructPath(cameFrom, startIndex, goalIndex);

            for ( int i = 0; i < directions.Length; i++ ) {
                Vector2Int neighborIndex = currentIndex + directions[i];

                if ( _indexPositionGrid.ContainsKey(neighborIndex) && !visitedTiles.Contains(neighborIndex) ) {
                    visitedTiles.Add(neighborIndex);
                    cameFrom[neighborIndex] = currentIndex;
                    indexQueue.Enqueue(neighborIndex);
                }
            }
        }

        return null; // No path found
    }

    private static List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int startIndex, Vector2Int goalIndex) {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int currentIndex = goalIndex;
        while ( currentIndex != startIndex ) {
            path.Add(currentIndex);
            currentIndex = cameFrom[currentIndex];
        }
        path.Add(startIndex);
        path.Reverse();
        return path;
    }

    private static int GetDirectionBetweenTiles(Vector2Int startIndex, Vector2Int endIndex) {
        Vector2Int direction = endIndex - startIndex;
        for ( int i = 0; i < directions.Length; i++ ) {
            if ( directions[i] == direction )
                return i;
        }
        Debug.LogError("Invalid direction from " + startIndex + " to " + endIndex);
        return -1; // Should never happen if neighbors are valid
    }

}