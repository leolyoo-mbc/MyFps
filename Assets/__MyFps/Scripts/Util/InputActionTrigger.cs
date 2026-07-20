using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputActionTrigger : MonoBehaviour
{
    public InputActionReference inputAction;
    public UnityEvent onTriggered;

    private void OnEnable()
    {
        if (inputAction == null) return;
        inputAction.action.performed += OnAction;
        inputAction.action.Enable();
    }

    private void OnDisable()
    {
        if (inputAction == null) return;
        inputAction.action.performed -= OnAction;
    }

    private void OnAction(InputAction.CallbackContext context)
    {
        onTriggered.Invoke();
    }
}
