using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class MoveableObject : InteractableObject
{
    Rigidbody2D rb;
    [SerializeField] PlayerInput input;

    bool isMoving;
    bool offsetTransform = true;
    [SerializeField] float offsetAmount;

    [SerializeField] Player2D player;
    [SerializeField] PlayerCharacter2D playerCharacter2D;

    [SerializeField] GameObject lineRenderer1;
    [SerializeField] GameObject lineRenderer2;
    [SerializeField] float maxAlpha = 0.67f;

    float targetVelocityX;
    float targetVelocityY;

    float playerVelocityY;

    float velX;
    float velY;

    bool lineRendererActive;

    Vector2 vel;

    Vector2 offset;

    [SerializeField] float minDst;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canInteract)
        {
            if (input.actions["Interact"].WasPressedThisFrame())
            {
                isMoving = !isMoving;
                if (!lineRenderer1.activeSelf) lineRenderer1.SetActive(true);
                if (!lineRenderer2.activeSelf) lineRenderer2.SetActive(true);

                StartCoroutine(ChangeAlpha());
                offset = transform.position - player.transform.position + Vector3.up * offsetAmount;
            }
        }


    }

    IEnumerator ChangeAlpha()
    {
        float startAlpha  = isMoving ? 0 : maxAlpha;
        float targetAlpha = isMoving ? maxAlpha : 0;
        float t = 0;

        while (t < 0.5f)
        {
            t += Time.deltaTime / 0.5f;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            SetAlpha(alpha);
            yield return null;
        }

        SetAlpha(targetAlpha);

        if (!isMoving)
        {
            ToggleLineRenderer();
        }

    }

    void SetAlpha(float alpha)
    {
        Material mat1 = lineRenderer1.GetComponent<LineRenderer>().material;
        Material mat2 = lineRenderer2.GetComponent<LineRenderer>().material;

        mat1.SetFloat("_Alpha", alpha);
        mat2.SetFloat("_Alpha", alpha);
    }

    void ToggleLineRenderer()
    {
        lineRenderer1.SetActive(!lineRenderer1);
        lineRenderer2.SetActive(!lineRenderer2);
    }

    void FixedUpdate()
    {
  
        if (isMoving)
        {
            //rb.linearVelocity = player.velocity;
            Vector2 currentPos = rb.position;
            Vector2 targetPos = (Vector2)player.transform.position + offset;
            Vector2 smoothedPos = Vector2.Lerp(currentPos, targetPos, 0.2f); // adjust factor
            playerCharacter2D.collisions.isMovingObject = true;



            float dist = (currentPos - (Vector2)player.transform.position).magnitude;
            if (dist < minDst && input.actions["Move"].ReadValue<Vector2>() != Vector2.zero)
            {
                rb.MovePosition(player.transform.position + (transform.position - player.transform.position));
            }

            rb.MovePosition(smoothedPos);

            //rb.MovePosition(targetPos);
            rb.gravityScale = 0;
        }
        else
        {
            rb.gravityScale = 1;
            playerCharacter2D.collisions.isMovingObject = false;
        }
    }

    public void MoveTransform(Vector2 moveAmount)
    {
        rb.MovePosition(moveAmount);
    }
    
}
