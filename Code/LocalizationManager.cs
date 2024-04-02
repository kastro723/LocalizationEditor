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

    public void Init() // 초기 설정
    {
        // 기기의 기본 언어 설정을 가져옴
        SystemLanguage systemLanguage = Application.systemLanguage;
        string defaultLanguage = ConvertSystemLanguageToCode(systemLanguage); // 기기의 시스템 언어를 언어 코드로 변환

        language = PlayerPrefsManager.Instance.GetString("Language", defaultLanguage); // 사용자가 설정한 언어가 없다면 기기의 기본 언어를 사용
        PlayerPrefsManager.Instance.SetString("Language", language); // 현재 언어 설정을 저장

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
                return "kr"; // 기본값으로 한국어 설정
        }
    }

    private void LoadLocalizationData()
    {
        string filePath = LocalizationConstants.excelFilePath; // LocalizationConstants.cs에서 정의된 경로를 사용

        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var result = reader.AsDataSet();
                DataTable table = result.Tables[0];

                // 언어 코드는 1행 2열 부터 위치함.
                var languageCodes = new List<string>();
                for (int columnIndex = 1; columnIndex < table.Columns.Count; columnIndex++)
                {
                    languageCodes.Add(table.Rows[0][columnIndex].ToString()); // 1행 2열부터 언어 코드를 읽음
                }

                // 데이터는 3행부터 시작합니다.
                for (int rowIndex = 2; rowIndex < table.Rows.Count; rowIndex++)
                {
                    var row = table.Rows[rowIndex];
                    string key = row[0].ToString(); // 첫 번째 열은 키 값입니다.

                    for (int columnIndex = 1; columnIndex < languageCodes.Count + 1; columnIndex++)
                    {
                        string languageCode = languageCodes[columnIndex - 1]; // 언어 코드 리스트에서 언어 코드를 가져옴
                        string value = row[columnIndex].ToString();
                        string dicKey = $"{key}_{languageCode}".ToUpper(); // 복합 키 생성 (예: TROPHY_POPUP_TITLE_FOR_UNLOCK_KR)

                        dicLocalizationData[dicKey] = value; // 딕셔너리에 언어별 값 저장
                    }
                }
            }
        }
    }

    public void SetLanguage(string lan) // 언어 변경
    {
        language = lan;
        language = language.ToLower(); // 대, 소문자를 소문자로 통합
        if (language == "kr" || language == "en" || language == "jp") // 언어 추가 시, 조건 추가
        {
            PlayerPrefsManager.Instance.SetString("Language", language); // 현재 언어 설정을 저장, (kr, en, jp)
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
    /// 언어별로 로컬라이즈된 문자열을 반환.
    /// </summary>
    /// <param name="key">로컬라이제이션 키 (예: "TROPHY_POPUP_TITLE_FOR_UNLOCK")</param>
    /// <returns>
    /// 현재 설정된 언어에 맞는 로컬라이즈된 문자열.
    /// 설정된 언어에 해당하는 문자열이 없을 경우 "Key {key} not found" 반환.
    /// </returns>
    /// <example>
    /// // 사용법 예시:
    /// string localizedText = LocalizationManager.instance.GetLocalString("TROPHY_POPUP_TITLE_FOR_UNLOCK");
    /// Debug.Log(localizedText); // 현재 언어가 "kr"이라면 "잠금해제!"를 출력
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
