using UnityEditor;
using UnityEngine;
using System.IO;
using ExcelDataReader;
using System.Data;
using System.Text;

public class LocalizationEditor : EditorWindow
{
    private string excelFilePath = string.Empty;
    private string classFilePath = string.Empty;

    [MenuItem("Tools/LocalizationEditor")]
    public static void ShowWindow()
    {
        var window = GetWindow(typeof(LocalizationEditor));
        window.maxSize = new Vector2(495, window.maxSize.y);
        window.minSize = new Vector2(495, 300);
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Ver. 1.0.0", EditorStyles.boldLabel);

        DrawLine();

        // �巡�� �� ��� ���� ����
        var dragArea = GUILayoutUtility.GetRect(0f, 100f, GUILayout.ExpandWidth(true));
        GUI.Box(dragArea, "Drag & Drop Excel file here or Click 'Generate Constants Class'");

        if (dragArea.Contains(Event.current.mousePosition) && Event.current.type == EventType.DragUpdated)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
            Event.current.Use();
        }
        else if (dragArea.Contains(Event.current.mousePosition) && Event.current.type == EventType.DragPerform)
        {
            DragAndDrop.AcceptDrag();

            foreach (var dragged_object in DragAndDrop.objectReferences)
            {
                var path = AssetDatabase.GetAssetPath(dragged_object);
                if (!string.IsNullOrEmpty(path) && path.EndsWith(".xlsx"))
                {
                    excelFilePath = path;

                    // ���ο� .xlsx ������ ���õ� �� ���� ��� �ʱ�ȭ
                    classFilePath = "";
                    break;
                }
            }
            Event.current.Use();
        }


        if (GUILayout.Button("Generate Constants Class") && !string.IsNullOrEmpty(excelFilePath))
        {
            GenerateConstantsClass(excelFilePath);
        }

        DrawLine();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Selected Excel File:    ", GUILayout.ExpandWidth(false));
        GUI.enabled = false;
        GUILayout.TextField(excelFilePath, GUILayout.ExpandWidth(true));
        GUI.enabled = true;
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        GUILayout.Label("Cs Save Path:              ", GUILayout.ExpandWidth(false));
        GUI.enabled = false;
        GUILayout.TextField(classFilePath, GUILayout.ExpandWidth(true));
        GUI.enabled = true;
        GUILayout.EndHorizontal();
    }

    private void GenerateConstantsClass(string path)
    {
        StringBuilder classBuilder = new StringBuilder();
        classBuilder.AppendLine("using System.Collections;");
        classBuilder.AppendLine("using System.Collections.Generic;");
        classBuilder.AppendLine("using UnityEngine;");
        classBuilder.AppendLine();
        classBuilder.AppendLine("public class LocalizationConstants");
        classBuilder.AppendLine("{");
        classBuilder.AppendLine();
        classBuilder.AppendLine($"    public static string excelFilePath = \"{excelFilePath}\";");
        classBuilder.AppendLine();

        using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                DataTable table = reader.AsDataSet().Tables[0];
                for (int i = 2; i < table.Rows.Count; i++) // ������ �� ���� (3�����)
                {
                    string key = table.Rows[i][0].ToString().ToUpper();
                    string value = table.Rows[i][0].ToString();
                    classBuilder.AppendLine($"    public const string {key} = \"{value}\";");
                }
            }
        }

        classBuilder.AppendLine("}");

        classFilePath = EditorUtility.OpenFolderPanel("Select Folder to Save Class", "Assets", "");
        if (!string.IsNullOrEmpty(classFilePath))
        {
            string targetFilePath = Path.Combine(classFilePath, "LocalizationConstants.cs");

            // ������ �̹� �����ϴ��� Ȯ��
            if (File.Exists(targetFilePath))
            {
                if (EditorUtility.DisplayDialog("File Exists", "The file already exists. Do you want to overwrite it?", "Yes", "No"))
                {
                    // yes�� �����ϸ� �����
                    WriteToFile(targetFilePath, classBuilder.ToString());
                    EditorUtility.DisplayDialog("Success", "Class file has been successfully overwritten.", "OK"); 
                }
                // no�� �����ϸ� �ƹ��͵� ���� ����
            }
            else
            {
                // ������ �������� ������ �ٷ� ����
                WriteToFile(targetFilePath, classBuilder.ToString());
                EditorUtility.DisplayDialog("Success", "Class file has been successfully generated.", "OK"); 
            }
        }
    }

    private void WriteToFile(string filePath, string content)
    {
        File.WriteAllText(filePath, content);
        AssetDatabase.Refresh();
    }
    private void DrawLine()
    {
        var rect = GUILayoutUtility.GetRect(1, 1, GUILayout.ExpandWidth(true));
        EditorGUI.DrawRect(rect, Color.gray);
    }
}