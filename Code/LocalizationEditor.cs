using UnityEditor;
using UnityEngine;
using System.IO;
using ExcelDataReader;
using System.Data;
using System.Text;
using System.Collections.Generic;

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
        GUILayout.Label("Ver. 1.1.0", EditorStyles.boldLabel);

        DrawLine();

        // �巡�� �� ��� ���� ����
        var dragArea = GUILayoutUtility.GetRect(0f, 100f, GUILayout.ExpandWidth(true));
        GUI.Box(dragArea, "Drag & Drop Excel file here or Click 'Generate LocalizationKeys Enum Script'");

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


        if (GUILayout.Button("Generate LocalizationKeys Enum Script") && !string.IsNullOrEmpty(excelFilePath))
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
        classBuilder.AppendLine("using System;");
        classBuilder.AppendLine();
        classBuilder.AppendLine("public enum LocalizationKeys");
        classBuilder.AppendLine("{");
        classBuilder.AppendLine();

        using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                DataTable table = reader.AsDataSet().Tables[0];
                List<string> keys = new List<string>();
                for (int i = 2; i < table.Rows.Count; i++) // ������ �� ���� (3�����)
                {
                    string key = table.Rows[i][0].ToString().ToUpper().Replace(" ", "_");
                    if (!keys.Contains(key)) // enum���� �ߺ� Ű ����
                    {
                        keys.Add(key);
                        classBuilder.AppendLine($"    {key},");
                    }
                }
            }
        }

        classBuilder.AppendLine("}");

        classFilePath = EditorUtility.OpenFolderPanel("Select Folder to Save Class", "Assets", "");
        if (!string.IsNullOrEmpty(classFilePath))
        {
            string targetFilePath = Path.Combine(classFilePath, "LocalizationKeys.cs");

            // ������ �̹� �����ϴ��� Ȯ��
            if (File.Exists(targetFilePath))
            {
                if (EditorUtility.DisplayDialog("File Exists", "The file already exists. Do you want to overwrite it?", "Yes", "No"))
                {
                    // yes�� �����ϸ� �����
                    WriteToFile(targetFilePath, classBuilder.ToString());
                    EditorUtility.DisplayDialog("Success", "Enum file has been successfully overwritten.", "OK");
                }
                // no�� �����ϸ� �ƹ��͵� ���� ����
            }
            else
            {
                // ������ �������� ������ �ٷ� ����
                WriteToFile(targetFilePath, classBuilder.ToString());
                EditorUtility.DisplayDialog("Success", "Enum file has been successfully generated.", "OK");
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
