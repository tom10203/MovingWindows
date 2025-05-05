using UnityEngine;

public class FloatingLetter : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float moveDistanceX;
    [SerializeField] float moveDistanceY;

    Vector3 startPosition;
    float startOffset;
    float yOffset;

    private void Start()
    {
        startPosition = transform.position;
        startOffset = Random.Range(0f, 1000f);
        yOffset = Random.Range(0f, 1000f);
    }

    private void Update()
    {
        // remap perline noise into -1,1 values
        // Add move distance * noise value to start value

        float perlinX = Mathf.PerlinNoise1D(startOffset + Time.time * moveSpeed) * 2  -1;
        float perlinY = Mathf.PerlinNoise1D(yOffset + Time.time * moveSpeed) * 2 - 1;
        transform.position = startPosition + Vector3.left * moveDistanceX * perlinX + Vector3.up * moveDistanceY * perlinY;
    }
}
