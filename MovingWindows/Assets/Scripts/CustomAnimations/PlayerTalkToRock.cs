using UnityEngine;

public class PlayerTalkToRock : CustomAnimation
{
    [SerializeField] SpeechBubbleManager speechBubbleManager;
    [SerializeField] Animator playerAnimator;

    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] PortalDoor pd;
    protected override void PlayAnimation()
    {
        base.PlayAnimation();
        playerAnimator.SetInteger("State", 0);
        speechBubbleManager.gameObject.SetActive(true);

        boxCollider.enabled = true;
        pd.ChangeToGlowMaterial();


    }


}
