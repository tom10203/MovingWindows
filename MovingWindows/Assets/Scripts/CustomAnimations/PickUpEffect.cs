using UnityEngine;

public class PickUpEffect : MonoBehaviour
{
    [Header("Trail Renderer")]
    [SerializeField] float radius;
    [SerializeField] Transform center;
    [SerializeField] float speed;
    [SerializeField] float perlinSpeed;
    [SerializeField] float heightOffset = 0.3f;
    float timeOffset;
    float perlinOffset;

    [Header("Line Renderer")]
    [SerializeField] Vector3[] points;
    [SerializeField] int noOfPoints;
    [SerializeField] float height;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] float frequencyExp;
    [SerializeField] float amplitudeExp;
    [SerializeField] int octaves;
    float offset;
    //[SerializeField] int frequency;

    private void Start()
    {
        //timeOffset = Random.Range(0f, Mathf.PI);
        //perlinOffset = Random.Range(0f, 1000f);

        offset = Random.Range(0, 2 * Mathf.PI);
    }

    void Update()
    {
        // h = radius
        // a = cos(theta) * h
        // o = sin(theta) * h
 
        InitializePoints();
        
        
    }

    void TrailRendererMovement()
    {
        float theta = timeOffset + Time.time * speed;

        float perlin = Mathf.PerlinNoise1D(perlinOffset + Time.time * perlinSpeed);

        float inputRadius = radius + heightOffset * perlin;

        float x = Mathf.Cos(theta) * inputRadius;
        float y = Mathf.Sin(theta) * inputRadius;

        Vector2 offset = new Vector2(x, y);

        transform.position = center.position + (Vector3)offset;
    }

    void InitializePoints()
    {
        lineRenderer.positionCount = 0;
        lineRenderer.positionCount = noOfPoints;

        float dx = (radius * 2) / (noOfPoints - 1);
        float dy = (2 * Mathf.PI) / (noOfPoints - 1);

        Vector3 startPoint = transform.position + Vector3.left * radius;


        for (int i = 0; i < noOfPoints; i++)
        {
            float h = radius;

            float totalHeight = startPoint.y;
            float amplitude = height;
            float frequency = 1;

            for (int j = 0; j < octaves; j++)
            {
                h += Mathf.Sin((offset + Time.time * speed + i * dy) * frequency) * amplitude;

                totalHeight += Mathf.Sin((Time.time * speed +  i * dy) * frequency) * amplitude;

                amplitude /= amplitudeExp;
                frequency *= frequencyExp;
            }

            //float x = startPoint.x + i * dx;
            //float y = totalHeight;

            float x = Mathf.Cos(i * dy) * h;
            float y = Mathf.Sin(i * dy) * h;

            Vector2 point = transform.position + new Vector3(x, y, 0);

            lineRenderer.SetPosition(i, point);
        }
    }
}
