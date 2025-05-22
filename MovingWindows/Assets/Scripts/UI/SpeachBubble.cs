using System.Collections;
using TMPro;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    [TextArea(2, 5)] public string text;
    public bool isEndOfSentence;
    public bool disableSpeechBubble;
}

public class SpeachBubble : MonoBehaviour
{
    [SerializeField] private DialogueLine[] textLines;
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private float delay = 0.1f;
    [SerializeField] private SpeechBubbleManager speechBubbleManager;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private int currentLineIndex = 0;
    private bool isWritingText = false;
    private string currentText = "";

    public bool finished = false;
    public bool disable;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void WriteTextLine()
    {
        if (currentLineIndex >= textLines.Length)
        {
            Debug.Log("SpeechBubble index outside of array length");
            return;
        }

        if (isWritingText)
        {
            // Skip animation, show full line
            StopAllCoroutines();
            currentText = textLines[currentLineIndex].text;
            displayText.text = currentText;
            isWritingText = false;
            OnLineFinished();
        }
        else
        {
            StartCoroutine(TypeLine(textLines[currentLineIndex]));
        }
    }

    //private IEnumerator TypeLine(DialogueLine line)
    //{
    //    isWritingText = true;
    //    currentText = "";
    //    displayText.text = "";

    //    foreach (char c in line.text)
    //    {
    //        currentText += c;
    //        displayText.text = currentText;
    //        yield return new WaitForSeconds(delay);
    //    }

    //    isWritingText = false;
    //    OnLineFinished();
    //}


    private IEnumerator TypeLine(DialogueLine line)
    {
        isWritingText = true;
        currentText = "";
        displayText.text = "";

        string[] words = line.text.Split(' ');

        foreach (string word in words)
        {
            string wordToMeasure = word + " ";
            string previewText = currentText + wordToMeasure;

            // Measure the size of the current line with the new word
            Vector2 preferredValues = displayText.GetPreferredValues(previewText);

            // If it exceeds the rect, move to new line
            if (preferredValues.x > displayText.rectTransform.rect.width)
            {
                currentText += "\n";
            }

            // Type word character by character
            foreach (char c in wordToMeasure)
            {
                currentText += c;
                displayText.text = currentText;
                yield return new WaitForSeconds(delay);
            }
        }

        isWritingText = false;
        OnLineFinished();
    }

    private void OnLineFinished()
    {
        // Notify manager if it's the end of a speech block
        if (textLines[currentLineIndex].isEndOfSentence)
        {
            speechBubbleManager.ToggleSpeechBubble();
        }

        disable = textLines[currentLineIndex].disableSpeechBubble ? true : false;


        currentLineIndex++;

        if (currentLineIndex >= textLines.Length)
        {
            finished = true;
        }
    }

    public void DisableSR()
    {
        spriteRenderer.enabled = false;
        displayText.enabled = false;
    }
    public void EnableSR()
    {
        spriteRenderer.enabled = true;
        displayText.enabled = true;
    }
}