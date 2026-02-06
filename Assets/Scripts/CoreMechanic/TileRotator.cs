using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TileRotator : MonoBehaviour {

    //We set them separatedly so we can have other sprites that will not be affected
    [SerializeField] private Transform _transform;

    public int rotationAngle;
    public float rotationDuration;

    public UnityAction OnFinishedRotating;

    /// <summary>
    /// Immediate, no animation
    /// </summary>
    public void SetTileRotation(int rotations) {
        _transform.Rotate(0, 0, rotations * rotationAngle);
        OnFinishedRotating?.Invoke();
    }

    /// <summary>
    /// Rotate the tile over time
    /// </summary>
    public void AnimateRotation() {
        StartCoroutine(RotateOverTime());
    }

    private IEnumerator RotateOverTime() {
        Quaternion startRotation = _transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, startRotation.eulerAngles.z + rotationAngle));

        float elapsed = 0f;

        while ( elapsed < rotationDuration ) {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / rotationDuration);

            _transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);

            yield return null;
        }

        _transform.rotation = targetRotation;
        OnFinishedRotating?.Invoke();
        EventManager.OnFinishRotatingTile?.Invoke();
    }
}
