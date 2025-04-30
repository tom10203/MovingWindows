using UnityEngine;

[RequireComponent (typeof(Player2D))]
public class AnimationManager : MonoBehaviour
{
    Player2D player;
    [SerializeField] Animator animator;

    private void Start()
    {
        player = GetComponent<Player2D>();
    }

    private void Update()
    {
        //if ()
    }
}
