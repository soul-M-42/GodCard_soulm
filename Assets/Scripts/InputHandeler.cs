using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class InputHandler : MonoBehaviour
{
    public GameStateMachine gameStateMachine;
    public GameData gameData;          // 游戏数据
    public Renderer gameRenderer;      // 渲染器，提供各区域RectTransform
    public Canvas canvas;

    // 枚举区域


    // 鼠标指向数据


    // 最新鼠标指向信息，可供其他脚本读取
    public MouseTarget CurrentMouseTarget { get; private set; } = new MouseTarget();
    public GameObject player1Hand;
    public GameObject player2Hand;
    public GameObject player1Prepare;
    public GameObject player2Prepare;
    public GameObject zoneGod;
    public GameObject zoneHuman;
    public GameObject zoneGhost;
    public GameObject player1Avatar;
    public GameObject player2Avatar;
    public Button buttonEndTurn;
    private void Start()
    {
        buttonEndTurn.onClick.AddListener(onEndTurnButtonClicked);
    }
    void onEndTurnButtonClicked()
    {
        gameStateMachine.endTurn();
    }

    void Update()
    {
        gameData = gameStateMachine.gameData;
        UpdateMouseTarget();
    }

    private void UpdateMouseTarget()
    {
        RectTransform player1HandRect = player1Hand.GetComponent<RectTransform>();
        RectTransform player2HandRect = player2Hand.GetComponent<RectTransform>();
        RectTransform player1PrepareRect = player1Prepare.GetComponent<RectTransform>();
        RectTransform player2PrepareRect = player2Prepare.GetComponent<RectTransform>();
        RectTransform zoneGodRect = zoneGod.GetComponent<RectTransform>();
        RectTransform zoneHumanRect = zoneHuman.GetComponent<RectTransform>();
        RectTransform zoneGhostRect = zoneGhost.GetComponent<RectTransform>();
        RectTransform zonePlayer1 = player1Avatar.GetComponent<RectTransform>();
        RectTransform zonePlayer2 = player2Avatar.GetComponent<RectTransform>();

        Vector2 mousePos = Input.mousePosition;
        RectTransform canvasRT = canvas.GetComponent<RectTransform>();
        MouseTarget target = new MouseTarget();

        // 手牌区域
        if (RectTransformUtility.RectangleContainsScreenPoint(player1HandRect, mousePos, Camera.main))
        {
            target.Region = null;
            target.Card = GetCardAtPosition(gameData.Player1.Hand, mousePos);
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(player2HandRect, mousePos, Camera.main))
        {
            target.Region = null;
            target.Card = GetCardAtPosition(gameData.Player2.Hand, mousePos);
        }
        // 准备区
        else if (RectTransformUtility.RectangleContainsScreenPoint(player1PrepareRect, mousePos, Camera.main))
        {
            target.Region = gameData.Field.Player1PrepareZone;
            target.Spawn = GetSpawnAtPosition(gameData.Field.Player1PrepareZone.Spawns, mousePos);
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(player2PrepareRect, mousePos, Camera.main))
        {
            target.Region = gameData.Field.Player2PrepareZone;
            target.Spawn = GetSpawnAtPosition(gameData.Field.Player2PrepareZone.Spawns, mousePos);
        }
        // 战场区域
        else if (RectTransformUtility.RectangleContainsScreenPoint(zoneGodRect, mousePos, Camera.main))
        {
            target.Region = gameData.Field.GodWay;
            target.Spawn = GetSpawnAtPosition(gameData.Field.GodWay.Spawns, mousePos);
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(zoneHumanRect, mousePos, Camera.main))
        {
            target.Region = gameData.Field.HumanWay;
            target.Spawn = GetSpawnAtPosition(gameData.Field.HumanWay.Spawns, mousePos);
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(zoneGhostRect, mousePos, Camera.main))
        {
            target.Region = gameData.Field.GhostWay;
            target.Spawn = GetSpawnAtPosition(gameData.Field.GhostWay.Spawns, mousePos);
        }
        /// 点击主帅
        else if (RectTransformUtility.RectangleContainsScreenPoint(zonePlayer1, mousePos, Camera.main))
        {
            target.Player = gameData.Player1;
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(zonePlayer2, mousePos, Camera.main))
        {
            target.Player = gameData.Player2;
        }

        CurrentMouseTarget = target; // 更新公共属性
    }

    private Card GetCardAtPosition(List<Card> cards, Vector2 mousePos)
    {
        foreach (var card in cards)
        {
            if (card.Sprite == null) continue;
            RectTransform rt = card.Sprite.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(rt, mousePos, Camera.main))
                return card;
        }
        return null;
    }

    private Spawn GetSpawnAtPosition(List<Spawn> spawns, Vector2 mousePos)
    {
        foreach (var s in spawns)
        {
            if (s.Sprite == null) continue;
            RectTransform rt = s.Sprite.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(rt, mousePos, Camera.main))
            {
                return s;
            }
        }
        return null;
    }

}


public class MouseTarget
{
    public Card Card { get; set; }
    public Spawn Spawn { get; set; }
    public Region Region { get; set; }
    public PlayerState Player { get; set; }
    public MouseTarget() { Card = null; Spawn = null; Region = null; }
    public override string ToString()
    {
        string cardStr = Card != null ? Card.ToString() : "None";
        string spawnStr = Spawn != null ? Spawn.ToString() : "None";
        string playerStr = Player != null ? Player.ToString() : "None";
        return $"MouseTarget [Region: {Region}, Card: {cardStr}, Spawn: {spawnStr}, Player: {playerStr}]";
    }
}