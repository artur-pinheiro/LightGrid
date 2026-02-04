using System.Collections.Generic;
using UnityEngine;

public enum LineType {
    Empty,
    Source,
    Wifi,
    Lamp,
    Line
}

[CreateAssetMenu(fileName = "NewLineElement", menuName = "LineElement")]
public class LineElementSO : ScriptableObject {    

    [Tooltip("The type of this line element")]
    public LineType lineType = LineType.Line;
    [Tooltip("List of connections on a tile following a Clockwise convention starting from the top")]
    public List<bool> neighborConnections = new List<bool>();
    [Tooltip("Sprite used when the tile is not energized")]
    public Sprite offSprite;
    [Tooltip("Sprite for when the tile is energized")]
    public Sprite onSprite;
    [Tooltip("Sprite for energized icon")]
    public Sprite iconOnSprite;
    [Tooltip("Sprite for not energized icon")]
    public Sprite iconOffSprite;
}
