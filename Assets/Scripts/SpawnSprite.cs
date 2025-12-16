using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;
using TMPro;


public class SpawnSprite : MonoBehaviour
{
    public Spawn spawnData;
    public TextMeshProUGUI spawnNameText;
    public TextMeshProUGUI atkText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI costText;
    public Image spawnImage;
    public SpriteRenderer moveRingSpriteRender;
    public string imageFolderPath;
    // Start is called before the first frame update
    void Start()
    {
        imageFolderPath = spawnData.gd.cardPath + "/texture";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void renderAll()
    {
        renderValue();
        renderImage();
        renderMoveRing();
    }
    public void renderValue()
    {
        spawnNameText.text = spawnData.Name;
        // spawnNameText.color = spawnData.Owner.featureColor;
        // spawnNameText.text = "";
        atkText.text = spawnData.Attack.ToString();
        healthText.text = spawnData.Health.ToString();
        costText.text = spawnData.MoveCost.ToString();
    }
    public void renderImage()
    {
        if (spawnData == null || string.IsNullOrEmpty(spawnData.Name)) return;

        string imagePath = Path.Combine(imageFolderPath, spawnData.Name + ".png");

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
                spawnImage.sprite = sprite;
            }
        }
        else
        {
            Debug.LogWarning($"未找到图片文件: {imagePath}");
        }
    }
    public void renderMoveRing()
    {
        if (spawnData.Owner != spawnData.gd.CurrentPlayer)
        {
            moveRingSpriteRender.color = Color.gray;
        }
        else
        {
            if (spawnData.moveLeft > 0 && spawnData.Owner.CurrentMana >= spawnData.MoveCost)
            {
                moveRingSpriteRender.color = Color.green;
            }
            else
            {
                moveRingSpriteRender.color = Color.red;
            }
        }
    }
}
