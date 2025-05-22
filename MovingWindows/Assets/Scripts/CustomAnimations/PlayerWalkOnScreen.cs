using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWalkOnScreen : CustomAnimation
{
    [SerializeField] Transform player;
    [SerializeField] Animator playerAnimator;
    [SerializeField] float distanceToMove;
    [SerializeField] SpeechBubbleManager speechBubbleManager;

    protected override void PlayAnimation()
    {
        base.PlayAnimation();
        StartCoroutine(WalkOnScreen());
    }
    IEnumerator WalkOnScreen()
    {
        float xVelocity = distanceToMove / animationLength;
        playerAnimator.SetInteger("State", 1);
        Vector2 vel = new Vector2(xVelocity, 0);
        float timer = 0f;

        while (timer < animationLength)
        {
            timer += Time.deltaTime;
            player.Translate(vel * Time.deltaTime);
            yield return null;
        }

        //EndAnimation();
        playerAnimator.SetInteger("State", 0);
        speechBubbleManager.gameObject.SetActive(true);
    }
}
