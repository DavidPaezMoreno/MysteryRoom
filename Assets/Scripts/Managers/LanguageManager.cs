using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LanguageManager : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown languageDropdown;

    private readonly string[] localeCodes =
    {
        "en",
        "es"
    };

    public void ChangeLanguage(int index)
    {
        if (index < 0 || index >= localeCodes.Length)
            return;

        var locale = LocalizationSettings
            .AvailableLocales
            .GetLocale(localeCodes[index]);

        if (locale != null)
            LocalizationSettings.SelectedLocale = locale;
    }

    private void Start()
    {
        int savedLanguage = StartManager.Instance.CurrentLanguage == Language.en ? 0 : 1;

        ChangeLanguage(savedLanguage);

        languageDropdown.value = savedLanguage;
    }

    public void OnLanguageSelected()
    {
        Language language = (Language)languageDropdown.value;

        StartManager.Instance.CurrentLanguage = language;

        ChangeLanguage((int)language);
        languageDropdown.value = (int)language;

        // Optionally, you can also reload the scene or update UI elements to reflect the new language
        Debug.Log($"Language set to: {(int)language}");
    }

    public void SetCurrentLanguage()
    {
        int savedLanguage = StartManager.Instance.CurrentLanguage == Language.en ? 0 : 1;
        languageDropdown.value = savedLanguage;
    }
}
