using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

//[RequireComponent (typeof(BoxCollider2D))]
public class CustomAnimation : MonoBehaviour
{
    [SerializeField] GameStateManager gameStateManager;
    [SerializeField] protected float animationLength;
    [SerializeField] bool ignoreCameraScript;

    [SerializeField] CustomAnimation nextAnimation;

    private void Start()
    {
        gameStateManager = FindFirstObjectByType<GameStateManager>();
        if (gameStateManager == null)
        {
            Debug.Log($"gamestatae manager is NULL start Function");
        }

    }
    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.layer == 3)
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            PlayAnimation();
        }
    }
    
    protected virtual void PlayAnimation()
    {
        if (gameStateManager == null)
        {
            Debug.Log("Gamestate manager is NULL");
        }
        gameStateManager.gameState = GameStateManager.GameState.inAnimation;
        gameStateManager.ToggleScripts(false, ignoreCameraScript);

        
    }

    protected void EndAnimation()
    {
        gameStateManager.gameState = GameStateManager.GameState.inPlay;

        gameStateManager.ToggleScripts(true, ignoreCameraScript);

        if (nextAnimation != null)
        {
            nextAnimation.PlayAnimation();
        }

        Destroy(gameObject);
    }

}
