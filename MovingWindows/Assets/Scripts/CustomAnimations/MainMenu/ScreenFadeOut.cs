using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class ScreenFadeOut : MonoBehaviour
{

    public Volume globalVolume;
    public TextMeshProUGUI[] texts;
    public float duration = 5f; // Time to reach -8 exposure
    private float targetExposure = -8f;
    private float startExposure = 0f;
    //private float timer = 0f;

    private ColorAdjustments colorAdjustments;

    void Start()
    {
        if (globalVolume == null)
        {
            Debug.LogError("Global Volume not assigned.");
            return;
        }

        // Try to get Color Adjustments from the volume profile
        if (!globalVolume.profile.TryGet(out colorAdjustments))
        {
            Debug.LogError("Color Adjustments not found in Volume Profile.");
            return;
        }

        // Ensure exposure is initially set
        colorAdjustments.postExposure.value = startExposure;
    }

    public IEnumerator ChangeExposure()
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            colorAdjustments.postExposure.value = Mathf.Lerp(startExposure, targetExposure, t);

            foreach (var text in texts)
            {
                text.alpha = 1 - t;
            }
            yield return null;
        }

        colorAdjustments.postExposure.value = targetExposure;
        SceneManager.LoadScene(1);
    }
}

