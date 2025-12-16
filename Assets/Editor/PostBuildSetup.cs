using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;

public class PostBuildSetup
{
    [PostProcessBuild]
    public static void OnPostBuild(BuildTarget target, string pathToBuiltProject)
    {
        // 获取 _Data 文件夹路径
        string dataFolder = pathToBuiltProject.Replace(".exe", "") + "_Data";
        string cardDataPath = Path.Combine(dataFolder, "CardData");

        if (!Directory.Exists(cardDataPath))
        {
            Directory.CreateDirectory(cardDataPath);
            File.WriteAllText(Path.Combine(cardDataPath, "readme.txt"), "这是 CardData 文件夹。");
        }

        UnityEngine.Debug.Log($"✅ CardData 目录已创建: {cardDataPath}");
    }
}
