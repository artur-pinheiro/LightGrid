using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLineElement", menuName = "LineElement")]
public class LineElementSO : ScriptableObject {

    public enum LineType {
        Empty,
        Source,
        Wifi,
        Line
    }

    [Tooltip("List of connections on a tile following the a Clockwise convention starting from the top")]
    public List<bool> neighboorConnections = new List<bool>();
    [Tooltip("Sprite used when the tile is not energized")]
    public Sprite offSprite;
    [Tooltip("Sprite for when the tile is energized")]
    public Sprite onSprite;
}
