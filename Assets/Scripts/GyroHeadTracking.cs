using UnityEngine;
using UnityEngine.InputSystem;

public class GyroHeadTracking : MonoBehaviour
{
    private AttitudeSensor attitudeSensor;
    private Quaternion initialRotation;

    void Awake()
    {
        // 1. Check if the Attitude Sensor exists on the hardware
        if (AttitudeSensor.current != null)
        {
            attitudeSensor = AttitudeSensor.current;
            
            // 2. Explicitly enable the device (Required in the New Input System)
            InputSystem.EnableDevice(attitudeSensor);
        }
        else
        {
            Debug.LogWarning("Attitude Sensor (Gyroscope) not detected on this device.");
        }
    }

    void Start()
    {
        // Cache the camera's initial rotation in the scene
        initialRotation = transform.rotation;
    }

    void Update()
    {
        if (attitudeSensor == null) return;

        if(StartManager.isCardboardMode) return;

        // 3. Read the attitude (quaternion orientation) from the sensor
        Quaternion gyroAttitude = attitudeSensor.attitude.ReadValue();

        // 4. Convert the coordinate system from mobile sensor space to Unity world space
        // This remaps the axes and inverts the handedness for a natural look-around feel
        Quaternion mappedRotation = new Quaternion(-gyroAttitude.x, gyroAttitude.y, gyroAttitude.z, -gyroAttitude.w);
        
        // 5. Rotate the camera relative to its initial orientation
        transform.rotation = initialRotation * Quaternion.Euler(-90f, 0f, 180f) * mappedRotation;
    }

    public void RotateCamera(Quaternion rotation)
    {
        // Apply the provided rotation to the camera
        transform.rotation = initialRotation * rotation;
    }

    void OnDestroy()
    {
        // Clean up: Disable the sensor when the script or scene changes
        if (attitudeSensor != null)
        {
            InputSystem.DisableDevice(attitudeSensor);
        }
    }
}
