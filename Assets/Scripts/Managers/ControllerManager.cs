using UnityEngine;
using UnityEngine.InputSystem;

public class ControllerManager : MonoBehaviour
{
    [SerializeField] private GameObject controllerUI;

    private void Start() {
        bool isGamepadConnected = Gamepad.current != null;
        controllerUI.SetActive(isGamepadConnected);
    }

    void OnEnable()
    {
        InputSystem.onDeviceChange += OnDeviceChange;
    }

    void OnDisable()
    {
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    private void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device is Gamepad)
        {
            switch (change)
            {
                case InputDeviceChange.Added:
                    controllerUI.SetActive(true);
                    break;
                case InputDeviceChange.Removed:
                    controllerUI.SetActive(false);
                    break;
            }
        }
    }
}
