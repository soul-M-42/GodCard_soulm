using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardSprite : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Card Data")]
    public Card cardData;

    [Header("UI References")]
    public Text cardNameText;
    public Text atkText;
    public Text healthText;
    public Text costText;
    public Text detailText;
    public Image cardImage;
    public SpriteRenderer deployRingSpriteRender;

    [Header("Settings")]
    public string imageFolderPath;
    public float hoverScale = 1.5f; // 鼠标悬停时放大倍数
    public float scaleSpeed = 8f;   // 缩放动画速度

    // 内部变量
    private Vector3 originalScale;
    private bool isHovered = false;
    private int originalSiblingIndex; // ⭐ 原始层级位置
    private Transform parentTransform; // ⭐ 父级引用

    void Start()
    {
        originalScale = transform.localScale;
        parentTransform = transform.parent;
        renderAll();
    }

    void Update()
    {
        // 平滑缩放动画
        Vector3 targetScale = isHovered ? originalScale * hoverScale : originalScale;
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
    }

    public void renderAll()
    {
        if(cardData.gd.CurrentPlayer == cardData.owner)
        {
            renderValue();
            renderColor();
        }
        else
        {
            renderBack();
        }
    }
    public void renderBack()
    {
        if (cardData == null) return;
        cardNameText.text = "";
        detailText.text = "";
        atkText.text = "";
        healthText.text = "";
        costText.text = "";
        cardImage.sprite = null;
    }

    public void renderValue()
    {
        if (cardData == null) return;

        cardNameText.text = cardData.Name;
        detailText.text = cardData.Detail;
        atkText.text = cardData.Attack.ToString();
        healthText.text = cardData.Health.ToString();
        costText.text = $"{cardData.DeployCost}/{cardData.MoveCost}";
    }

    public void renderImage()
    {
        if (cardData == null || string.IsNullOrEmpty(cardData.Name)) return;

        string imagePath = Path.Combine(imageFolderPath, cardData.Name + ".png");

        if (File.Exists(imagePath))
        {
            byte[] fileData = File.ReadAllBytes(imagePath);
            Texture2D tex = new Texture2D(2, 2);
            if (tex.LoadImage(fileData))
            {
                Sprite sprite = Sprite.Create(
                    tex,
                    new Rect(0, 0, tex.width, tex.height),
                    new Vector2(0.5f, 0.5f)
                );
                cardImage.sprite = sprite;
            }
        }
        else
        {
            Debug.LogWarning($"未找到图片文件: {imagePath}");
        }
    }

    public void renderColor()
    {
        // 可根据需要设置卡牌颜色
        // deployRingSpriteRender.color = cardData.Owner.featureColor;
    }

    // 当鼠标进入卡牌区域时调用
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isHovered) return;
        isHovered = true;

        // 记录原始层级位置
        originalSiblingIndex = transform.GetSiblingIndex();

        // ⭐ 将卡片移到父物体子层级的最上方
        transform.SetAsLastSibling();
    }

    // 当鼠标离开卡牌区域时调用
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isHovered) return;
        isHovered = false;

        // ⭐ 恢复原始层级位置
        transform.SetSiblingIndex(originalSiblingIndex);
    }
}
