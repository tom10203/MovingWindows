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
            PlayAnimation();
        }
    }
    
    protected virtual void PlayAnimation()
    {
        gameStateManager.gameState = GameStateManager.GameState.inAnimation;
        gameStateManager.ToggleScripts();
    }

    protected void EndAnimation()
    {
        gameStateManager.gameState = GameStateManager.GameState.inPlay;
        Destroy(gameObject);
        gameStateManager.ToggleScripts();
    }

}
