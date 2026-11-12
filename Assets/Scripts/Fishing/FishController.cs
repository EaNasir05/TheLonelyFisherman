using System.Collections;
using UnityEngine;

public class FishController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Collider2D trigger;
    public string fishType;
    public float strength;
    public int value;
    private int layer;
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
    [SerializeField] private Vector2 rotWhileDeadRange;
    [SerializeField] private Vector2 posWhileDeadMin;
    [SerializeField] private Vector2 posWhileDeadMax;
    [SerializeField] private float scaleWhileDead;
    private Vector3 startingScale;
    private Quaternion startingRotation;

    public void SetLayer(int layer) => this.layer = layer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        trigger = GetComponent<Collider2D>();
        startingScale = transform.localScale;
        startingRotation = transform.rotation;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Hook hook = collision.gameObject.GetComponent<Hook>();
            if (!hook.IsOverweight())
            {
                dead = true;
                trigger.enabled = false;
                FishesSpawner.instance.RemoveFishFromFishesActivatedPerLayer(layer, fishType, gameObject);
                transform.SetParent(hook.transform);
                hook.CatchFish(this, strength, value);
                MoveTowardsHook(collision.transform);
            }
        }
    }

    public void ResetFish()
    {
        if (trigger != null)
            trigger.enabled = true;
        dead = false;
        transform.localScale = startingScale;
        transform.rotation = startingRotation;
        FishesPool.instance.DeactivateFish(gameObject, fishType);
    }

    private void MoveTowardsHook(Transform hook)
    {
        float targetRotation = Random.Range(rotWhileDeadRange.x, rotWhileDeadRange.y);
        Vector2 targetPosition = Vector2.Lerp(posWhileDeadMin, posWhileDeadMax, (targetRotation - rotWhileDeadRange.x) / (rotWhileDeadRange.y - rotWhileDeadRange.x));
        float targetScale = scaleWhileDead;
        transform.rotation = Quaternion.Euler(0, 0, targetRotation);
        transform.localPosition = targetPosition;
        transform.localScale = new Vector3(targetScale, targetScale, 1);
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

    public IEnumerator Fall()
    {
        transform.parent = null;
        transform.localScale = startingScale;
        float elapsedTime = 0;
        float fallSpeed = Random.Range(2f, 4f);
        float rotationSpeed = Random.Range(-90f, 90f);
        while (elapsedTime < fallDuration && dead)
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        ResetFish();
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
                randomAngle = Random.Range(-maxRotation, -minRotation);
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
        float y = Mathf.Sin(rad); 
        movementDir = new Vector2(x, y).normalized;
    }

    private void InvertVerticalDirection()
    {
        float newRot = -movementRot;
        goingUp = !goingUp;
        SetMovementDirection(newRot);
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
