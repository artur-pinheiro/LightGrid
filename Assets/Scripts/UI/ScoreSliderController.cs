using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class ScoreSliderController : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI valueText;

    public void AnimateSlider(float targetValue, float duration) {
        StartCoroutine(AnimateSliderCoroutine(targetValue, duration));
    }

    private IEnumerator AnimateSliderCoroutine(float targetValue, float duration) {

        Slider slider = GetComponent<Slider>();

        float startValue = 0f;
        float elapsed = 0f;

        while ( elapsed < duration ) {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            slider.value = Mathf.Lerp(startValue, targetValue, t);
            valueText.text = slider.value.ToString("00");
            yield return null;
        }

        slider.value = targetValue; 
        valueText.text = targetValue.ToString("00");
    }

}
