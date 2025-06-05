using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLevel : CustomAnimation
{
    [SerializeField] Transform playerTransform;
    [SerializeField] Transform endPoint;
    [SerializeField] float dstThreshold = 0.2f;
    [SerializeField] float playerXVel = 1;

    [SerializeField] Player2D player;
    [SerializeField] Animator animator;


    [SerializeField] int nextScene;
    protected override void PlayAnimation()
    {
        Debug.Log($"FInish game animation");
        base.PlayAnimation();
        
        StartCoroutine(FinishLevelAnimation());
    }

    IEnumerator FinishLevelAnimation()
    {
        float t = 0;

        while (!player.controller.collisions.below && t < 10f)
        {
            t += Time.deltaTime;
            yield return null;
        }

        t = 0f;
        animator.SetInteger("State", 1);

        while (Vector2.Distance((Vector2)playerTransform.position, (Vector2)endPoint.position) > dstThreshold)
        {
            playerTransform.Translate(Vector3.right * playerXVel * Time.deltaTime);
            yield return null;

        }

        //EndAnimation();
        SceneManager.LoadScene(nextScene);
    }
}
