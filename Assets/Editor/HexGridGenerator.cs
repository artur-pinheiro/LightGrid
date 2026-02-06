using UnityEngine;
using UnityEditor;

public class HexGridGeneratorEditor : EditorWindow
{
    public GameObject hexTilePrefab;
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float hexRadius = 1f;

    [MenuItem("Tools/Hex Grid Generator")]
    public static void ShowWindow() {
        GetWindow<HexGridGeneratorEditor>("Hex Grid Generator");
    }

    private void OnGUI() {
        GUILayout.Label("Hex Grid Settings", EditorStyles.boldLabel);

        hexTilePrefab = (GameObject)EditorGUILayout.ObjectField("Hex Tile Prefab", hexTilePrefab, typeof(GameObject), false);
        gridWidth = EditorGUILayout.IntField("Grid Width", gridWidth);
        gridHeight = EditorGUILayout.IntField("Grid Height", gridHeight);
        hexRadius = EditorGUILayout.FloatField("Hex Radius", hexRadius);

        if ( GUILayout.Button("Generate Hex Grid") ) {
            if ( hexTilePrefab == null ) {
                Debug.LogError("Please assign a Hex Tile Prefab.");
                return;
            }
            GenerateHexGrid();
        }
    }

    void GenerateHexGrid() {
        // Create a parent object to hold the grid tiles
        GameObject parent = new GameObject("HexGrid");

        float hexWidth = 2f * hexRadius;
        float hexHeight = Mathf.Sqrt(3f) * hexRadius;
        float horizontalSpacing = hexWidth * 0.75f;

        for ( int x = 0; x < gridWidth; x++ ) {
            for ( int y = 0; y < gridHeight; y++ ) {
                // Calculate position for flat-topped hex
                float posX = x * horizontalSpacing;
                float posY = y * hexHeight + (x % 2) * (hexHeight / 2f);

                Vector3 pos = new Vector3(posX, posY, 0);

                // Instantiate prefab in the editor scene
                GameObject hexTile = (GameObject)PrefabUtility.InstantiatePrefab(hexTilePrefab);
                hexTile.transform.position = pos;
                hexTile.transform.parent = parent.transform;

                // Register undo for editor undo support
                Undo.RegisterCreatedObjectUndo(hexTile, "Create Hex Tile");
            }
        }

        // Register undo for parent object creation
        Undo.RegisterCreatedObjectUndo(parent, "Create Hex Grid");
    }
}
