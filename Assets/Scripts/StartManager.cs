using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Management;

public class StartManager : MonoBehaviour
{
    public static bool isCardboardMode = false;
    [SerializeField] private Canvas loadingImage;

    public void Start()
    {
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
}
