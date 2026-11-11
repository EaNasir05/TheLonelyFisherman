using System.Collections;
using UnityEngine;

public class FishController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Collider2D trigger;
    public string fishType;
    public float strength;
    public int value;
    [SerializeField] private float movementSpeed;
    [SerializeField] private float diagonalMovementDuration;
    [SerializeField] private float diagonalMovementDelay;
    [SerializeField] private float maxRotation;
    [SerializeField] private float minRotation;
    private Vector2 movementRangeX;
    private Vector2 movementRangeY;
    private float facingDirection;
    [SerializeField] private Vector2 movementDir;
    [SerializeField] private float movementRot;
    private bool diagonalPhase;
    [SerializeField] private float fallDuration;
    public bool goingUp;
    public bool dead;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Move();
    }

    private void OnEnable()
    {
        if (Random.Range(0, 2) == 0)
            goingUp = true;
        else
            goingUp = false;
        
        if (Random.Range(0, 2) == 0)
            ChangeHorizontalDirection(Direction.Left);
        else
            ChangeHorizontalDirection(Direction.Right);
        StartCoroutine(MovementRoutine());
    }

    private void OnDisable()
    {
        //trigger.enabled = true;
        dead = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Collisione con l'amo
    }

    public void SetMovementRange(Vector2 rangeX, Vector2 rangeY)
    {
        movementRangeX = rangeX;
        movementRangeY = rangeY;
    }

    private void Move()
    {
        if (!dead)
        {
            transform.position += (Vector3)(movementSpeed * Time.deltaTime * movementDir);
            CheckBoundaries();
        }
    }

    private void ChangeHorizontalDirection(Direction newDirection)
    {
        if (newDirection == Direction.Left)
        {
            facingDirection = -1;
            if (spriteRenderer != null)
                spriteRenderer.flipX = false;
            float newRot = diagonalPhase ? movementRot : 0f;
            SetMovementDirection(newRot);
        }
        else
        {
            facingDirection = 1;
            if (spriteRenderer != null)
                spriteRenderer.flipX = true;
            float newRot = diagonalPhase ? movementRot : 0f;
            SetMovementDirection(newRot);
        }
    }

    private void InvertHorizontalDirection()
    {
        facingDirection *= -1;
        if (spriteRenderer != null)
            spriteRenderer.flipX = facingDirection == 1;
        float newRot = diagonalPhase ? movementRot : 0f;
        SetMovementDirection(newRot);
    }

    private void InvertVerticalDirection()
    {
        float newRot = diagonalPhase ? -movementRot : 0f;
        SetMovementDirection(newRot);
    }

    public IEnumerator Fall()
    {
        yield return null;
    }

    private IEnumerator MovementRoutine()
    {
        while (!dead)
        {
            diagonalPhase = true;
            float randomAngle = 0;
            if (goingUp)
                randomAngle = Random.Range(minRotation, maxRotation);
            else
                randomAngle = Random.Range(-minRotation, -maxRotation);
            SetMovementDirection(randomAngle);
            yield return new WaitForSeconds(diagonalMovementDuration);

            diagonalPhase = false;
            SetMovementDirection(0f);
            goingUp = !goingUp;
            yield return new WaitForSeconds(diagonalMovementDelay);
        }
    }

    private void SetMovementDirection(float angleInDegrees)
    {
        movementRot = angleInDegrees;
        float rad = angleInDegrees * Mathf.Deg2Rad;
        float x = Mathf.Cos(rad) * facingDirection;
        float y = 0f;
        if (angleInDegrees > 0f)
        {
            y = Mathf.Sin(rad) * (Random.value > 0.5f ? 1f : -1f);
        }
        movementDir = new Vector2(x, y).normalized;
    }

    private void CheckBoundaries()
    {
        Vector3 pos = transform.position;

        if (pos.x >= movementRangeX.y && facingDirection == 1)
            InvertHorizontalDirection();
        else if (pos.x <= movementRangeX.x && facingDirection == -1)
            InvertHorizontalDirection();

        if (pos.y >= movementRangeY.y && movementRot > 0)
            InvertVerticalDirection();
        else if (pos.y <= movementRangeY.x && movementRot < 0)
            InvertVerticalDirection();
    }
}
