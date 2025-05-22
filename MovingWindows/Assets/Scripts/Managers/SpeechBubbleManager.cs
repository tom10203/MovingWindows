using UnityEngine;
using UnityEngine.InputSystem;

public class SpeechBubbleManager : MonoBehaviour
{
    [SerializeField] SpeachBubble mageSpeechBubble;
    [SerializeField] SpeachBubble wizardSpeechBubble;

    [SerializeField] PlayerInput input;
    [SerializeField] bool mageSpeaking = true;

    [SerializeField] GameStateManager gameStateManager;

    public bool disableMage;
    public bool disableWizard;

    private void Start()
    {
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


    }

    void Update()
    {
        if (input.actions["Attack"].WasPressedThisFrame())
        {
            Debug.Log($"Next line clicked");

            if (mageSpeechBubble.gameObject && wizardSpeechBubble.gameObject && mageSpeechBubble.finished && wizardSpeechBubble.finished)
            {
                Debug.Log($"Destroy speachbubbles");
                gameStateManager.ToggleScripts();
                Destroy(mageSpeechBubble.gameObject);
                Destroy(wizardSpeechBubble.gameObject);
                Destroy(gameObject);
            }

            Debug.Log($"Here");

            if (mageSpeaking)
            {
                mageSpeechBubble.gameObject.SetActive (true);
                mageSpeechBubble.EnableSR();

                if (wizardSpeechBubble.disable)
                {
                    wizardSpeechBubble.DisableSR();
                }

                if (mageSpeechBubble.gameObject)
                {
                    mageSpeechBubble.WriteTextLine();
                }
            }
            else
            {
                Debug.Log($"Here 2");

                wizardSpeechBubble.gameObject.SetActive(true);
                wizardSpeechBubble.EnableSR();

                if (mageSpeechBubble.disable)
                {
                    mageSpeechBubble.DisableSR();
                }

                if (wizardSpeechBubble.gameObject)
                {
                    wizardSpeechBubble.WriteTextLine();
                }
            }

            
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
}
