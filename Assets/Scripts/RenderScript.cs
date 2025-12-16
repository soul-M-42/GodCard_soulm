using System.Collections.Generic;
using UnityEngine;
using UnityEngine.GPUDriven;
using UnityEngine.UI;

public class Renderer : MonoBehaviour
{
    public GameStateMachine gm;
    public GameData gameData;

    public Canvas canvas;
    public GameObject cardPrefab;
    public GameObject spawnPrefab;
    public Text manaText;
    public AvatarSprite player1Avatar;
    public AvatarSprite player2Avatar;
    public LoggerSprite Logger;
    public RegionRenderer GodZoneRenderer;
    public RegionRenderer HumanZoneRenderer;
    public RegionRenderer GhostZoneRenderer;

    // 用于区分不同UI区域的父对象
    private GameObject handParentTop;
    private GameObject handParentBottom;
    private GameObject prepareParent;
    private GameObject battlefieldParent;
    private GameObject backgroundParent;

    private void Start() { }

    public void RenderAll()
    {
        gameData = gm.gameData;

        // RenderBackground();
        RenderHand(gameData.Player1, isTop: false, gameData.CurrentPlayer);
        RenderHand(gameData.Player2, isTop: true, gameData.CurrentPlayer);
        RenderPreparationZones();
        RenderBattlefield();
        RenderMana();
        player1Avatar.renderHealthText();
        player2Avatar.renderHealthText();
    }

    private void RenderBackground()
    {
        if (backgroundParent != null) Destroy(backgroundParent);
        backgroundParent = new GameObject("Background");
        backgroundParent.transform.SetParent(canvas.transform, false);
        Image img = backgroundParent.AddComponent<Image>();
        img.color = Color.gray;
        RectTransform rt = backgroundParent.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    public void RenderMana()
    {
        GameData gd = gm.gameData;
        manaText.text = $"{gd.CurrentPlayer.CurrentMana} / {gd.CurrentPlayer.MaxMana}";
    }

    private void RenderHand(PlayerState player, bool isTop, PlayerState currentPlayer)
    {
        GameObject parent = isTop ? handParentTop : handParentBottom;
        if (parent != null) Destroy(parent);

        parent = new GameObject(isTop ? "HandTop" : "HandBottom");
        parent.transform.SetParent(canvas.transform, false);
        if (isTop) handParentTop = parent; else handParentBottom = parent;

        List<Card> hand = player.Hand;
        float spacing = 80f;
        float startX = -((hand.Count - 1) / 2f) * spacing;
        float y = isTop ? 400f : -400f;

        for (int i = 0; i < hand.Count; i++)
        {
            GameObject go = Instantiate(cardPrefab, parent.transform);
            CardSprite cardSprite = go.GetComponent<CardSprite>();
            cardSprite.cardData = hand[i];
            cardSprite.imageFolderPath = gameData.cardPath + "/texture";
            hand[i].Sprite = go;
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(startX + i * spacing, y);
            cardSprite.renderImage();
            cardSprite.renderAll();
            // Text text = go.GetComponentInChildren<Text>();
            // if (player == currentPlayer)
            // {
            //     text.text = hand[i].ToString();
            // }
            // else
            // {
            //     text.text = "? ? ?";
            // }
        }
    }

    private void RenderPreparationZones()
    {
        if (prepareParent != null) Destroy(prepareParent);
        prepareParent = new GameObject("PreparationZones");
        prepareParent.transform.SetParent(canvas.transform, false);
        RenderSpawnList(gameData.Field.Player1PrepareZone.Spawns, new Vector2(0, -220), "P1 Prepare", prepareParent.transform);
        RenderSpawnList(gameData.Field.Player2PrepareZone.Spawns, new Vector2(0, 220), "P2 Prepare", prepareParent.transform);
    }

    private void RenderBattlefield()
    {
        if (battlefieldParent != null) Destroy(battlefieldParent);
        battlefieldParent = new GameObject("Battlefield");
        battlefieldParent.transform.SetParent(canvas.transform, false);

        float spacingX = 350f;
        RenderSpawnList(gameData.Field.GodWay.Spawns, new Vector2(-spacingX, 0), "God Zone", battlefieldParent.transform);
        RenderSpawnList(gameData.Field.HumanWay.Spawns, new Vector2(0, 0), "Human Zone", battlefieldParent.transform);
        RenderSpawnList(gameData.Field.GhostWay.Spawns, new Vector2(spacingX, 0), "Ghost Zone", battlefieldParent.transform);
        RenderField(gameData.Field.GodWay, GodZoneRenderer);
        RenderField(gameData.Field.HumanWay, HumanZoneRenderer);
        RenderField(gameData.Field.GhostWay, GhostZoneRenderer);
    }

    private void RenderField(Region region, RegionRenderer regionRenderer)
    {
        int tmp = region.OwnerAsInt();
        regionRenderer.RenderColor(region.OwnerAsInt());
    }

    private void RenderSpawnList(List<Spawn> spawns, Vector2 centerPos, string zoneName, Transform parent)
    {
        float spacing = 120f;
        float y = centerPos.y;

        for (int i = 0; i < spawns.Count; i++)
        {
            GameObject go = Instantiate(spawnPrefab, parent);
            RectTransform rt = go.GetComponent<RectTransform>();
            SpawnSprite spawnSprite = go.GetComponent<SpawnSprite>();
            spawnSprite.spawnData = spawns[i];
            spawnSprite.imageFolderPath = gameData.cardPath + "/texture";
            spawns[i].Sprite = go;


            // 计算相对中轴线的偏移，使整个列表关于centerPos对称
            float offset = (i - (spawns.Count - 1) / 2f) * spacing;
            rt.anchoredPosition = new Vector2(centerPos.x + offset, y);

            // Text text = go.GetComponentInChildren<Text>();
            // text.text = spawns[i].ToString();
            spawnSprite.renderAll();
        }

        // 创建标签（在上方居中）
        GameObject label = new GameObject(zoneName);
        label.transform.SetParent(parent, false);

        Text lblText = label.AddComponent<Text>();
        // lblText.text = zoneName;
        lblText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        // lblText.color = Color.black;

        RectTransform lblRt = label.GetComponent<RectTransform>();
        lblRt.sizeDelta = new Vector2(150, 30);
        lblRt.anchoredPosition = new Vector2(centerPos.x, centerPos.y + 50);
    }


}
