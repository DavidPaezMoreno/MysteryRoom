using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Management;

public class StartManager : MonoBehaviour
{
    public static bool isCardboardMode = false;
    [SerializeField] private Canvas loadingImage;
    public static StartManager Instance { get; private set; }

    private Language language = Language.en; // Default to English
    private float TimeLimit = 30f; // Default time limit

    public Language CurrentLanguage
    {
        get { return language; }
        set
        {
            language = value;
            PlayerPrefs.SetInt("SelectedLanguage", (int)language);
            PlayerPrefs.Save();
        }
    }

    public float CurrentTimeLimit
    {
        get { return TimeLimit; }
        set
        {
            TimeLimit = value;
            PlayerPrefs.SetFloat("TimeLimit", TimeLimit);
            PlayerPrefs.Save();
        }
    }

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Load the selected language from PlayerPrefs
        int savedLanguage = PlayerPrefs.GetInt("SelectedLanguage", 0); // Default to English if not set
        language = (Language)savedLanguage;

        // Load the time limit from PlayerPrefs
        TimeLimit = PlayerPrefs.GetFloat("TimeLimit", 30f); // Default to 30 seconds if not set
    }

    public void StartGameWithCardboard()
    {
        isCardboardMode = true;
        StartCoroutine(LoadSceneAsync("MainGameScene"));
    }

    public void StartGameWithoutCardboard()
    {
        isCardboardMode = false;
        StartCoroutine(LoadSceneAsync("MainGameScene"));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // Show loading image
        if (loadingImage != null)
        {
            loadingImage.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Loading image not assigned!", this);
        }

        if(isCardboardMode)
        {
            EnableVR();
        }

        // Start async scene load
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        // Wait until scene is mostly loaded (90%)
        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        // Scene is loaded, allow activation
        asyncLoad.allowSceneActivation = true;

        // Optionally wait for scene to fully activate
        yield return null;

        // Hide loading image when done
        if (loadingImage != null)
        {
            loadingImage.gameObject.SetActive(false);
        }
    }

    public void EnableVR()
    {
        StartCoroutine(StartXRSubsystems());
    }

    private IEnumerator StartXRSubsystems()
    {
        Debug.Log("Initializing Cardboard VR...");
        
        var xrManager = XRGeneralSettings.Instance.Manager;

        // 1. Only initialize if it hasn't been initialized yet
        if (!xrManager.isInitializationComplete)
        {
            yield return xrManager.InitializeLoader();
        }

        // 2. Start the VR subsystems if initialization was successful
        if (xrManager.activeLoader != null)
        {
            xrManager.StartSubsystems();
            Debug.Log("Cardboard VR successfully enabled! Dual-eye screen active.");
        }
        else
        {
            Debug.LogError("Failed to initialize Cardboard XR Loader. Check your project settings.");
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
