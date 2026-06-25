using UnityEngine;
using UnityEngine.InputSystem;

public class MoveWithTriggers : MonoBehaviour
{
	public Transform MovingObject;
	public float movementSpeed = 1.0f;

	[Tooltip("Deadzone to ignore small d-pad/hat values.")]
	public float deadzone = 0.1f;

	// Input action for D-pad / hat / primary2DAxis
	private InputAction moveAction;

	private void Awake()
	{
		// Create a 2D Vector action and bind common dpad/hat/primary2DAxis paths
		moveAction = new InputAction("MoveDPad", InputActionType.Value, "<Gamepad>/dpad");
		moveAction.AddBinding("<Joystick>/hat");
		moveAction.AddBinding("<XRController>/{Primary2DAxis}");
	}

	private void OnEnable()
	{
		moveAction?.Enable();
	}

	private void OnDisable()
	{
		moveAction?.Disable();
	}

	private void Update()
	{
		if (MovingObject == null) return;

		Vector2 input = Vector2.zero;
		if (moveAction != null) input = moveAction.ReadValue<Vector2>();

		// also fallback to Gamepad.current.dpad for some platforms
		var gp = Gamepad.current;
		if ((input.magnitude < Mathf.Epsilon) && gp != null)
		{
			input = gp.dpad.ReadValue();
		}

		if (input.magnitude <= deadzone) return;

		// Move: y -> forward/back, x -> right/left (local space)
		Vector3 forward = MovingObject.transform.forward * input.y;
		Vector3 right = MovingObject.transform.right * input.x;

		Vector3 move = (forward + right) * movementSpeed * Time.deltaTime;
		MovingObject.position += move;
	}
}
