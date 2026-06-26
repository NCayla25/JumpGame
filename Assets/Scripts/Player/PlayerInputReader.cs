using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerInputReader : MonoBehaviour
{
    public static PlayerInputReader Instance { get; private set; }

    public event Action JumpPressed;
    public event Action UpPressed;
    public event Action LeftPressed;
    public event Action RightPressed;

    private InputAction jumpAction;
    private InputAction upAction;
    private InputAction leftAction;
    private InputAction rightAction;
    private InputAction restartAction;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        CreateInputActions();
    }

    private void OnEnable()
    {
        EnableInputActions();
    }

    private void OnDisable()
    {
        DisableInputActions();
    }

    private void OnDestroy()
    {
        DisposeInputActions();
    }

    private void CreateInputActions()
    {
        jumpAction = new InputAction(
            name: "Jump",
            type: InputActionType.Button
            );

        jumpAction.AddBinding("<Pointer>/press");
        jumpAction.AddBinding("<Keyboard>/space");
        jumpAction.AddBinding("<Gamepad>/buttonSouth");

        upAction = new InputAction(
            name: "Choose Up",
            type: InputActionType.Button
            );

        upAction.AddBinding("<Keyboard>/w");
        upAction.AddBinding("<Keyboard>/upArrow");
        upAction.AddBinding("<Gamepad>/dpad/up");

        leftAction = new InputAction(
            name: "Choose Left",
            type: InputActionType.Button
            );

        leftAction.AddBinding("<Keyboard>/a");
        leftAction.AddBinding("<Keyboard>/leftArrow");
        leftAction.AddBinding("<Gamepad>/dpad/left");

        rightAction = new InputAction(
            name: "Choose Right",
            type: InputActionType.Button
            );

        rightAction.AddBinding("<Keyboard>/d");
        rightAction.AddBinding("<Keyboard>/rightArrow");
        rightAction.AddBinding("<Gamepad>/dpad/right");

        restartAction = new InputAction(
            name: "Restart",
            type: InputActionType.Button
            );

        restartAction.AddBinding("<Keyboard>/r");
        restartAction.AddBinding("<Gamepad>/dpad/down");

        jumpAction.performed += OnJumpPerformed;
        upAction.performed += _ => UpPressed?.Invoke();
        leftAction.performed += _ => LeftPressed?.Invoke();
        rightAction.performed += _ => RightPressed?.Invoke();
        restartAction.performed += _ => TryRestart();
    }

    private void EnableInputActions()
    {
        jumpAction?.Enable();
        upAction?.Enable();
        leftAction?.Enable();
        rightAction?.Enable();
        restartAction?.Enable();
    }

    private void DisableInputActions()
    {
        jumpAction?.Disable();
        upAction?.Disable();
        leftAction?.Disable();
        rightAction?.Disable();
        restartAction?.Disable();
    }

    private void DisposeInputActions()
    {
        jumpAction?.Dispose();
        upAction?.Dispose();
        leftAction?.Dispose();
        rightAction?.Dispose();
        restartAction?.Dispose();
    }

    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (IsPointerOverUi())
            return;

        JumpPressed?.Invoke();
    }

    private bool IsPointerOverUi()
    {
        if (EventSystem.current == null)
            return false;

        return EventSystem.current.IsPointerOverGameObject();
    }

    private void TryRestart()
    {
        if (GameFlow.Instance != null && GameFlow.Instance.IsGameOver)
        {
            GameFlow.Instance.RestartScene();
        }
    }
}
