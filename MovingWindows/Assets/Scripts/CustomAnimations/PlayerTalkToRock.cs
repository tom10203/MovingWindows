using UnityEngine;

public class PlayerTalkToRock : CustomAnimation
{
    [SerializeField] SpeechBubbleManager speechBubbleManager;
    [SerializeField] Animator playerAnimator;
    protected override void PlayAnimation()
    {
        base.PlayAnimation();
        playerAnimator.SetInteger("State", 0);
        speechBubbleManager.gameObject.SetActive(true);


    }


}
