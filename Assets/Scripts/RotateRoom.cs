using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotateRoom : MonoBehaviour
{
    public Transform player;

	[Tooltip("Ordered list of transforms to move the object through.")]
	public Transform[] waypoints;

	[Header("UI")]
	[Tooltip("Text to display the current waypoint index.")]
	public TMPro.TextMeshProUGUI indexText;
	[Tooltip("Text to display the remaining time.")]
	public TMPro.TextMeshProUGUI timeText;
	[Tooltip("Time limit to complete the action.")]
	public float timeLimit = 30f;

	public GameObject timeLimitObstruction;

	[Tooltip("Minimum trigger value to register a press.")]
	public float triggerThreshold = 0.5f;

	[Tooltip("Wrap around when reaching ends.")]
	public bool loop = true;

	private int currentIndex = 0;
	private float remainingTime;
    
    private InputAction actionButton;
    private InputAction backButton;
	public Transform[] waypointsRandomized;

	private void Awake()
	{
    
        actionButton = new InputAction("ActionButton", InputActionType.Button, "<XRController>{RightHand}/primaryButton");
        actionButton.AddBinding("<Gamepad>/buttonEast");

        backButton = new InputAction("BackButton", InputActionType.Button, "<XRController>{RightHand}/secondaryButton");
        backButton.AddBinding("<Gamepad>/buttonSouth");
		
		// Deactivate all waypoints at the start
		foreach (var waypoint in waypoints)
		{
			waypoint.gameObject.SetActive(false);
		}	

		// Randomize the order of waypoints and move to the first one
        if (waypoints == null || waypoints.Length > 0)
        {
			waypointsRandomized = waypoints.ToList().OrderBy(x => UnityEngine.Random.value).ToArray();
            MoveToIndex(0);
        }

		if(timeLimit <= 0)
		{
			timeText.text = "";
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

		if(timeLimit > 0)
		{
			remainingTime -= Time.deltaTime;
			int displaySeconds = Mathf.Max(0, Mathf.FloorToInt(remainingTime));
			int displayCentiseconds = Mathf.Clamp(Mathf.FloorToInt((remainingTime - displaySeconds) * 100f), 0, 99);
			timeText.text = $"{displaySeconds:D2}:{displayCentiseconds:D2}";

			if (remainingTime <= 0)
			{
				timeLimitObstruction.SetActive(true);
			}
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
		//Activate the room 
		waypointsRandomized[currentIndex].gameObject.SetActive(false);
		waypointsRandomized[targetIndex].gameObject.SetActive(true);

		if (waypoints == null || waypoints.Length == 0) return;
		if (targetIndex < 0 || targetIndex >= waypoints.Length) return;

		Vector3 endPos = waypointsRandomized[targetIndex].position;
		Quaternion endRot = waypointsRandomized[targetIndex].rotation;

		player.position = endPos;
		player.rotation = endRot;
		currentIndex = targetIndex;

		//Label the current room index based on the original order of waypoints
		int currentRoomIndex = System.Array.IndexOf(waypoints, waypointsRandomized[currentIndex]);
		indexText.text = $"No. {currentRoomIndex + 1}";

		remainingTime = timeLimit;
		timeLimitObstruction.SetActive(false);
	}
}
