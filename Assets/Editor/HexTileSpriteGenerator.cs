using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class HexTileSpriteGenerator : EditorWindow
{
    // UI inputs
    private int spriteWidth = 256;
    private int spriteHeight = 256;
    private int lineWidth = 8;
    private Color lineColor = Color.white;

    // Unique patterns (generated once)
    private List<bool[]> uniquePatterns;

    // Scroll position for pattern list
    private Vector2 scrollPos;

    [MenuItem("Tools/Hex Tile Sprite Generator")]
    public static void ShowWindow() {
        GetWindow<HexTileSpriteGenerator>("Hex Tile Sprite Generator");
    }

    private void OnEnable() {
        GenerateUniquePatterns();
    }

    private void OnGUI() {
        GUILayout.Label("Sprite Settings", EditorStyles.boldLabel);

        spriteWidth = EditorGUILayout.IntField("Sprite Width (px)", spriteWidth);
        spriteHeight = EditorGUILayout.IntField("Sprite Height (px)", spriteHeight);
        lineWidth = EditorGUILayout.IntSlider("Line Width (px)", lineWidth, 1, 32);
        lineColor = EditorGUILayout.ColorField("Line Color", lineColor);

        GUILayout.Space(10);

        if ( GUILayout.Button("Regenerate Unique Patterns") ) {
            GenerateUniquePatterns();
        }

        GUILayout.Space(10);
        GUILayout.Label($"Unique Patterns (count: {uniquePatterns.Count})", EditorStyles.boldLabel);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        // Show each pattern with a preview and save button
        for ( int i = 0; i < uniquePatterns.Count; i++ ) {
            bool[] pattern = uniquePatterns[i];
            EditorGUILayout.BeginHorizontal();

            // Pattern string
            string patternStr = string.Join("", pattern.Select(b => b ? "1" : "0"));
            EditorGUILayout.LabelField($"Pattern {i}: {patternStr}", GUILayout.Width(150));

            if ( GUILayout.Button("Generate & Save", GUILayout.Width(120)) ) {
                GenerateAndSaveSprite(pattern);
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }

    private void GenerateUniquePatterns() {
        int total = 1 << 6; // 64 combinations
        HashSet<string> uniqueSet = new HashSet<string>();
        uniquePatterns = new List<bool[]>();

        for ( int i = 0; i < total; i++ ) {
            bool[] pattern = IntToBoolArray(i, 6);
            string canonical = GetCanonicalForm(pattern);
            if ( !uniqueSet.Contains(canonical) ) {
                uniqueSet.Add(canonical);
                uniquePatterns.Add(pattern);
            }
        }
    }

    private bool[] IntToBoolArray(int val, int n) {
        bool[] arr = new bool[n];
        for ( int i = 0; i < n; i++ ) {
            arr[i] = (val & (1 << i)) != 0;
        }
        return arr;
    }

    private string GetCanonicalForm(bool[] pattern) {
        string[] rotations = new string[6];
        for ( int rot = 0; rot < 6; rot++ ) {
            rotations[rot] = RotatePattern(pattern, rot);
        }
        System.Array.Sort(rotations);
        return rotations[0];
    }

    private string RotatePattern(bool[] pattern, int rot) {
        int n = pattern.Length;
        char[] rotated = new char[n];
        for ( int i = 0; i < n; i++ ) {
            rotated[i] = pattern[(i + rot) % n] ? '1' : '0';
        }
        return new string(rotated);
    }

    private void GenerateAndSaveSprite(bool[] connections) {
        Texture2D tex = new Texture2D(spriteWidth, spriteHeight, TextureFormat.ARGB32, false);
        Color32[] fillColorArray = tex.GetPixels32();

        for ( int i = 0; i < fillColorArray.Length; i++ )
            fillColorArray[i] = new Color32(0, 0, 0, 0);

        tex.SetPixels32(fillColorArray);

        Vector2 center = new Vector2(spriteWidth / 2f, spriteHeight / 2f);
        float radius = Mathf.Min(spriteWidth, spriteHeight) / 2f - lineWidth;

        // Calculate hex edge points (flat topped), corrected for your convention: top = index 0 at 90 degrees
        Vector2[] edgePoints = new Vector2[6];
        for ( int i = 0; i < 6; i++ ) {
            float angle_deg = 90 - 60 * i;
            float angle_rad = Mathf.Deg2Rad * angle_deg;
            edgePoints[i] = new Vector2(
                center.x + radius * Mathf.Cos(angle_rad),
                center.y + radius * Mathf.Sin(angle_rad)
            );
        }

        // Draw lines from center to connected edges
        for ( int i = 0; i < 6; i++ ) {
            if ( connections[i] ) {
                DrawThickLine(tex, center, edgePoints[i], lineColor, lineWidth);
            }
        }

        // Draw curves connecting consecutive connected edges around perimeter
        int[] connectedIndices = GetConnectedIndicesInOrder(connections);

        for ( int i = 0; i < connectedIndices.Length; i++ ) {
            int current = connectedIndices[i];
            int next = connectedIndices[(i + 1) % connectedIndices.Length];

            if ( connectedIndices.Length > 1 ) {
                DrawBezierCurve(tex, edgePoints[current], center, edgePoints[next], lineColor, lineWidth);
            }
        }

        tex.Apply();

        // Save PNG in project folder Assets/HexTileSprites/
        string folderPath = "Assets/HexTileSprites";
        if ( !Directory.Exists(folderPath) ) {
            Directory.CreateDirectory(folderPath);
        }

        // Filename based on pattern string
        string patternStr = string.Join("", connections.Select(b => b ? "1" : "0"));
        string path = Path.Combine(folderPath, $"HexTile_{patternStr}.png");

        byte[] pngData = tex.EncodeToPNG();
        if ( pngData != null ) {
            File.WriteAllBytes(path, pngData);
            Debug.Log($"Saved sprite: {path}");
            AssetDatabase.Refresh();
        } else {
            Debug.LogError("Failed to encode PNG");
        }
    }

    private int[] GetConnectedIndicesInOrder(bool[] connections) {
        List<int> list = new List<int>();
        for ( int i = 0; i < connections.Length; i++ )
            if ( connections[i] ) list.Add(i);
        return list.ToArray();
    }

    private void DrawThickLine(Texture2D tex, Vector2 p1, Vector2 p2, Color col, int thickness) {
        int steps = (int)Vector2.Distance(p1, p2);
        for ( int i = 0; i <= steps; i++ ) {
            float t = i / (float)steps;
            Vector2 point = Vector2.Lerp(p1, p2, t);
            DrawFilledCircle(tex, (int)point.x, (int)point.y, thickness / 2, col);
        }
    }

    private void DrawFilledCircle(Texture2D tex, int cx, int cy, int radius, Color col) {
        int sqrRadius = radius * radius;
        for ( int x = -radius; x <= radius; x++ ) {
            int height = (int)Mathf.Sqrt(sqrRadius - x * x);
            for ( int y = -height; y <= height; y++ ) {
                int px = cx + x;
                int py = cy + y;
                if ( px >= 0 && px < tex.width && py >= 0 && py < tex.height ) {
                    Color existing = tex.GetPixel(px, py);
                    Color blended = Color.Lerp(existing, col, col.a);
                    tex.SetPixel(px, py, blended);
                }
            }
        }
    }

    private void DrawBezierCurve(Texture2D tex, Vector2 p0, Vector2 p1, Vector2 p2, Color col, int thickness) {
        int segments = 40;
        Vector2 prevPoint = p0;
        for ( int i = 1; i <= segments; i++ ) {
            float t = i / (float)segments;
            Vector2 point = CalculateQuadraticBezierPoint(t, p0, p1, p2);
            DrawThickLine(tex, prevPoint, point, col, thickness);
            prevPoint = point;
        }
    }

    private Vector2 CalculateQuadraticBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2) {
        float u = 1 - t;
        return u * u * p0 + 2 * u * t * p1 + t * t * p2;
    }
}

