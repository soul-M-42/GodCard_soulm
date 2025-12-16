using System;
using System.Collections.Generic;

public static class RandomHelper
{
    // 全局共享 Random（避免每次新建导致种子重复）
    private static readonly Random random = new Random();

    /// <summary>
    /// 从列表中随机抽取 n 个元素（可重复）
    /// </summary>
    public static List<T> GetRandomElementsWithRepeat<T>(List<T> list, int n)
    {
        if (list == null || list.Count == 0)
            throw new ArgumentException("List 不能为空。");

        var result = new List<T>();
        for (int i = 0; i < n; i++)
        {
            int index = random.Next(list.Count);
            result.Add(list[index]);
        }
        return result;
    }

    /// <summary>
    /// 从列表中随机抽取 n 个元素（不重复）
    /// </summary>
    public static List<T> GetRandomElementsNoRepeat<T>(List<T> list, int n)
    {
        if (list == null || list.Count == 0)
            throw new ArgumentException("List 不能为空。");
        if (n > list.Count)
            throw new ArgumentException("抽取数量不能大于列表长度。");

        var tempList = new List<T>(list);
        var result = new List<T>();
        for (int i = 0; i < n; i++)
        {
            int index = random.Next(tempList.Count);
            result.Add(tempList[index]);
            tempList.RemoveAt(index);
        }
        return result;
    }
}
