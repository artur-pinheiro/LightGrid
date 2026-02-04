using UnityEngine;
using UnityEngine.Events;

public static class EventManager {

    /// <summary>
    /// Called when a tile finishes its rotation
    /// </summary>
    public static UnityAction OnFinishRotatingTile { get; set; }
}
