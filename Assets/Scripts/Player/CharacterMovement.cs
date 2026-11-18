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
    private bool talking = false;
    private bool shopping = false;
    private Direction currentDirection = Direction.Left;
    private int walkLoopId = 0;

    private void Start()
    {
        inputHandler = FindFirstObjectByType<InputHandler>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        animator = gameObject.GetComponent<Animator>();
        inputHandler.onMoveInput += SetMoveInput;
        DialoguesManager.instance.onDialogueActivation += SetTalking;
        UIManager.instance.onShopInteraction += SetShopping;
    }

    private void OnDestroy()
    {
        SFXManager.StopLoop(walkLoopId);
        inputHandler.onMoveInput -= SetMoveInput;
        DialoguesManager.instance.onDialogueActivation -= SetTalking;
        UIManager.instance.onShopInteraction -= SetShopping;
    }

    private void Update()
    {
        UpdateWalkSound();
    }

    // Suono "CamminataLoop" finché il giocatore cammina nelle scene del villaggio.
    private void UpdateWalkSound()
    {
        bool walking = !talking && !shopping && Mathf.Abs(moveInput) > 0.01f
            && GameManager.instance != null && GameManager.instance.GetCurrentScene() == Scene.Village;
        if (walking && walkLoopId == 0)
        {
            walkLoopId = SFXManager.PlayLoop(SFX.CamminataLoop);
        }
        else if (!walking && walkLoopId != 0)
        {
            SFXManager.StopLoop(walkLoopId);
            walkLoopId = 0;
        }
    }

    private void FixedUpdate()
    {
        if (!talking && !shopping)
        {
            MoveCharacter();
            UpdateSpriteAndAnimation();
        }
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

    public void SetTalking(bool value)
    {
        talking = value;
        rb.linearVelocityX = 0;
    }

    public void SetShopping(bool value)
    {
        shopping = value;
        rb.linearVelocityX = 0;
    }
}
