using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    ScreenFadeOut screenFadeOut;
    private void Start()
    {
        screenFadeOut = GetComponent<ScreenFadeOut>();
    }
    public void StartGame()
    {
        StartCoroutine(screenFadeOut.ChangeExposure());

    }


}
