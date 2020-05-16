using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestPlayerScript : MonoBehaviour
{
    public void Fire(InputAction.CallbackContext context)
    {
        Debug.Log("Fire!");
        Debug.Log($"triggererd by {context.control.device.displayName}");
    }

    public void Move(InputAction.CallbackContext context)
    {
        var touchInput = context.ReadValue<Vector2>();
        Debug.Log($"move input vec:{touchInput}");
        Debug.Log($"triggererd by {context.control.displayName}");
    }

    public void OpenMenu(InputAction.CallbackContext context)
    {
        Debug.Log("open Menu");
        Debug.Log($"triggererd by {context.control.displayName}");
    }

    public void GripButton(InputAction.CallbackContext context)
    {
        Debug.Log("grip pressed");
        Debug.Log($"triggererd by {context.control.displayName}");
    }
}
