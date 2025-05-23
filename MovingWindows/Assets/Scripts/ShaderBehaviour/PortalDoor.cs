using UnityEngine;

public class PortalDoor : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer;
    Material material;

    [SerializeField] Material glowMaterial;
    [SerializeField] Material baseMaterial;

    [SerializeField] float minIntensity;
    [SerializeField] float maxIntensity;
    [SerializeField] float speed;

    float t;
    bool glow;

    [SerializeField] GameObject[] rocks;

    private void Start()
    {
        spriteRenderer.material = baseMaterial;
    }
    void Update()
    {
        if (glow)
        {
            Glow();
        }
       
    }

    void Glow()
    {
        t += Time.deltaTime * speed;

        float sin = Mathf.Sin(t);

        sin = (sin + 1) / 2;

        float intensity = Mathf.Lerp(minIntensity, maxIntensity, sin);

        glowMaterial.SetFloat("_Intensity", intensity);
    }

    public void ChangeToGlowMaterial()
    {
        Debug.Log($"Changing material to glow material");
        ChanegRockRenderOrder();

        spriteRenderer.sharedMaterial = glowMaterial;
        spriteRenderer.color = Color.white;
        glow = true;
    }

    void ChanegRockRenderOrder()
    {
        foreach (var rock in rocks)
        {
            SpriteRenderer sr = rock.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sortingOrder = 9;
            }
        }
    }
}
