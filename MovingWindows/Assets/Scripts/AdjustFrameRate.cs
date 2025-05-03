using UnityEngine;

public class AdjustFrameRate : MonoBehaviour
{
    [SerializeField] float time;
    
    void Update()
    {
        Time.timeScale = time;
    }
}
