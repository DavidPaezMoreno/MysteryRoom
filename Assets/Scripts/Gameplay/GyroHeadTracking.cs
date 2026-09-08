using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GyroHeadTracking : MonoBehaviour
{
    private AttitudeSensor attitudeSensor;
    private Quaternion initialRotation;

    private Vector3 paddingRotation = new Vector3(90f, 0f, 0f); // Optional padding rotation for calibration

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
        Quaternion mappedRotation = new Quaternion(gyroAttitude.x, gyroAttitude.y, -gyroAttitude.z, -gyroAttitude.w);
        
        // 5. Rotate the camera relative to its initial orientation
        transform.rotation = initialRotation * Quaternion.Euler(paddingRotation) * mappedRotation;
    }

    public void UpdatePaddingRotationX(float newPadding)
    {
        paddingRotation += new Vector3(newPadding, 0f, 0f);
        UpdateDebugRotationPadding();
    }

    public void UpdatePaddingRotationY(float newPadding)
    {
        paddingRotation += new Vector3(0f, newPadding, 0f);
        UpdateDebugRotationPadding();
    }

    public void UpdatePaddingRotationZ(float newPadding)
    {
        paddingRotation += new Vector3(0f, 0f, newPadding);
        UpdateDebugRotationPadding();
    }

    private void UpdateDebugRotationPadding()
    {
        TMP_Text debugText = GameObject.Find("DebugRotationPaddingText")?.GetComponent<TMP_Text>();
        if (debugText != null)
        {
            debugText.text = $"X={paddingRotation.x:F2}, Y={paddingRotation.y:F2}, Z={paddingRotation.z:F2}";
        }
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
