using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotateRoom : MonoBehaviour
{
    public Transform player;

	[Tooltip("Ordered list of transforms to move the object through.")]
	public Transform[] waypoints;

	[Tooltip("Minimum trigger value to register a press.")]
	public float triggerThreshold = 0.5f;

	[Tooltip("Wrap around when reaching ends.")]
	public bool loop = true;

	private int currentIndex = 0;
    
    private InputAction actionButton;
    private InputAction backButton;

	private void Awake()
	{
    
        actionButton = new InputAction("ActionButton", InputActionType.Button, "<XRController>{RightHand}/primaryButton");
        actionButton.AddBinding("<Gamepad>/buttonEast");

        backButton = new InputAction("BackButton", InputActionType.Button, "<XRController>{RightHand}/secondaryButton");
        backButton.AddBinding("<Gamepad>/buttonSouth");

        if (waypoints == null || waypoints.Length == 0)
        {
            player.position = waypoints[currentIndex].position;
        }
	}

	private void OnEnable()
	{
		actionButton?.Enable();
		backButton?.Enable();
	}

	private void OnDisable()
	{
		actionButton?.Disable();
        backButton?.Disable();
    }

	private void Update()
	{
		if (waypoints == null || waypoints.Length == 0) return;

        bool actionPressed = actionButton.WasPressedThisFrame();
        bool backPressed = backButton.WasPressedThisFrame();

		// On press (rising edge) and not already moving, move
		if (actionPressed)
		{
			MoveNext();
		}

		if (backPressed)
		{
			MovePrevious();
		}
	}

	private void MoveNext()
	{
		int target = currentIndex + 1;
		if (target >= waypoints.Length)
		{
			if (loop) target = 0; else return;
		}
		MoveToIndex(target);
	}

	private void MovePrevious()
	{
		int target = currentIndex - 1;
		if (target < 0)
		{
			if (loop) target = waypoints.Length - 1; else return;
		}
		MoveToIndex(target);
	}

	private void MoveToIndex(int targetIndex)
	{
		if (waypoints == null || waypoints.Length == 0) return;
		if (targetIndex < 0 || targetIndex >= waypoints.Length) return;

		Vector3 endPos = waypoints[targetIndex].position;
		Quaternion endRot = waypoints[targetIndex].rotation;

		player.position = endPos;
		player.rotation = endRot;
		currentIndex = targetIndex;
	}
}
