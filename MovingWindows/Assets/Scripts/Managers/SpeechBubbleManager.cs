using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpeechBubbleManager : CustomAnimation
{
    [SerializeField] SpeachBubble mageSpeechBubble;
    [SerializeField] SpeachBubble wizardSpeechBubble;

    [SerializeField] PlayerInput input;
    [SerializeField] bool mageSpeaking = true;


    public bool endInteraction;
    public bool isInteracting;

    private void Start()
    {
        isInteracting = true;
    }

    protected override void PlayAnimation()
    {
        base.PlayAnimation();

        if (mageSpeaking)
        {
            mageSpeechBubble.gameObject.SetActive(true);
            mageSpeechBubble.WriteTextLine();
        }
        else
        {
            wizardSpeechBubble.gameObject.SetActive(true);
            wizardSpeechBubble.WriteTextLine();
        }

        StartCoroutine(Interact());


    }
    IEnumerator Interact()
    {
        while (isInteracting)
        {
            if (input.actions["Attack"].WasPressedThisFrame())
            {


                if (endInteraction)
                {
                    isInteracting = false;
                    EndAnimation();
                    yield break;

                }


                if (mageSpeaking)
                {
                    mageSpeechBubble.gameObject.SetActive(true);
                    mageSpeechBubble.EnableSR();

                    if (wizardSpeechBubble.disable)
                    {
                        wizardSpeechBubble.DisableSR();
                    }

                    mageSpeechBubble.WriteTextLine();

                }
                else
                {
                    wizardSpeechBubble.gameObject.SetActive(true);
                    wizardSpeechBubble.EnableSR();

                    if (mageSpeechBubble.disable)
                    {
                        mageSpeechBubble.DisableSR();
                    }

                    wizardSpeechBubble.WriteTextLine();

                }


            }

            yield return null;
        }
    }

    public void ToggleSpeechBubble()
    {
        mageSpeaking = !mageSpeaking;
    }

    public void ToggleSpeechBubbleActive()
    {
        mageSpeechBubble.gameObject.SetActive(!mageSpeechBubble.gameObject);
        wizardSpeechBubble.gameObject.SetActive(!wizardSpeechBubble.gameObject);
    }

    public void EndInteraction()
    {
        mageSpeechBubble.gameObject.SetActive(false);
        wizardSpeechBubble.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
