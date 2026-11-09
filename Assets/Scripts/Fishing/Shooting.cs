using System.Collections.Generic;
using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Shooting : MonoBehaviour
{
    [SerializeField] private AudioClip hookThrowAudio;

    [SerializeField] private Vector2 launchVelocity;
    [Range(0.01f, 0.1f)]
    [SerializeField] private float timeStep = 0.02f;
    [SerializeField] private int maxCalculationSteps = 128;
    [Range(0.01f, 1.0f)]
    [SerializeField] private float tolerance;
    [SerializeField] private bool showHitMarker;
    [SerializeField] private float maxRangeLeft;
    [SerializeField] private float maxRangeRight;
    [SerializeField] private float minRangeLeft;
    [SerializeField] private float minRangeRight;

    [SerializeField] private GameObject hitMarker;
    [SerializeField] private SpriteRenderer characterSpriteRenderer;
    private LineRenderer lineRenderer;
    private InputHandler inputHandler;
    [SerializeField] private Rigidbody2D hookRB;
    private Transform hookTransform;
    [SerializeField] private Transform rodStartPoint;
    [SerializeField] private float hookStartingXPosLeft;
    [SerializeField] private float hookStartingXPosRight;

    private RaycastHit hit;
    readonly List<Vector3> points = new();

    [SerializeField] private bool fire;

    [SerializeField] private float velocityRightLeft;
    private float moveInput;

    [Range(0.01f, 1.0f)]
    [SerializeField] private float lineStartWidth;
    [Range(0.01f, 1.0f)]
    [SerializeField] private float lineEndWidth;

    private bool isShot = false;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        hookRB.constraints = RigidbodyConstraints2D.FreezePositionY;
        hookTransform = hookRB.transform;
        inputHandler = FindFirstObjectByType<InputHandler>();
        inputHandler.onMoveInput += UpdateMoveInput;
        inputHandler.onSpeedUpInput += Shoot;
        //Hook.onFishOutOfWater += Restart;
    }

    private void FixedUpdate()
    {
        if (!isShot)
            DrawTrajectory();
        if (!isShot && !FishingManager.instance.stop)
            MoveTrajectory();
    }

    private void UpdateMoveInput(float value)
    {
        moveInput = value;
    }

    private void MoveTrajectory()
    {
        if (!lineRenderer.enabled)
            lineRenderer.enabled = true;
        float previousVelocityX = launchVelocity.x;
        float newVelocityX = Mathf.Clamp(launchVelocity.x + velocityRightLeft * moveInput, maxRangeLeft, maxRangeRight);
        if (previousVelocityX <= minRangeLeft && newVelocityX > minRangeLeft)
        {
            newVelocityX = minRangeRight;
            characterSpriteRenderer.flipX = !characterSpriteRenderer.flipX;
        }
        else if (previousVelocityX >= minRangeRight && newVelocityX < minRangeRight)
        {
            newVelocityX = minRangeLeft;
            characterSpriteRenderer.flipX = !characterSpriteRenderer.flipX;
        }
        launchVelocity.x = newVelocityX;
    }

    private void DrawTrajectory()
    {
        lineRenderer.startWidth = lineStartWidth;
        lineRenderer.startWidth = lineEndWidth;

        if (!showHitMarker)
            hitMarker.SetActive(false);

        var position = transform.position;
        var velocity = launchVelocity;
        points.Clear();
        points.Add(position);
        while (true)
        {
            bool collision;
            (position, collision) = GetNextPosition(position, velocity);
            points.Add(position);
            if (collision || points.Count >= maxCalculationSteps)
            {
                break;
            }
            velocity += Physics2D.gravity * timeStep;
        }

        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
        lineRenderer.Simplify(tolerance);
    }

    public void Shoot(bool value)
    {
        if (value && !isShot && !FishingManager.instance.stop)
        {
            FishingManager.instance.StartFishingAttempt();
            //SoundEffectsManager.instance.PlaySFXClip(hookThrowAudio, 1);
            isShot = true;
            hookRB.gravityScale = 1f;
            hookRB.transform.position = transform.position;
            hookRB.constraints = RigidbodyConstraints2D.FreezeRotation;
            hookRB.linearVelocity = launchVelocity;
            lineRenderer.positionCount = 0;
            lineRenderer.enabled = false;
        }
    }

    private (Vector2, bool) GetNextPosition(Vector2 currectPoint, Vector2 velocity)
    {
        if (Physics.Raycast(currectPoint, velocity, out hit, velocity.magnitude * timeStep))
        {
            if (showHitMarker)
            {
                hitMarker.SetActive(true);
                hitMarker.transform.SetPositionAndRotation(hit.point, Quaternion.LookRotation(hit.normal));
                hitMarker.transform.Rotate(90, 0, 0);
            }
            return (hit.point, true);
        }
        hitMarker.SetActive(false);
        return (currectPoint + velocity * timeStep, false);
    }

    public void Restart()
    {
        launchVelocity.x = -0.5f;
        showHitMarker = true;
        hitMarker.SetActive(true);
        isShot = false;
        points.Clear();
        lineRenderer.positionCount = points.Count;
    }

    public void OnDestroy()
    {
        //Hook.onFishOutOfWater -= Restart;
        inputHandler.onMoveInput -= UpdateMoveInput;
        inputHandler.onSpeedUpInput -= Shoot;
    }
}