using System.Collections;
using UnityEngine;

public class Hook : MonoBehaviour
{
    private InputHandler inputHandler;
    private Rigidbody2D rb;
    private Shooting shootingSystem;

    [Header("Movement")]
    [SerializeField] private float horizontalSpeed = 5f;
    public float horizontalSpeedMultiplier = 1f;
    [SerializeField] private Vector2 movementRange;
    [SerializeField] private float verticalSpeed = 5f;
    [SerializeField] private float verticalSpeedAcceleration = 10f;
    [SerializeField] private float verticalSpeedDeceleration = 10f;
    public float verticalSpeedMultiplierWhileReturning = 1f;
    [SerializeField] private float verticalForceWhileReturning = 100000f;
    [SerializeField] private float verticalForceCooldown = 0.3f;
    [SerializeField] private float linearDumpOnReturn = 10f;
    public float verticalSpeedUpMultiplier = 1.5f;
    public float verticalSpeedDownMultiplier = 0.5f;
    private Vector2 contactPoint;
    private float verticalRange;

    [Header("Catching")]
    public float power;
    [Range(1, 100)] [SerializeField] private int standardCriticalChance = 5;
    [Range(1, 100)] public int criticalChanceWithMagic = 5;
    [Range(1, 100)] public int criticalChanceWithMagicWhileReturning = 5;
    public float criticalCatchPointsMultiplier = 1f;

    [Header("Stamina")]
    public float maxStamina = 100f;
    private float currentStamina;
    public float staminaConsumedPerSecond = 1f;
    public float staminaConsumedMultiplierOnSpeedUp = 1.5f;
    public float staminaConsumedMultiplierOnSpeedDown = 1f;
    public float staminaLossOnCollisionMultiplier = 1f;

    [Header("Weight")]
    [SerializeField] private float maxWeight = 100f;
    private float currentWeight;
    public float weightGainMultiplier = 1f;
    public float weightAccumulatedPerSecond = 0.1f;
    public float overweightPointsLossMultiplier = 1f;
    public float weightGainOnCollisionMultiplier = 1f;

    [Header("Character sprite settings")]
    [SerializeField] private SpriteRenderer characterSpriteRenderer;
    [SerializeField] private Transform rodStartPoint;
    [SerializeField] private float posXToFlipSprite;
    private Vector2 hookLeftStartPos;
    private Vector2 hookRightStartPos;

    private float moveInput;
    private bool speedUp;
    private bool speedDown;
    private bool inTheSea = false;
    private bool returning = false;
    private bool grandsonHere;
    private bool decelerating = false;
    private bool acceleratingOnReturn = false;
    private bool overweight = false;
    private bool readyToDash = true;

    public bool IsInTheSea() => inTheSea;
    public bool IsReturning() => returning;
    public Vector2 GetContactPoint() => contactPoint;
    public float GetCurrentStamina() => currentStamina;
    public float GetCurrentWeight() => currentWeight;
    public float GetMaxWeight() => maxWeight;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        inputHandler = FindFirstObjectByType<InputHandler>();
        inputHandler.onMoveInput += UpdateMoveInput;
        inputHandler.onSpeedUpInput += SpeedUp;
        inputHandler.onSpeedDownInput += SpeedDown;
        grandsonHere = GameManager.instance.IsGrandsonHere();
        shootingSystem = FindFirstObjectByType<Shooting>();
        hookLeftStartPos = new Vector2(shootingSystem.GetHookStartingXPosLeft(), transform.position.y);
        hookRightStartPos = new Vector2(shootingSystem.GetHookStartingXPosRight(), transform.position.y);
        verticalRange = FishingManager.instance.biomesHeight * FishingManager.instance.biomesUnlocked * -1;
    }

    private void FixedUpdate()
    {
        if (inTheSea && !FishingManager.instance.stop)
        {
            if (decelerating)
                VerticalDeceleration();
            else if (acceleratingOnReturn)
                VerticalAccelerationOnReturn();
        }
    }

    private void Update()
    {
        if (inTheSea && !FishingManager.instance.stop)
        {
            MoveHorizontally();
            UpdateStaminaAndWeightOvertime();
            CheckVerticalPosition();
        }
    }

    private void OnDestroy()
    {
        inputHandler.onMoveInput -= UpdateMoveInput;
        inputHandler.onSpeedUpInput -= SpeedUp;
        inputHandler.onSpeedDownInput -= SpeedDown;
    }

    private void UpdateMoveInput(float value) => moveInput = value;

    private void SpeedUp(bool value)
    {
        if (!returning)
        {
            if (value)
            {

            }
            else
            {
                
            }
        }
        else if (value && inTheSea && !FishingManager.instance.stop && readyToDash)
        {
            rb.AddForce(Vector2.up * verticalForceWhileReturning);
            StartCoroutine(RechargeVerticalForce());
        }
    }

    private void SpeedDown(bool value)
    {
        if (value)
        {

        }
        else
        {
            
        }
    }

    private void MoveHorizontally()
    {
        float posX = Mathf.Clamp(transform.position.x + moveInput * horizontalSpeed * horizontalSpeedMultiplier * Time.deltaTime, movementRange.x, movementRange.y);
        transform.position = new Vector2(posX, transform.position.y);
        contactPoint = new Vector2(posX, contactPoint.y);
        if (shootingSystem.characterDirection == Direction.Left && posX > posXToFlipSprite)
        {
            characterSpriteRenderer.flipX = true;
            rodStartPoint.position = new Vector2(hookRightStartPos.x, rodStartPoint.position.y);
            shootingSystem.transform.position = hookRightStartPos;
            shootingSystem.characterDirection = Direction.Right;
        }
        else if (shootingSystem.characterDirection == Direction.Right && posX < posXToFlipSprite)
        {
            characterSpriteRenderer.flipX = false;
            rodStartPoint.position = new Vector2(hookLeftStartPos.x, rodStartPoint.position.y);
            shootingSystem.transform.position = hookLeftStartPos;
            shootingSystem.characterDirection = Direction.Left;
        }
    }

    private void VerticalDeceleration()
    {
        //Decelerazione
    }

    private void VerticalAccelerationOnReturn()
    {
        float targetSpeed = verticalSpeed * verticalSpeedMultiplierWhileReturning;
        float movement = verticalSpeedAcceleration * (targetSpeed - rb.linearVelocityY);
        if (movement == targetSpeed)
            acceleratingOnReturn = false;
        rb.linearVelocityY = movement;
    }

    private void UpdateStaminaAndWeightOvertime()
    {
        if (!returning)
        {
            if (speedUp)
                DecreaseStamina(staminaConsumedPerSecond * staminaConsumedMultiplierOnSpeedUp * Time.deltaTime);
            else if (speedDown)
                DecreaseStamina(staminaConsumedPerSecond * staminaConsumedMultiplierOnSpeedDown * Time.deltaTime);
            else
                DecreaseStamina(staminaConsumedPerSecond * Time.deltaTime);
        }
        else if (!grandsonHere && !overweight)
        {
            IncreaseWeight(weightAccumulatedPerSecond * weightGainMultiplier * Time.deltaTime);
        }
    }

    public void DecreaseStamina(float decrease)
    {
        currentStamina = Mathf.Clamp(currentStamina - decrease, 0, maxStamina);
        UIManager.instance.UpdateStaminaBar(currentStamina);
        if (currentStamina == 0)
            StartCoroutine(ReturnDelay());
    }

    public void IncreaseWeight(float increase)
    {
        currentWeight = Mathf.Clamp(currentWeight + increase, 0, maxWeight);
        UIManager.instance.UpdateStaminaBar(currentWeight);
        if (currentWeight == maxWeight)
            StartCoroutine(Overweight());
    }

    private void CheckVerticalPosition()
    {
        if (!returning && !FishingManager.instance.stop)
        {
            if (transform.position.y < verticalRange)
                StartCoroutine(ReturnDelay());
        }
    }

    public void EnterTheSea()
    {
        rb.gravityScale = 0;
        contactPoint = transform.position;
        inTheSea = true;
        rb.linearVelocity = Vector2.down * verticalSpeed;
        currentStamina = maxStamina;
        UIManager.instance.RefreshStaminaBar();
        UIManager.instance.ShowStaminaBar();
    }

    public void ExitTheSea()
    {
        inTheSea = false;
        returning = false;
        speedUp = false;
        rb.linearDamping = 0;
        rb.linearVelocity = Vector2.zero;
        UIManager.instance.HideStaminaBar();
        StartCoroutine(RestartFishing());
    }

    private void StartReturning()
    {
        returning = true;
        speedDown = false;
        if (grandsonHere)
        {
            acceleratingOnReturn = true;
            speedUp = true;
        }
        else
        {
            rb.linearDamping = linearDumpOnReturn;
            speedUp = false;
        }
        currentWeight = 0f;
        UIManager.instance.RefreshWeightBar();
    }

    private IEnumerator ReturnDelay()
    {
        rb.linearVelocityY = 0;
        yield return new WaitForSeconds(1f);
        StartReturning();
    }

    private IEnumerator Overweight()
    {
        yield return null;
        //Sovraccarico
    }

    private IEnumerator RechargeVerticalForce()
    {
        readyToDash = false;
        yield return new WaitForSeconds(verticalForceCooldown);
        readyToDash = true;
    }

    private IEnumerator RestartFishing()
    {
        Vector2 startingPos = transform.position;
        Vector2 targetPos = Vector2.zero;
        if (shootingSystem.characterDirection == Direction.Left)
            targetPos = hookLeftStartPos;
        else
            targetPos = hookRightStartPos;
        float elapsedTime = 0f;
        float duration = 0.7f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector2.Lerp(startingPos, targetPos, elapsedTime / duration);
            yield return null;
        }
        transform.position = targetPos;
        shootingSystem.Restart();
    }
}
