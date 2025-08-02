using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// Detects press, hold, and release actions using Unity's Input System.
/// Exposes UnityEvents for easy integration in the Inspector.
/// </summary>
public class PressReleaseDetector : MonoBehaviour
{
    private PlayerControls playerControls;

    private bool isPressed = false;

    [Header("Events")]
    [Tooltip("Event fired once when the user initially presses the screen. Passes the screen position.")]
    [SerializeField] private UnityEvent<Vector2> OnPress;

    [Tooltip("Event fired every frame while the user holds the press. Passes the current screen position.")]
    [SerializeField] private UnityEvent<Vector2> OnHold; // <-- NEW EVENT

    [Tooltip("Event fired once when the user releases the screen. Passes the screen position.")]
    [SerializeField] private UnityEvent<Vector2> OnRelease;

    private void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnEnable()
    {
        playerControls.Player.Enable();
        playerControls.Player.PrimaryContact.started += OnPressStarted;
        playerControls.Player.PrimaryContact.canceled += OnPressCanceled;
    }

    private void OnDisable()
    {
        playerControls.Player.PrimaryContact.started -= OnPressStarted;
        playerControls.Player.PrimaryContact.canceled -= OnPressCanceled;
        playerControls.Player.Disable();
    }

    private void Update()
    {
        if (isPressed)
        {
            Vector2 currentPosition = Camera.main.ScreenToWorldPoint(playerControls.Player.PrimaryPosition.ReadValue<Vector2>());
            OnHold?.Invoke(currentPosition);
        }
    }

    private void OnPressStarted(InputAction.CallbackContext context)
    {
        isPressed = true;

        Vector2 pressPosition = Camera.main.ScreenToWorldPoint(playerControls.Player.PrimaryPosition.ReadValue<Vector2>());

        OnPress?.Invoke(pressPosition);
    }

    private void OnPressCanceled(InputAction.CallbackContext context)
    {
        isPressed = false;

        Vector2 releasePosition = Camera.main.ScreenToWorldPoint(playerControls.Player.PrimaryPosition.ReadValue<Vector2>());

        OnRelease?.Invoke(releasePosition);
    }
}