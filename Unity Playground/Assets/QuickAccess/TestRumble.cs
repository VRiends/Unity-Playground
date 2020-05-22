using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class TestRumble : MonoBehaviour
{
    UnityEngine.InputSystem.InputDevice inputDevice;
    UnityEngine.XR.InputDevice rightHand;
    public uint hapticChannel = 0;
    [Range(0, 1)]
    public float hapticAmplitude = 0.8f;

    List<UnityEngine.XR.InputDevice> devices = new List<UnityEngine.XR.InputDevice>();

    // Start is called before the first frame update
    void Start()
    {
        InitalizeInputDevices();

    }

    private void InitalizeInputDevices()
    {
        InputDeviceCharacteristics characteristics = InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right;

        List<UnityEngine.XR.InputDevice> rightHandDevices = new List<UnityEngine.XR.InputDevice>();

        InputDevices.GetDevices( devices);
        foreach (var device in devices)
        {
            Debug.Log($"found Device {device.name}");
        }
        if(devices.Count < 1)
        {
            Debug.LogError("no device found");
        }
        else
        {
            InputDevices.GetDevicesWithCharacteristics(characteristics, rightHandDevices);
        }
        /*        rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
                if(!rightHand.isValid)
                {
                    Debug.LogError("no device found for rightHand node");
                }*/
        rightHand = rightHandDevices.Count > 0 ? rightHandDevices[0] : rightHand;
        if (rightHand.isValid)
        {
            Debug.Log($"Initialized rightHand with {rightHand.name}");
        }
        else
        {
            Debug.LogError("initializing right controller failed.");
        }
    }

    public void Update()
    {
        if(devices.Count < 1)
        {
            InitalizeInputDevices();
        }
    }

    public void SendHaptics(InputAction.CallbackContext context)
    {
        //inputDevice.ExecuteCommand<>();

        if (rightHand.TryGetHapticCapabilities(out var capabilities))
        {
            if(capabilities.supportsImpulse)
            {
                if(rightHand.SendHapticImpulse(hapticChannel, hapticAmplitude, 10))
                {
                    Debug.Log($"Sending haptic impulse with channel {hapticChannel} and amplitude {hapticAmplitude}");
                }
                else
                {
                    Debug.LogError("Something went wrong sending the haptic impulse");
                }
            }
            else
            {
                Debug.LogError($"{rightHand.name} does not have haptic Impulse capabilities");
            }
        }
        else
        {
            Debug.LogError($"{rightHand.name} does not have haptic capabilities");
        }
    }
}
