using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AdjustTextSize : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI targetText;
    [SerializeField] float enlargedSize;
    [SerializeField] float smoothTime;
    float startSize;
    float sizeDifference;
    bool enlarged;

    private void Start()
    {
        startSize = targetText.fontSize;
        sizeDifference = Mathf.Abs(enlargedSize - startSize);
    }

    public void StartTextEnlargment()
    {
        StopAllCoroutines();
        StartCoroutine(EnlargeText());
        enlarged = !enlarged;
    }

    IEnumerator EnlargeText()
    {
        float targetSize = enlarged ? startSize : enlargedSize;
        float currentSize = targetText.fontSize;
        int sign = enlarged ? -1 : 1;



        while (currentSize >= startSize && currentSize <= enlargedSize)
        {
            currentSize += Time.deltaTime * smoothTime * sign;
            targetText.fontSize = currentSize;
            yield return null;
        }

        targetText.fontSize = targetSize;

        
    }

}
