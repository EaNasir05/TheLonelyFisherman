using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public Action<float> onMoveInput;
    public Action onInteractInput;
    public Action onNextLineInput;
    public Action<bool> onSpeedUpInput;
    public Action<bool> onSpeedDownInput;
    public Action onBackInput;

    public void OnMove(InputValue input)
    {
        var moveVector = input.Get<Vector2>();
        onMoveInput?.Invoke(moveVector.x);
    }

    public void OnInteract(InputValue input)
    {
        if (input.isPressed)
            onInteractInput?.Invoke();
    }

    public void OnNextLine(InputValue input)
    {
        if (input.isPressed)
            onNextLineInput?.Invoke();
    }

    public void OnSpeedUp(InputValue input)
    {
        onSpeedUpInput?.Invoke(input.isPressed);
    }

    public void OnSpeedDown(InputValue input)
    {
        onSpeedDownInput?.Invoke(input.isPressed);
    }

    public void OnBack(InputValue input)
    {
        if (input.isPressed)
            onBackInput?.Invoke();
    }
}
