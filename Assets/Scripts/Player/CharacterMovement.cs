using UnityEngine;

public enum Direction { Left, Right }

public class CharacterMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private InputHandler inputHandler;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    private float moveInput = 0f;
    private Direction currentDirection = Direction.Left;

    private void Start()
    {
        inputHandler = FindFirstObjectByType<InputHandler>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        animator = gameObject.GetComponent<Animator>();
        inputHandler.onMoveInput += SetMoveInput;
    }

    private void OnDestroy()
    {
        inputHandler.onMoveInput -= SetMoveInput;
    }

    private void FixedUpdate()
    {
        MoveCharacter();
        UpdateSpriteAndAnimation();
    }

    private void MoveCharacter()
    {
        float targetSpeed = moveInput * moveSpeed;
        float movement = acceleration * (targetSpeed - rb.linearVelocityX);
        rb.AddForce(movement * Vector2.right);
        //rb.linearVelocity = movement * Vector2.right;
    }

    private void UpdateSpriteAndAnimation()
    {
        if (moveInput > 0)
        {
            currentDirection = Direction.Right;
            spriteRenderer.flipX = true;
            //Cambia animazione
        }
        else if (moveInput < 0)
        {
            currentDirection = Direction.Left;
            spriteRenderer.flipX = false;
            //Cambia animazione
        }
        else
        {
            //Cambia animazione
        }
    }

    public void SetMoveInput(float input)
    {
        moveInput = input;
    }
}
