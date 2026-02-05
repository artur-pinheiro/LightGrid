using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class GridMenuAnimator : MonoBehaviour {
    [SerializeField] private float _moveOffset = 50f;
    [SerializeField] private float _duration = 0.5f;
    [SerializeField] private float _staggerDelay = 0.05f;

    private CanvasGroup _backgroundImage;
    private RectTransform[] _buttonRects;
    private CanvasGroup[] _buttonCanvasGroups;

    public UnityAction<bool> OnFinishedAnimation;

    private void Awake() {
        _backgroundImage = GetComponent<CanvasGroup>();
        var rects = GetComponentsInChildren<RectTransform>();
        _buttonRects = rects.Where(r => r.gameObject != gameObject).ToArray();
        _buttonCanvasGroups = new CanvasGroup[_buttonRects.Length];

        for ( int i = 0; i < _buttonRects.Length; i++ ) {
            CanvasGroup canvasGroup = _buttonRects[i].GetComponent<CanvasGroup>();
            if ( canvasGroup == null ) canvasGroup = _buttonRects[i].gameObject.AddComponent<CanvasGroup>();
            _buttonCanvasGroups[i] = canvasGroup;
        }
    }

    public void OpenMenu() {
        StartCoroutine(OpenMenuSequence());
    }

    private IEnumerator OpenMenuSequence() {
        // Fade in background
        yield return StartCoroutine(FadeBackground(1f, _duration));

        // Animate buttons after background is done
        for ( int i = 0; i < _buttonRects.Length; i++ ) {
            StartCoroutine(AnimateButton(_buttonRects[i], _buttonCanvasGroups[i], i * _staggerDelay, true));
        }

        yield return new WaitForSeconds(_duration * _buttonRects.Length + _buttonRects.Length * _staggerDelay);
        OnFinishedAnimation?.Invoke(true);
    }


    public void CloseMenu() {
        StartCoroutine(CloseMenuSequence());
        
    }

    private IEnumerator CloseMenuSequence() {
        // Animate buttons out
        for ( int i = 0; i < _buttonRects.Length; i++ ) {
            StartCoroutine(AnimateButton(_buttonRects[i], _buttonCanvasGroups[i], i * _staggerDelay, false));
        }

        // Wait until last button finishes, then fade background
        yield return new WaitForSeconds(_duration + _buttonRects.Length * _staggerDelay);
        yield return StartCoroutine(FadeBackground(0f, _duration));

        OnFinishedAnimation?.Invoke(false);
    }


    private IEnumerator FadeBackground(float targetAlpha, float fadeDuration, float delay = 0f) {
        yield return new WaitForSeconds(delay);

        float startAlpha = _backgroundImage.alpha;
        float elapsedTime = 0f;

        while ( elapsedTime < fadeDuration ) {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);
            _backgroundImage.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        _backgroundImage.alpha = targetAlpha;
    }


    private IEnumerator AnimateButton(RectTransform rect, CanvasGroup canvasGroup, float delay, bool fadeIn) {
        yield return new WaitForSeconds(delay);

        Vector2 startPosition = rect.anchoredPosition;
        Vector2 endPosition = rect.anchoredPosition;
        float endAlpha = 1f;

        if ( fadeIn ) {
            startPosition += new Vector2(0, _moveOffset);
            rect.anchoredPosition = startPosition;
            canvasGroup.alpha = 0f;
        } else { //is fading out
            endPosition += new Vector2(0, _moveOffset);
            endAlpha = 0f;
        }

        float elapsedTime = 0f;
        while ( elapsedTime < _duration ) {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / _duration);

            rect.anchoredPosition = Vector2.Lerp(startPosition, endPosition, Mathf.SmoothStep(0, 1, t));
            canvasGroup.alpha = fadeIn ? t : 1f - t;

            yield return null;
        }

        rect.anchoredPosition = endPosition;
        canvasGroup.alpha = endAlpha;
    }

}
