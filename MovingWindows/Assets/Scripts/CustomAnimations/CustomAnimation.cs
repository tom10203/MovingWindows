using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent (typeof(BoxCollider2D))]
public class CustomAnimation : MonoBehaviour
{
    GameStateManager gameStateManager;
    [SerializeField] protected float animationLength;


    private void Start()
    {
        gameStateManager = FindFirstObjectByType<GameStateManager>();

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.gameObject.layer == 3)
        {
            Debug.Log($"Player triggered animation");
            PlayAnimation();
        }
    }
    
    protected virtual void PlayAnimation()
    {
        Debug.Log($"PlayAnimation CUstom animation");
        gameStateManager.gameState = GameStateManager.GameState.inAnimation;
        gameStateManager.ToggleScripts();
    }

    protected void EndAnimation()
    {
        Debug.Log($"End of animation");
        gameStateManager.gameState = GameStateManager.GameState.inPlay;
        Destroy(gameObject);
        gameStateManager.ToggleScripts();
    }

}
