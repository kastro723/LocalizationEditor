using System.Collections.Generic;
using UnityEngine;
using System.IO;
using ExcelDataReader;
using System.Data;


public class LocalizationManager : ILocalization
{
    public static readonly LocalizationManager instance = new LocalizationManager();

    private string language;
    private Dictionary<string, string> dicLocalizationData;

    private LocalizationManager()
    {
        dicLocalizationData = new Dictionary<string, string>();
        Init();
    }

    public void Init() // �ʱ� ����
    {
        // ����� �⺻ ��� ������ ������
        SystemLanguage systemLanguage = Application.systemLanguage;
        string defaultLanguage = ConvertSystemLanguageToCode(systemLanguage); // ����� �ý��� �� ��� �ڵ�� ��ȯ

        language = PlayerPrefsManager.Instance.GetString("Language", defaultLanguage); // ����ڰ� ������ �� ���ٸ� ����� �⺻ �� ���
        PlayerPrefsManager.Instance.SetString("Language", language); // ���� ��� ������ ����

        Debug.Log("current language: " + language);

        LoadLocalizationData();
    }


    private string ConvertSystemLanguageToCode(SystemLanguage language)
    {
        switch (language)
        {
            case SystemLanguage.Korean:
                return "kr";
            case SystemLanguage.English:
                return "en";
            case SystemLanguage.Japanese:
                return "jp";
            default:
                return "kr"; // �⺻������ �ѱ��� ����
        }
    }

    private void LoadLocalizationData()
    {
        string filePath = LocalizationConstants.excelFilePath; // LocalizationConstants.cs���� ���ǵ� ��θ� ���

        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var result = reader.AsDataSet();
                DataTable table = result.Tables[0];

                // ��� �ڵ�� 1�� 2�� ���� ��ġ��.
                var languageCodes = new List<string>();
                for (int columnIndex = 1; columnIndex < table.Columns.Count; columnIndex++)
                {
                    languageCodes.Add(table.Rows[0][columnIndex].ToString()); // 1�� 2������ ��� �ڵ带 ����
                }

                // �����ʹ� 3����� �����մϴ�.
                for (int rowIndex = 2; rowIndex < table.Rows.Count; rowIndex++)
                {
                    var row = table.Rows[rowIndex];
                    string key = row[0].ToString(); // ù ��° ���� Ű ���Դϴ�.

                    for (int columnIndex = 1; columnIndex < languageCodes.Count + 1; columnIndex++)
                    {
                        string languageCode = languageCodes[columnIndex - 1]; // ��� �ڵ� ����Ʈ���� ��� �ڵ带 ������
                        string value = row[columnIndex].ToString();
                        string dicKey = $"{key}_{languageCode}".ToUpper(); // ���� Ű ���� (��: TROPHY_POPUP_TITLE_FOR_UNLOCK_KR)

                        dicLocalizationData[dicKey] = value; // ��ųʸ��� �� �� ����
                    }
                }
            }
        }
    }

    public void SetLanguage(string lan) // ��� ����
    {
        language = lan;
        language = language.ToLower(); // ��, �ҹ��ڸ� �ҹ��ڷ� ����
        if (language == "kr" || language == "en" || language == "jp") // ��� �߰� ��, ���� �߰�
        {
            PlayerPrefsManager.Instance.SetString("Language", language); // ���� ��� ������ ����, (kr, en, jp)
            Debug.Log("current language: " + language);

            dicLocalizationData.Clear();
            LoadLocalizationData(); 
        }

        else
        {
            Debug.Log("Language Invaild Input!");
        }
       
    }

    /// <summary>
    /// ���� ���ö������ ���ڿ��� ��ȯ.
    /// </summary>
    /// <param name="key">���ö������̼� Ű (��: "TROPHY_POPUP_TITLE_FOR_UNLOCK")</param>
    /// <returns>
    /// ���� ������ �� �´� ���ö������ ���ڿ�.
    /// ������ �� �ش��ϴ� ���ڿ��� ���� ��� "Key {key} not found" ��ȯ.
    /// </returns>
    /// <example>
    /// // ���� ����:
    /// string localizedText = LocalizationManager.instance.GetLocalString("TROPHY_POPUP_TITLE_FOR_UNLOCK");
    /// Debug.Log(localizedText); // ���� �� "kr"�̶�� "�������!"�� ���
    /// </example>

    public string GetLocalString(string key)
    {
        string dicKey = $"{key}_{language}".ToUpper();
        if (dicLocalizationData.TryGetValue(dicKey, out string value))
        {
            return value;
        }
        return $"Key {key} not found";
    }


}
