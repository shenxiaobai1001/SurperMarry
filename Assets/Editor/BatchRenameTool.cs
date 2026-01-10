using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class BatchRenameTool : EditorWindow
{
    private string folderPath = "";
    private string fileExtension = "";
    private int startNumber = 1;
    private bool includeSubfolders = false;
    private string prefix = "";
    private string suffix = "";

    [MenuItem("Tools/批量重命名工具")]
    public static void ShowWindow()
    {
        GetWindow<BatchRenameTool>("批量重命名");
    }

    private void OnGUI()
    {
        GUILayout.Label("批量重命名工具", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // 文件夹选择
        EditorGUILayout.BeginHorizontal();
        folderPath = EditorGUILayout.TextField("文件夹路径", folderPath);
        if (GUILayout.Button("浏览", GUILayout.Width(60)))
        {
            string path = EditorUtility.OpenFolderPanel("选择文件夹", Application.dataPath, "");
            if (!string.IsNullOrEmpty(path))
            {
                folderPath = path;
            }
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.HelpBox("请选择要处理的文件夹", MessageType.Info);

        // 文件后缀
        fileExtension = EditorGUILayout.TextField("文件后缀", fileExtension);
        EditorGUILayout.HelpBox("例如: .png, .jpg, .prefab, .fbx (不带*)", MessageType.Info);

        // 起始数字
        startNumber = EditorGUILayout.IntField("起始数字", startNumber);

        // 前缀和后缀
        prefix = EditorGUILayout.TextField("前缀", prefix);
        suffix = EditorGUILayout.TextField("后缀", suffix);

        // 是否包含子文件夹
        includeSubfolders = EditorGUILayout.Toggle("包含子文件夹", includeSubfolders);

        EditorGUILayout.Space();

        // 预览和重命名按钮
        if (GUILayout.Button("预览重命名", GUILayout.Height(30)))
        {
            PreviewRename();
        }

        if (GUILayout.Button("执行重命名", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("确认", "确定要执行重命名吗？此操作不可撤销！", "确定", "取消"))
            {
                ExecuteRename();
            }
        }

        // 显示当前选择的文件夹中的文件
        if (!string.IsNullOrEmpty(folderPath) && Directory.Exists(folderPath))
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("文件夹信息", EditorStyles.boldLabel);

            string[] files = GetTargetFiles();
            EditorGUILayout.LabelField($"找到 {files.Length} 个文件", EditorStyles.label);

            if (files.Length > 0)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("前5个文件:");
                for (int i = 0; i < Mathf.Min(files.Length, 5); i++)
                {
                    EditorGUILayout.LabelField(Path.GetFileName(files[i]));
                }
                if (files.Length > 5)
                {
                    EditorGUILayout.LabelField($"... 等 {files.Length} 个文件");
                }
                EditorGUILayout.EndVertical();
            }
        }
    }

    private string[] GetTargetFiles()
    {
        if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
            return new string[0];

        SearchOption searchOption = includeSubfolders ?
            SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

        if (string.IsNullOrEmpty(fileExtension))
        {
            return Directory.GetFiles(folderPath, "*", searchOption);
        }
        else
        {
            return Directory.GetFiles(folderPath, $"*{fileExtension}", searchOption);
        }
    }

    private void PreviewRename()
    {
        if (!Directory.Exists(folderPath))
        {
            EditorUtility.DisplayDialog("错误", "文件夹不存在！", "确定");
            return;
        }

        string[] files = GetTargetFiles();
        if (files.Length == 0)
        {
            EditorUtility.DisplayDialog("提示", "没有找到符合条件的文件！", "确定");
            return;
        }

        // 按照文件名的自然顺序排序
        System.Array.Sort(files, (a, b) =>
        {
            return NaturalCompare(Path.GetFileName(a), Path.GetFileName(b));
        });

        string previewText = $"将重命名 {files.Length} 个文件:\n\n";

        for (int i = 0; i < files.Length; i++)
        {
            string oldName = Path.GetFileName(files[i]);
            string extension = Path.GetExtension(files[i]);
            string newName = $"{prefix}{startNumber + i}{suffix}{extension}";

            previewText += $"{oldName}  →  {newName}\n";
        }

        EditorUtility.DisplayDialog("预览重命名", previewText, "确定");
    }

    private void ExecuteRename()
    {
        if (!Directory.Exists(folderPath))
        {
            EditorUtility.DisplayDialog("错误", "文件夹不存在！", "确定");
            return;
        }

        string[] files = GetTargetFiles();
        if (files.Length == 0)
        {
            EditorUtility.DisplayDialog("提示", "没有找到符合条件的文件！", "确定");
            return;
        }

        // 按照文件名的自然顺序排序
        System.Array.Sort(files, (a, b) =>
        {
            return NaturalCompare(Path.GetFileName(a), Path.GetFileName(b));
        });

        int successCount = 0;
        int failCount = 0;

        for (int i = 0; i < files.Length; i++)
        {
            try
            {
                string oldPath = files[i];
                string directory = Path.GetDirectoryName(oldPath);
                string extension = Path.GetExtension(oldPath);
                string newName = $"{prefix}{startNumber + i}{suffix}{extension}";
                string newPath = Path.Combine(directory, newName);

                // 检查新文件名是否已存在
                if (File.Exists(newPath))
                {
                    // 如果目标文件已存在，先删除
                    File.Delete(newPath);

                    // 如果是Unity资源，还需要删除.meta文件
                    if (File.Exists(newPath + ".meta"))
                    {
                        File.Delete(newPath + ".meta");
                    }
                }

                // 重命名文件
                File.Move(oldPath, newPath);

                // 重命名.meta文件（如果是Unity资源）
                string oldMetaPath = oldPath + ".meta";
                if (File.Exists(oldMetaPath))
                {
                    File.Move(oldMetaPath, newPath + ".meta");
                }

                successCount++;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"重命名文件失败: {files[i]}, 错误: {e.Message}");
                failCount++;
            }
        }

        // 刷新Unity资源数据库
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog("完成",
            $"重命名完成！\n成功: {successCount} 个\n失败: {failCount} 个",
            "确定");
    }

    // 自然排序比较（处理文件名中的数字）
    private int NaturalCompare(string a, string b)
    {
        if (a == null && b == null) return 0;
        if (a == null) return -1;
        if (b == null) return 1;

        int aIndex = 0, bIndex = 0;

        while (aIndex < a.Length && bIndex < b.Length)
        {
            if (char.IsDigit(a[aIndex]) && char.IsDigit(b[bIndex]))
            {
                // 提取数字部分
                string aNum = "";
                while (aIndex < a.Length && char.IsDigit(a[aIndex]))
                {
                    aNum += a[aIndex++];
                }

                string bNum = "";
                while (bIndex < b.Length && char.IsDigit(b[bIndex]))
                {
                    bNum += b[bIndex++];
                }

                int aInt = int.Parse(aNum);
                int bInt = int.Parse(bNum);

                if (aInt != bInt)
                {
                    return aInt.CompareTo(bInt);
                }
            }
            else
            {
                int aChar = char.ToLower(a[aIndex]);
                int bChar = char.ToLower(b[bIndex]);

                if (aChar != bChar)
                {
                    return aChar.CompareTo(bChar);
                }

                aIndex++;
                bIndex++;
            }
        }

        return a.Length.CompareTo(b.Length);
    }
}