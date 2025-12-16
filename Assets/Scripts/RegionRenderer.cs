using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(SpriteRenderer))]
public class RegionRenderer : MonoBehaviour
{
    public float transitionTime = 0.3f;

    [Header("Hex Colors")]
    string blueHex = "#75B5B2";
    string redHex = "#D8A6A6";

    SpriteRenderer top;
    SpriteRenderer bottom;
    SpriteRenderer baseRenderer;
    RectTransform rect;

    Coroutine anim;

    Color Blue => HexToColor(blueHex);
    Color Red => HexToColor(redHex);

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        baseRenderer = GetComponent<SpriteRenderer>();

        baseRenderer.enabled = false;

        CreateChildren();
        ApplyInstant(Blue, Blue);
    }

    void CreateChildren()
    {
        top = CreatePart("Top", 0.25f);
        bottom = CreatePart("Bottom", -0.25f);
    }

    SpriteRenderer CreatePart(string name, float yOffsetNormalized)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(transform, false);

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = baseRenderer.sprite;
        sr.sortingLayerID = baseRenderer.sortingLayerID;
        sr.sortingOrder = baseRenderer.sortingOrder;

        Vector2 size = rect.rect.size;
        sr.drawMode = SpriteDrawMode.Sliced;
        sr.size = new Vector2(size.x, size.y / 2f);

        go.transform.localPosition =
            new Vector3(0, size.y * yOffsetNormalized, 0);

        return sr;
    }

    /// <summary>
    /// 0 = 全蓝
    /// 1 = 全红
    /// 2 = 上红下蓝
    /// </summary>
    public void RenderColor(int mode)
    {
        Color topTarget = Blue;
        Color bottomTarget = Blue;

        if (mode == 1)
        {
            topTarget = bottomTarget = Red;
        }
        else if (mode == 2)
        {
            topTarget = Red;
            bottomTarget = Blue;
        }

        if (anim != null) StopCoroutine(anim);
        anim = StartCoroutine(Animate(topTarget, bottomTarget));
    }

    IEnumerator Animate(Color topTarget, Color bottomTarget)
    {
        Color topStart = top.color;
        Color bottomStart = bottom.color;

        float t = 0;
        while (t < transitionTime)
        {
            t += Time.deltaTime;
            float k = t / transitionTime;

            top.color = Color.Lerp(topStart, topTarget, k);
            bottom.color = Color.Lerp(bottomStart, bottomTarget, k);

            yield return null;
        }

        top.color = topTarget;
        bottom.color = bottomTarget;
    }

    void ApplyInstant(Color topColor, Color bottomColor)
    {
        top.color = topColor;
        bottom.color = bottomColor;
    }

    Color HexToColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out var c))
            return c;

        Debug.LogWarning($"Invalid hex color: {hex}");
        return Color.white;
    }
}
