using UnityEngine;

public enum GrandsonState { Idling, Following, Escaping }

public class GrandsonController : MonoBehaviour
{
    private Transform target;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    [SerializeField] public float activationDistance = 8f;
    [SerializeField] public float targetDistance = 2f;
    [SerializeField] private float movementForce = 10f;
    public GrandsonState state = GrandsonState.Idling;
    private bool nextToPlayer = true;

    void Start()
    {
        target = FindFirstObjectByType<CharacterMovement>().transform;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (state == GrandsonState.Following)
            MoveTowardsPlayer();
        else if (state == GrandsonState.Escaping)
            ExitTheVillage();
    }

    private void MoveTowardsPlayer()
    {
        if (target == null) return;
        float currentDistance = Vector2.Distance(transform.position, target.position);
        if (currentDistance > activationDistance && nextToPlayer)
        {
            nextToPlayer = false;
            float dirX = target.position.x - transform.position.x;
            float dir = Mathf.Sign(dirX);
            rb.linearVelocityX = dir * movementForce;
            if (dir > 0)
                spriteRenderer.flipX = true;
            else
                spriteRenderer.flipX = false;
        }
        if (currentDistance < targetDistance && !nextToPlayer)
        {
            nextToPlayer = true;
            rb.linearVelocityX = 0;
        }
    }

    private void ExitTheVillage()
    {
        spriteRenderer.flipX = false;
        rb.linearVelocityX = -movementForce;
    }
}