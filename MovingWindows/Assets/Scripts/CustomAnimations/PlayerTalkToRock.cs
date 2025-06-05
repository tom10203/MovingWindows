using UnityEngine;

public class PlayerTalkToRock : CustomAnimation
{
    [SerializeField] Animator playerAnimator;

    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] PortalDoor pd;

    [SerializeField] bool activatePortalDoor;
    protected override void PlayAnimation()
    {
        base.PlayAnimation();
        playerAnimator.SetInteger("State", 0);

        if (activatePortalDoor)
        {
            boxCollider.enabled = true;
            pd.ChangeToGlowMaterial();
        }

        EndAnimation();
    }


}
