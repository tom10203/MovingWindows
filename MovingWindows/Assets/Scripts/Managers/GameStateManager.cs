using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    bool toggleState = true;
    public GameState gameState = GameState.inPlay;
    public GameState oldState;
    public int state;

    public InPlayScript[] scripts;
    private void Awake()
    {
        oldState = gameState;
    }

    //private void Update()
    //{


    //    if (oldState != gameState)
    //    {
    //        Debug.Log($"Changing state");
    //        ToggleScripts();
    //    }


    //    oldState = gameState;
    //}
    public enum GameState
    {
        inPlay,
        inAnimation
    }

    public void ToggleScripts()
    {
        // Camera
        // Player
        // Portal


        foreach (var script in scripts)
        {
            script.inPlay = !script.inPlay;
        }

       

    }

}
