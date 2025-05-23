using UnityEngine;

public class FinishLevel : CustomAnimation
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void PlayAnimation()
    {
        Debug.Log($"FInish game animation");
        base.PlayAnimation();
    }
}
