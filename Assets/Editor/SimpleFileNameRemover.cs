using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SimpleFileNameRemover : EditorWindow
{
    private string folderPath = "";
    private string removeText = "";
    private Vector2 scrollPos;

    [MenuItem("Tools/简单文件名清理")]
    public static void ShowWindow()
    {
        GetWindow<SimpleFileNameRemover>("文件名清理");
    }

    private void OnGUI()
    {
        GUILayout.Label("简单文件名清理工具", EditorStyles.boldLabel);
        EditorGUILayout.Space(10);

        // 1. 选择文件夹
        EditorGUILayout.LabelField("选择包含MP4的文件夹:");
        EditorGUILayout.BeginHorizontal();
        folderPath = EditorGUILayout.TextField(folderPath);
        if (GUILayout.Button("选择", GUILayout.Width(60)))
        {
            string path = EditorUtility.OpenFolderPanel("选择文件夹", Application.dataPath, "");
            if (!string.IsNullOrEmpty(path))
            {
                if (path.StartsWith(Application.dataPath))
                {
                    folderPath = "Assets" + path.Substring(Application.dataPath.Length);
                }
                else
                {
                    folderPath = path;
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        // 2. 输入要删除的汉字
        EditorGUILayout.Space(5);
        removeText = EditorGUILayout.TextField("删除的汉字:", removeText);

        // 3. 执行按钮
        EditorGUILayout.Space(10);

        if (GUILayout.Button("开始清理", GUILayout.Height(30)))
        {
            if (string.IsNullOrEmpty(folderPath) || string.IsNullOrEmpty(removeText))
            {
                EditorUtility.DisplayDialog("错误", "请先选择文件夹并输入汉字", "确定");
                return;
            }

            if (EditorUtility.DisplayDialog("确认", $"将从文件夹中的所有MP4文件名删除: {removeText}", "确定", "取消"))
            {
                ProcessFiles();
            }
        }

        // 显示文件夹状态
        if (Directory.Exists(folderPath))
        {
            EditorGUILayout.HelpBox($"找到文件夹: {folderPath}", MessageType.Info);
        }
        else if (!string.IsNullOrEmpty(folderPath))
        {
            EditorGUILayout.HelpBox("文件夹不存在", MessageType.Warning);
        }
    }

    private void ProcessFiles()
    {
        // 获取所有mp4文件
        string[] mp4Files = Directory.GetFiles(folderPath, "*.mp4", SearchOption.AllDirectories);

        if (mp4Files.Length == 0)
        {
            EditorUtility.DisplayDialog("提示", "没有找到MP4文件", "确定");
            return;
        }

        int renamedCount = 0;

        foreach (string filePath in mp4Files)
        {
            string dir = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string ext = Path.GetExtension(filePath);

            // 删除所有指定的汉字
            string newName = fileName;
            foreach (char c in removeText)
            {
                newName = newName.Replace(c.ToString(), "");
            }

            // 如果文件名没变，跳过
            if (newName == fileName) continue;

            // 避免空文件名
            if (string.IsNullOrEmpty(newName))
            {
                newName = "renamed_video";
            }

            // 构建新路径
            string newPath = Path.Combine(dir, newName + ext);

            // 避免重复
            int counter = 1;
            while (File.Exists(newPath))
            {
                newPath = Path.Combine(dir, $"{newName}_{counter}{ext}");
                counter++;
            }

            try
            {
                File.Move(filePath, newPath);
                renamedCount++;
                Debug.Log($"重命名: {fileName} -> {Path.GetFileName(newPath)}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"重命名失败 {filePath}: {e.Message}");
            }
        }

        // 刷新资源数据库
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("完成", $"处理完成！重命名了 {renamedCount} 个文件", "确定");
    }
}