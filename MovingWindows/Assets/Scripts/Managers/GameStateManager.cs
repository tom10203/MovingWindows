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

    public enum GameState
    {
        inPlay,
        inAnimation
    }

    public void ToggleScripts(bool active, bool ignoreCamera)
    {

        foreach (var script in scripts)
        {
            if (ignoreCamera && script.GetType() == typeof(CameraFollow)) continue;
            script.inPlay = active;
        }

    }

}
