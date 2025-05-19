using UnityEngine;

public class TimeLineManager : MonoBehaviour
{
    public GameState gameState = new GameState();
    [SerializeField] Player2D player2D;
    [SerializeField] CustomAnimation[] customAnimations;

    void Start()
    {
        gameState.playingAnimation = true;
        //customAnimations[0].PlayAnimation();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public struct GameState
    {
        public bool playingAnimation;
    }
}
