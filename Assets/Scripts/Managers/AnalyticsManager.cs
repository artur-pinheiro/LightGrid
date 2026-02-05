using UnityEngine;

public class AnalyticsManager : MonoBehaviour {

    private Amplitude _amplitude;
    void Start() {
        _amplitude = Amplitude.getInstance();
        _amplitude.setServerUrl("https://api2.amplitude.com");
        _amplitude.logging = true;
        _amplitude.trackSessionEvents(true);
        _amplitude.init("de7addacc419e351e0c32f4a04a28290");

        EventManager.OnClickTile += LogClick;
        EventManager.OnFinishedLevel += LogLevelComplete;
    }

    private void OnDestroy() {
        EventManager.OnClickTile -= LogClick;
        EventManager.OnFinishedLevel -= LogLevelComplete;
    }

    private void LogClick() {
        _amplitude.logEvent("Tile Clicked");
    }

    private void LogLevelComplete() {
        _amplitude.logEvent("Level Complete");
    }
}
