using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;

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
	[Tooltip("Text to display the current waypoint index for the camera.")]
	public TMPro.TextMeshProUGUI indexTextCamera;
	[Tooltip("Text to display the remaining time for the camera.")]
	public TMPro.TextMeshProUGUI timeTextCamera;
	[Tooltip("Time limit to complete the action.")]
	public float timeLimit = 30f;

	public GameObject timeLimitObstruction;

	[Tooltip("Minimum trigger value to register a press.")]
	public float triggerThreshold = 0.5f;

	[Tooltip("Wrap around when reaching ends.")]
	public bool loop = true;

	private int currentIndex = 0;
	private float remainingTime;
    
    private InputAction rightButton;
    private InputAction downButton;
    private InputAction upButton;
    private InputAction leftButton;
	public Transform[] waypointsRandomized;

	public UnityEvent onRoomChanged;

	private void Awake()
	{
    
		if(StartManager.Instance != null)
		{
			timeLimit = StartManager.Instance.CurrentTimeLimit;
		}

        rightButton = new InputAction("ActionButton", InputActionType.Button, "<XRController>{RightHand}/primaryButton");
        rightButton.AddBinding("<Gamepad>/buttonEast");

        downButton = new InputAction("BackButton", InputActionType.Button, "<XRController>{RightHand}/secondaryButton");
        downButton.AddBinding("<Gamepad>/buttonSouth");

		upButton = new InputAction("ShowNameButton", InputActionType.Button, "<XRController>{RightHand}/gripButton");
		upButton.AddBinding("<Gamepad>/buttonNorth");

		leftButton = new InputAction("ResetTimerButton", InputActionType.Button, "<XRController>{RightHand}/menuButton");
		leftButton.AddBinding("<Gamepad>/buttonWest");

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

		if(!StartManager.isCardboardMode)
		{
			timeText.gameObject.SetActive(false);
			indexText.gameObject.SetActive(false);
		}
		else
		{
			timeTextCamera.gameObject.SetActive(false);
			indexTextCamera.gameObject.SetActive(false);
		}
	}

	private void OnEnable()
	{
		rightButton?.Enable();
		downButton?.Enable();
		upButton?.Enable();
		leftButton?.Enable();
	}

	private void OnDisable()
	{
		rightButton?.Disable();
        downButton?.Disable();
		upButton?.Disable();
		leftButton?.Disable();
    }

	private void Update()
	{
		if (waypoints == null || waypoints.Length == 0) return;

        bool rightPressed = rightButton.WasPressedThisFrame();
        bool downPressed = downButton.WasPressedThisFrame();
		bool upPressed = upButton.WasPressedThisFrame();
		bool leftPressed = leftButton.WasPressedThisFrame();

		// On press (rising edge) and not already moving, move
		if (rightPressed)
		{
			MoveNext();
		}

		if (leftPressed)
		{
			MovePrevious();
		}

		if (downPressed)
		{
			ResetRoom();
		}

		if (upPressed)
		{
			ShowRoomName();
		}

		if(timeLimit > 0)
		{
			remainingTime -= Time.deltaTime;
			int displaySeconds = Mathf.Max(0, Mathf.FloorToInt(remainingTime));
			int displayCentiseconds = Mathf.Clamp(Mathf.FloorToInt((remainingTime - displaySeconds) * 100f), 0, 99);
			timeText.text = $"{displaySeconds:D2}:{displayCentiseconds:D2}";
			timeTextCamera.text = $"{displaySeconds:D2}:{displayCentiseconds:D2}";

			if (remainingTime <= 0)
			{
				// Time's up, activate the obstruction and deactivate the current waypoint
				timeLimitObstruction.SetActive(true);
				waypointsRandomized[currentIndex].gameObject.SetActive(false);
			}
		}
	}

	public void MoveNext()
	{
		int target = currentIndex + 1;
		if (target >= waypoints.Length)
		{
			if (loop) target = 0; else return;
		}
		MoveToIndex(target);
	}

	public void MovePrevious()
	{
		int target = currentIndex - 1;
		if (target < 0)
		{
			if (loop) target = waypoints.Length - 1; else return;
		}
		MoveToIndex(target);
	}

	public void ResetRoom()
	{
		MoveToIndex(currentIndex);
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
		indexTextCamera.text = $"No. {currentRoomIndex + 1}";

		remainingTime = timeLimit;
		timeLimitObstruction.SetActive(false);
		onRoomChanged?.Invoke();
	}

	public void ShowRoomName()
	{
		int currentRoomIndex = System.Array.IndexOf(waypoints, waypointsRandomized[currentIndex]);
		string roomName = LocalizationSettings.StringDatabase.GetLocalizedString("RoomNamesTable", $"room.name.{currentRoomIndex + 1}");

		if (!string.IsNullOrEmpty(roomName))
		{
			
			indexText.text = $"No. {currentRoomIndex + 1} - {roomName}";
			indexTextCamera.text = $"No. {currentRoomIndex + 1} - {roomName}";
			Debug.Log($"Current Room Name: {roomName}");
		}
		else
		{
			Debug.LogWarning($"Room name not found for index {currentRoomIndex + 1}.", this);
		}
	}
}
