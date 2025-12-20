using Newtonsoft.Json; 
using System.Collections.Generic;
using System.IO; 
using UnityEditor;
using UnityEngine; 

public class GuidToJsonExporter 
{
    [MenuItem("Tools/Export Filename-GUID Mapping")]
    private static void ExportGuidFilenameMap()
    {
        Dictionary<string, string> guidMap = new Dictionary<string, string>();
        string outputPath = Path.Combine(Application.dataPath, "guid_assetpath_map.json"); 
        // 获取所有资源路径
        string[] allAssets = AssetDatabase.GetAllAssetPaths();
        
        foreach (string assetPath in allAssets)
        {
            // 跳过目录和.meta文件本身
            if (assetPath.EndsWith(".meta"))
                continue;

            // 获取GUID
            string guid = AssetDatabase.AssetPathToGUID(assetPath); 
            if (string.IsNullOrEmpty(guid))
                continue; 
            // 添加到字典
            guidMap[assetPath] = guid;
        }

        // 转换为JSON并保存
        string jsonData = JsonConvert.SerializeObject(guidMap);
        File.WriteAllText(outputPath, jsonData);

        AssetDatabase.Refresh();
        Debug.Log($"GUID-Filename映射已保存至: {outputPath}");
    }

    [MenuItem("Tools/Replace GUID form Mapping")]
    private static void ReplaceGuidformMapping()
    {
        string outputPath = Path.Combine(Application.dataPath, "guid_assetpath_map.json");
        if (!File.Exists(outputPath))
        {
            return;
        }
        string jsonText = File.ReadAllText(outputPath);
        Dictionary<string, string> dict_new = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonText); 
        foreach (var item in dict_new)
        {   
            string assetPath = item.Key;
            string oldGuid = item.Value; 
            string filePath = assetPath + ".meta"; 
            ReplaceMetaFileGuid(filePath, oldGuid); 
        }
        AssetDatabase.Refresh();
    }
    
    private static void ReplaceMetaFileGuid(string filePath, string newGuid)
    {
        if (!File.Exists(filePath))
        {
            return;
        } 
        var lines = File.ReadAllLines(filePath);
        
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].StartsWith("guid:"))
            {   
                lines[i] = $"guid: {newGuid}";
                break;
            }
        }   
        File.WriteAllLines(filePath, lines); 
       
    }

}