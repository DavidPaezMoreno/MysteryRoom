using TMPro;
using UnityEngine;

public class TimeLimitManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField timeText;

    private float timeLimit = 30f; // Default time limit



    private void Start()
    {
        // Load the time limit from StartManager
        if (StartManager.Instance != null)
        {
            timeLimit = StartManager.Instance.CurrentTimeLimit;
        }
        timeText.text = timeLimit.ToString();
    }

    public void ModifyTimeLimit(float speed)
    {
        if(timeLimit <= 0 && speed < 0)
        {
            timeLimit = 0;
            timeText.text = Mathf.FloorToInt(timeLimit).ToString();
            return;
        }

        timeLimit += 1f * speed;
        timeText.text = Mathf.FloorToInt(timeLimit).ToString();
    }

    public void SaveTimeLimit()
    {
        StartManager.Instance.CurrentTimeLimit = Mathf.FloorToInt(timeLimit);
    }
    
}
