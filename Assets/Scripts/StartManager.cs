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
        DisableVR();   
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

    public void DisableVR()
    {
        StartCoroutine(StopXRSubsystems());
    }

    private IEnumerator StopXRSubsystems()
    {
        var xrManager = XRGeneralSettings.Instance.Manager;

        if (xrManager.isInitializationComplete)
        {
            // 1. Stop the running XR subsystems (stops rendering & tracking)
            xrManager.StopSubsystems();
            yield return null; 

            // 2. Completely de-initialize the XR loader
            xrManager.DeinitializeLoader();
            yield return null;

            Debug.Log("Cardboard VR successfully disabled. App is now in 2D mode.");
        }
    }
}
