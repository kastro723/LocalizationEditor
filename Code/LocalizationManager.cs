using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class LocalizationManager
{
    public static readonly LocalizationManager instance = new LocalizationManager();
    private Dictionary<string, Dictionary<string, string>> localizationData = new Dictionary<string, Dictionary<string, string>>();
    private string language;

    private LocalizationManager()
    {
        Init();
    }

    private void Init()
    {
        language = GetSavedLanguageOrDefault();
        Debug.Log("Current language: " + language);
        LoadLocalizationData();
    }

    private string GetSavedLanguageOrDefault()
    {
        SystemLanguage systemLanguage = Application.systemLanguage;
        string defaultLanguageCode = ConvertSystemLanguageToCode(systemLanguage);
        return PlayerPrefsManager.Instance.GetString("Language", defaultLanguageCode).ToLower();
    }

    private string ConvertSystemLanguageToCode(SystemLanguage language)
    {
        switch (language)
        {
            case SystemLanguage.Korean: return "kr";
            case SystemLanguage.English: return "en";
            case SystemLanguage.Japanese: return "jp";
            default: return "en"; // Default to English if unsupported
        }
    }

    private void LoadLocalizationData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Data/localization_data");
        if (textAsset == null)
        {
            Debug.LogError("Localization data not found.");
            return;
        }

        LocalizationData[] datas = JsonConvert.DeserializeObject<LocalizationData[]>(textAsset.text);
        foreach (var item in datas)
        {
            localizationData[item.id] = new Dictionary<string, string> { { "kr", item.kr }, { "en", item.en }, { "jp", item.jp } };
        }
    }

    public void SetLanguage(string lan)
    {
        string newLanguage = lan.ToLower();
        if (IsSupportedLanguage(newLanguage))
        {
            language = newLanguage;
            PlayerPrefsManager.Instance.SetString("Language", language);
            Debug.Log("Language set to: " + language);
        }
        else
        {
            Debug.LogError("Invalid language input: " + lan);
        }
    }

    private bool IsSupportedLanguage(string languageCode)
    {
        return languageCode == "kr" || languageCode == "en" || languageCode == "jp";
    }

    public string GetLocalizedText(LocalizationKeys key)
    {
        string keyStr = key.ToString().ToLower();
        if (localizationData.ContainsKey(keyStr) && localizationData[keyStr].ContainsKey(language))
        {
            return localizationData[keyStr][language];
        }
        return "Localization key not found";
    }
}