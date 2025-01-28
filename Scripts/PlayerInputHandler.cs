using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    PlayerControls playerControls;

    //public TriggerableWrapper<object> FireTrigger { get; set; } = new TriggerableWrapper<object> { };
    //public TriggerableWrapper<object> ReloadTrigger { get; set; } = new TriggerableWrapper<object> { };

    public void Init()
    {
        playerControls = new PlayerControls();
        playerControls?.Enable();
    }

    private void OnEnable()
    {
        addInputActionCallbacks();
    }

    private void OnDisable()
    {
        removeInputActionCallbacks();
    }

    private void addInputActionCallbacks()
    {
        playerControls.Player.Fire.performed += inputFire;
        playerControls.Player.Reload.performed += inputReload;
    }

    private void removeInputActionCallbacks()
    {
        playerControls.Player.Fire.performed -= inputFire;
        playerControls.Player.Reload.performed -= inputReload;
    }


    private void inputFire(InputAction.CallbackContext context)
    {
        InputTriggerContainer.Instance?.FireTrigger?.OnTrigger?.Invoke(null);
    }

    private void inputReload(InputAction.CallbackContext context)
    {
        InputTriggerContainer.Instance?.ReloadTrigger?.OnTrigger?.Invoke(null);
    }
} 