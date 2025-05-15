using UnityEngine;

public class PortalOutline : MonoBehaviour
{
    Material material;
    public float fadeInSpeed = 0.1f;
    public float alphaValue = 0f;
    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {

        if (alphaValue >= 1)
        {
            alphaValue = 1;
        }
        else
        {
            alphaValue += Time.deltaTime * fadeInSpeed;
            
        }

        material.SetFloat("_AlphaValue", alphaValue);

    }
}
