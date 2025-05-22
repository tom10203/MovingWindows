using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CutSceneManager : MonoBehaviour
{
    public PlayableDirector director;
    public int nextSceneNumber;

    void Start()
    {
        if (director == null)
        {
            director = GetComponent<PlayableDirector>();
        }

        // Subscribe to the stopped event
        director.stopped += OnTimelineFinished;
    }

    void OnTimelineFinished(PlayableDirector pd)
    {
        // Optional: Unsubscribe to avoid multiple calls
        director.stopped -= OnTimelineFinished;

        // Load the next scene
        SceneManager.LoadScene(nextSceneNumber);
    }
}