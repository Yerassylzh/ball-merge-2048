using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic; // Required for List<RaycastResult>

/// <summary>
/// Detects press, hold, and release actions using Unity's Input System.
/// Defers UI checks to the Update loop to ensure reliability.
/// </summary>
public class PressReleaseDetector : MonoBehaviour
{
    private PlayerControls playerControls;

    private bool isPressed = false;
    // Flags to defer input processing to the Update loop,
    // where UI checks are reliable.
    private bool inputStartedThisFrame = false;
    private bool inputEndedThisFrame = false;
    private bool wasInputStartedOverUI = false;

    // Store pending positions from input callbacks
    private Vector2 pendingPressPosition;
    private Vector2 pendingReleasePosition;

    [Header("Events")]
    [Tooltip("Event fired once when the user initially presses the screen. Passes the screen position.")]
    [SerializeField] private UnityEvent<Vector2> OnPress;

    [Tooltip("Event fired every frame while the user holds the press. Passes the current screen position.")]
    [SerializeField] private UnityEvent<Vector2> OnHold;

    [Tooltip("Event fired once when the user releases the screen. Passes the screen position.")]
    [SerializeField] private UnityEvent<Vector2> OnRelease;

    public void Enable()
    {
        playerControls.Player.Enable();
    }

    public void Disable()
    {
        playerControls.Player.Disable();
    }

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
        // --- Input Start Processing ---
        if (inputStartedThisFrame)
        {
            inputStartedThisFrame = false;

            // Check if the press started over a UI element. This is reliable in Update().
            if (IsPointerOverUIElement())
            {
                wasInputStartedOverUI = true;
            }
            else
            {
                wasInputStartedOverUI = false;
                isPressed = true;
                OnPress?.Invoke(pendingPressPosition);
            }
        }

        // --- Input End Processing ---
        if (inputEndedThisFrame)
        {
            inputEndedThisFrame = false;

            // Only trigger release if the press did not start over UI
            if (isPressed && !wasInputStartedOverUI)
            {
                isPressed = false;
                OnRelease?.Invoke(pendingReleasePosition);
            }
            // Reset the flag
            wasInputStartedOverUI = false;
        }


        // --- Hold Processing ---
        if (isPressed && !wasInputStartedOverUI)
        {
            Vector2 currentPosition = Camera.main.ScreenToWorldPoint(playerControls.Player.PrimaryPosition.ReadValue<Vector2>());
            OnHold?.Invoke(currentPosition);
        }
    }

    private void OnPressStarted(InputAction.CallbackContext context)
    {
        // Defer processing to Update by setting a flag and storing the position
        inputStartedThisFrame = true;
        pendingPressPosition = Camera.main.ScreenToWorldPoint(playerControls.Player.PrimaryPosition.ReadValue<Vector2>());
    }

    private void OnPressCanceled(InputAction.CallbackContext context)
    {
        // Defer processing to Update by setting a flag
        inputEndedThisFrame = true;
        pendingReleasePosition = Camera.main.ScreenToWorldPoint(playerControls.Player.PrimaryPosition.ReadValue<Vector2>());
    }

    /// <summary>
    /// Checks if the pointer is over any UI element using a raycast.
    /// This is more reliable than IsPointerOverGameObject when used with the new Input System.
    /// </summary>
    /// <returns>True if the pointer is over a UI element, false otherwise.</returns>
    private bool IsPointerOverUIElement()
    {
        if (EventSystem.current == null)
        {
            return false;
        }

        // Create a PointerEventData object for the current pointer position.
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        // The position must be the raw screen-space position.
        eventData.position = playerControls.Player.PrimaryPosition.ReadValue<Vector2>();

        // Raycast against the UI.
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        // You can uncomment this block to debug or add specific logic for buttons
        /*
        foreach (var hit in results)
        {
            // Check if the hit object has a Button component
            if (hit.gameObject.GetComponent<UnityEngine.UI.Button>() != null)
            {
                Debug.Log("Pointer is over a UI Button!");
                // You could return true here or perform other actions
            }
        }
        */

        // If the list has any results, it means the pointer is over a UI element.
        return results.Count > 0;
    }
}
