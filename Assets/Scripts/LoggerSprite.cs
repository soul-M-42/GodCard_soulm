using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class LoggerSprite : MonoBehaviour
{
    [Header("UI References")]
    public Transform content;        // ScrollView/Viewport/Content
    public ScrollRect scrollRect;    // ScrollView

    [Header("Settings")]
    public bool autoScroll = true;
    public int maxLines = 0;         // 0 = 不限制
    public float lineWidth = 600f;   // 每行最大宽度（像素）
    public float lineSpacing = 4f;   // 行间距

    private readonly List<Text> lines = new List<Text>();

    private VerticalLayoutGroup layoutGroup;

    private void Awake()
    {
        if (!content)
        {
            Debug.LogError("LoggerSprite: Content 未设置！");
            return;
        }

        // 确保 Content 有 VerticalLayoutGroup
        layoutGroup = content.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup == null)
            layoutGroup = content.gameObject.AddComponent<VerticalLayoutGroup>();

        layoutGroup.childAlignment = TextAnchor.UpperLeft;
        layoutGroup.childControlHeight = true;
        layoutGroup.childControlWidth = false; // 用 Text 的宽度
        layoutGroup.childForceExpandHeight = false;
        layoutGroup.childForceExpandWidth = false;
        layoutGroup.spacing = lineSpacing;

        // 确保 Content 有 ContentSizeFitter
        var fitter = content.GetComponent<ContentSizeFitter>();
        if (fitter == null)
            fitter = content.gameObject.AddComponent<ContentSizeFitter>();

        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
    }

    /// <summary>
    /// 普通日志（白色）
    /// </summary>
    public void LogMsg(string text)
    {
        AddLine(text, Color.white);
    }

    /// <summary>
    /// 警告日志（黄色）
    /// </summary>
    public void LogWarning(string text)
    {
        AddLine(text, Color.yellow);
    }

    /// <summary>
    /// 添加一行并指定颜色
    /// </summary>
    private void AddLine(string text, Color color)
    {
        Text t = CreateNewTextLine(text, color);
        lines.Add(t);

        // 限制最大行数
        if (maxLines > 0 && lines.Count > maxLines)
        {
            Destroy(lines[0].gameObject);
            lines.RemoveAt(0);
        }

        RefreshLayout();

        if (autoScroll)
            ScrollToBottom();
    }

    /// <summary>
    /// 创建新的 Text 行
    /// </summary>
    private Text CreateNewTextLine(string text, Color color)
    {
        GameObject go = new GameObject("LogLine", typeof(RectTransform), typeof(Text));
        go.transform.SetParent(content, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(lineWidth, 0); // 宽度固定，高度由 Text 自适应

        Text t = go.GetComponent<Text>();
        t.text = text;
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.fontSize = 18;
        t.color = color;
        t.horizontalOverflow = HorizontalWrapMode.Wrap;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        t.alignment = TextAnchor.UpperLeft;

        // 让 Text 高度根据内容自适应
        t.rectTransform.sizeDelta = new Vector2(lineWidth, t.preferredHeight);

        return t;
    }

    /// <summary>
    /// 清空所有日志
    /// </summary>
    public void Clear()
    {
        foreach (var t in lines)
            Destroy(t.gameObject);

        lines.Clear();
        RefreshLayout();
    }

    /// <summary>
    /// 设置最大显示行数
    /// </summary>
    public void SetMaxLines(int count)
    {
        maxLines = count;

        if (maxLines > 0)
        {
            while (lines.Count > maxLines)
            {
                Destroy(lines[0].gameObject);
                lines.RemoveAt(0);
            }
        }

        RefreshLayout();
    }

    /// <summary>
    /// 滚动到底
    /// </summary>
    public void ScrollToBottom()
    {
        if (scrollRect)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }

    private void RefreshLayout()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(content as RectTransform);
    }
}
