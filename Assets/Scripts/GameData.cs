using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.GPUDriven;
using System.IO;

/// <summary>
/// 表示整个卡牌游戏的对局状态
/// </summary>
public class GameData
{
    public GameStateMachine gameStateMachine;
    public string cardPath;
    public PlayerState Player1 { get; set; }
    public PlayerState Player2 { get; set; }
    public Battlefield Field { get; set; }
    public PlayerState CurrentPlayer{ get; set; }

    public GameData(GameStateMachine gsm)
    {
        gameStateMachine = gsm;
        cardPath = Application.dataPath + "/CardData"; // Lua 卡牌目录
        Player1 = new PlayerState("Player1");
        Player2 = new PlayerState("Player2");
        Field = new Battlefield(Player1, Player2);
        Player1.prepareRegion = Field.Player1PrepareZone;
        Player2.prepareRegion = Field.Player2PrepareZone;
        Player1.opponent = Player2;
        Player2.opponent = Player1;
        Color tmpColor_1 = new Color(0.6f, 1f, 1f);
        Color tmpColor_2 = new Color(1f, 0.6f, 0.6f);
        Player1.featureColor = tmpColor_1;
        Player2.featureColor = tmpColor_2;
    }
    public override string ToString()
    {
        return
            "=== GameData ===\n" +
            $"Player1:\n{Player1}\n\n" +
            $"Player2:\n{Player2}\n\n" +
            $"Battlefield:\n{Field}\n" +
            "================";
    }
    public void handToPrepare(Card card)
    {
        if (card.owner.CurrentMana >= card.DeployCost)
        {
            Spawn newSpawn = card.ToSpawn();
            card.owner.prepareRegion.AddSpawn(newSpawn);
            newSpawn.Owner.Spawns.Add(newSpawn);
            newSpawn.moveLeft -= 1;
            card.owner.Hand.Remove(card);
            card.owner.CurrentMana -= card.DeployCost;
            card.onPlay(this);
        }
    }
    public void spawnMove(Spawn spawn, Region targetRegion)
    {
        // Debug.Log($"{spawn.Name} move to {region.Name}");
        if (spawn.Region == targetRegion)
        {
            return;
        }
        if (spawn.Owner != CurrentPlayer)
        {
            return;
        }
        if (spawn.Owner.CurrentMana < spawn.MoveCost)
        {
            return;
        }
        if (spawn.moveLeft <= 0)
        {
            return;
        }
        if (regionDis(spawn.Region, targetRegion) != 1)
        {
            return;
        }
        if (targetRegion.Name.Contains("Prepare"))
        {
            return;
        }
        if (targetRegion.Spawns.Count >= 2)
        {
            return;
        }
        if (targetRegion.Spawns.Count == 1 && targetRegion.Spawns[0].Owner == spawn.Owner)
        {
            return;
        }
        if(spawn.Region.Name.Contains("Way") && targetRegion.Name.Contains("Way"))
        {
            return;
        }
        if(spawn.isGod() && targetRegion != Field.GodWay)
        {
            return;
        }
        if(spawn.isHuman() && targetRegion != Field.HumanWay)
        {
            return;
        }
        if(spawn.isGhost() && targetRegion != Field.GhostWay)
        {
            return;
        }
        spawn.moveLeft -= 1;
        spawn.onMove(spawn.gd);
        spawn.Region.RemoveSpawn(spawn);
        targetRegion.AddSpawn(spawn);
        spawn.Owner.CurrentMana -= spawn.MoveCost;
    }
    public void spawnInteract(Spawn spawn1, Spawn spawn2)
    {
        if (spawn1.Owner != CurrentPlayer)
        {
            return;
        }
        if (spawn1.Owner.CurrentMana < spawn1.MoveCost)
        {
            return;
        }
        if (spawn1.moveLeft <= 0)
        {
            return;
        }
        if (spawn1.Type == "Melee")
        {
            if (regionDis(spawn1.Region, spawn2.Region) > 1)
            /// 手短
            {
                return;
            }
            if (spawn1.Region.isPrepare() && spawn1.Owner != spawn2.Owner && spawn2.Region.Spawns.Count > 1)
            ///被道上同仁挡住
            {
                return;
            }
        }
        spawn1.moveLeft -= 1;
        spawn1.OnInterSpawn(spawn1, spawn2, this);
        spawn1.Owner.CurrentMana -= spawn1.MoveCost;
    }
    public void spawnPlayerInteract(Spawn spawn, PlayerState player)
    {
        Debug.Log($"{spawn.Name} inter with {player.Name}");
        gameStateMachine.gameRenderer.Logger.LogMsg($"{spawn.Name} inter with {player.Name}");
        if (
            spawn.Type == "Ranged" ||
            (spawn.Type == "Melee" &&
            spawn.Region.isWay() &&
            spawn.Region.Spawns.Count == 1 &&
            spawn.Owner.opponent.prepareRegion.Spawns.Count == 0)
            )
        {
            spawn.moveLeft -= 1;
            spawn.OnInterPlayer(spawn, player, this);
            spawn.Owner.CurrentMana -= spawn.MoveCost;
        }
    }
    public int regionDis(Region r1, Region r2)
    {
        if (r1 == r2)
        {
            return 0;
        }
        if (r1.Name.Contains("Prepare") && r2.Name.Contains("Prepare"))
        {
            return 2;
        }
        return 1;
    }
    public void debugLog(String msg)
    {
        Debug.Log(msg);
        gameStateMachine.gameRenderer.Logger.LogMsg(msg);
    }
}

/// <summary>
/// 表示单个玩家的状态：血量,费用,手牌
/// </summary>
public class PlayerState
{
    public string Name { get; set; }
    public int Health { get; set; }
    public int CurrentMana { get; set; }
    public int MaxMana { get; set; }
    public Region prepareRegion{ get; set; }
    public Color featureColor { get; set; }
    public PlayerState opponent{ get; set; }

    /// <summary>
    /// 手牌
    /// </summary>
    public List<Card> Hand { get; set; }
    public List<Spawn> Spawns { get; set; }

    public PlayerState(string name, int health = 20, int maxMana = 3)
    {
        Name = name;
        Health = health;
        CurrentMana = maxMana;
        MaxMana = maxMana;
        Hand = new List<Card>();
        Spawns = new List<Spawn>();
    }
    public void getHurt(int hurt)
    {
        Health -= hurt;
    }
    public void addCardToHand(Card card)
    {
        Hand.Add(card);
    }
    public void recoverAllSpawnMove()
    {
        foreach (var s in Spawns)
        {
            s.moveLeft = s.moveMax;
        }
    }
    public List<Spawn> getRandomSpawns(int k)
    {
        return RandomHelper.GetRandomElementsWithRepeat(Spawns, k);
    }
    public override string ToString()
    {
        string handInfo = Hand.Count > 0
            ? string.Join("\n  ", Hand)
            : "（空手牌）";

        return
            $"玩家: {Name}\n" +
            $"血量: {Health}\n" +
            $"法力: {CurrentMana}/{MaxMana}\n" +
            $"手牌({Hand.Count}):\n  {handInfo}";
    }

}

/// <summary>
/// 战场信息，包含5个区域
/// </summary>
public class Battlefield
{
    public Region Player1PrepareZone { get; set; }
    public Region Player2PrepareZone { get; set; }
    public Region GodWay { get; set; }
    public Region HumanWay { get; set; }
    public Region GhostWay { get; set; }
    // public GameObject Sprite { get; set; }
    // public PlayerState owner { get; set; }

    public Battlefield(PlayerState Player1, PlayerState Player2)
    {
        Player1PrepareZone = new Region("Player1PrepareZone", Player1);
        Player2PrepareZone = new Region("Player2PrepareZone", Player2);
        GodWay = new Region("GodWay", null);
        HumanWay = new Region("HumanWay", null);
        GhostWay = new Region("GhostWay", null);
    }
}


public static class CardFactory
{
    private static readonly System.Random random = new System.Random();

    /// <summary>
    /// 从指定路径加载 Lua 卡牌文件，并随机生成 k 张 Card 实例
    /// </summary>
    /// <param name="k">要生成的卡牌数量</param>
    /// <param name="cardDataPath">Lua 卡牌文件夹路径</param>
    public static List<Card> GenerateRandomCardsFromLua(int k, string cardDataPath, GameData gd)
    {
        var cards = new List<Card>();

        if (!Directory.Exists(cardDataPath))
        {
            Debug.LogError($"Card data path does not exist: {cardDataPath}");
            return cards;
        }

        // 获取所有 Lua 文件
        string[] luaFiles = Directory.GetFiles(cardDataPath, "*.lua");
        if (luaFiles.Length == 0)
        {
            Debug.LogWarning($"No Lua card files found in {cardDataPath}");
            return cards;
        }

        // 随机抽取 k 个 Lua 文件
        for (int i = 0; i < k; i++)
        {
            string luaFile = luaFiles[random.Next(luaFiles.Length)];

            try
            {
                Card card = new Card(gd, luaFile); // 调用你之前写的 Lua 初始化构造函数
                cards.Add(card);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Failed to load card from {luaFile}: {ex.Message}");
            }
        }

        return cards;
    }
}
