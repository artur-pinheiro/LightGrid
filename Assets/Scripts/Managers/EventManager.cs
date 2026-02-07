using UnityEngine;
using UnityEngine.Events;

public static class EventManager {

    /// <summary>
    /// Called when a tile finishes its rotation
    /// </summary>
    public static UnityAction OnFinishRotatingTile { get; set; }

    /// <summary>
    /// Called when a lamp tile gets energized or turned off
    /// </summary>
    public static UnityAction<bool> OnEnergizeLamp { get; set; }

    /// <summary>
    /// Called when all lamps in the grid gave been counted
    /// </summary>
    public static UnityAction<int> OnCountLamps { get; set; }

    /// <summary>
    /// Called when the level has been completed
    /// </summary>
    public static UnityAction<int> OnFinishedLevel { get; set; }

    /// <summary>
    /// Called when the end level UI is shown
    /// </summary>
    public static UnityAction OnShowEndLevelUI { get; set; }

    /// <summary>
    /// Called when the new score is set
    /// </summary>
    public static UnityAction<ScoreData, int> OnSetScore { get; set; }

    /// <summary>
    /// Called when the score is loaded from disk
    /// </summary>
    public static UnityAction<ScoreData> OnScoreLoaded { get; set; }

    /// <summary>
    /// Called when a tile is clicked on the grid
    /// </summary>
    public static UnityAction OnClickTile { get; set; }

    /// <summary>
    /// Called when a new level is selected on Levels Menu
    /// </summary>
    public static UnityAction<int> OnNewLevelSelected { get; set; }

    /// <summary>
    /// Called when a new level is loaded
    /// </summary>
    public static UnityAction<int> OnLoadedNewLevel { get; set; }
}
