using UnityEngine;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
public class Spawn
{
    public bool IsDestroyed = false;
    public GameData gd;
    public string Name { get; set; }
    public int Health { get; set; }
    public int Attack { get; set; }
    public int MoveCost { get; set; }
    public String Type { get; set; }
    public String Race { get; set; }
    public int moveMax { get; set; }
    public int moveLeft { get; set; }
    public GameObject Sprite { get; set; }
    public Region Region { get; set; }
    public PlayerState Owner { get; set; }
    public DynValue LuaOnMove;
    public DynValue LuaOnInterSpawn;
    public DynValue LuaOnInterPlayer;
    public DynValue LuaOnDeath;
    public Script LuaScript;
    public Card cardData;

    public CardSprite cardSprite;

    public Spawn(GameData gameData,
                string name,
                int health,
                int attack,
                int moveCost,
                string type,
                string race,
                int move,
                string luaCode,
                Card card
                )
    {
        gd = gameData;
        UserData.RegisterType<Card>();
        UserData.RegisterType<GameData>();
        UserData.RegisterType<Spawn>();
        UserData.RegisterType<PlayerState>();
        UserData.RegisterType<GameStateMachine>();
        UserData.RegisterType<TargetSelectState>();
        LuaScript = new Script();
        LuaScript.Globals["spawn"] = this;
        LuaScript.DoString(luaCode);
        LuaOnMove = LuaScript.Globals.Get("OnMove");
        LuaOnInterSpawn = LuaScript.Globals.Get("OnInterWithSpawn");
        LuaOnInterPlayer = LuaScript.Globals.Get("OnInterWithPlayer");
        LuaOnDeath = LuaScript.Globals.Get("OnDeath");
        Name = name;
        Health = health;
        Attack = attack;
        MoveCost = moveCost;
        Race = race;
        Type = type;
        moveMax = move;
        moveLeft = moveMax;
        cardData = card;
    }
    public void destroySelf()
    {
        GameObject.Destroy(Sprite);
        IsDestroyed = true;
    }

    public override string ToString()
    {
        return $"{Name}\nHP: {Health}\nATK: {Attack}\nMoveCost\n{MoveCost})";
    }
    public void getHurt(int hurt)
    {
        Health -= hurt;
        if (Health <= 0)
        {
            OnDeath(this, gd);
        }
        gd.gameStateMachine.gameRenderer.RenderAll();
    }
    public void atkPlus(int x)
    {
        Attack += x;
        gd.gameStateMachine.gameRenderer.RenderAll();
    }
    public void onMove(GameData gd)
    {
        LuaScript.Call(LuaOnMove, this, gd);
    }
    public void OnInterSpawn(Spawn sp1, Spawn sp2, GameData gd)
    {
        LuaScript.Call(LuaOnInterSpawn, sp1, sp2, gd);
    }
    public void OnInterPlayer(Spawn sp, PlayerState player, GameData gd)
    {
        LuaScript.Call(LuaOnInterPlayer, sp, player, gd);
    }
    public void OnDeath(Spawn sp, GameData gd)
    {
        IsDestroyed = true;
        Region.RemoveSpawn(this);
        Owner.Spawns.Remove(this);
        LuaScript.Call(LuaOnDeath, sp, gd);
    }
    public bool isMonster()
    {
        return Race.Contains("Monster");
    }
    public bool isGod()
    {
        return Name.Contains("God");
    }
    public bool isHuman()
    {
        return Name.Contains("Human");
    }
    public bool isGhost()
    {
        return Name.Contains("Ghost");
    }
}
public class Card
{
    public GameData gd;
    public string Name { get; set; }
    public string Detail { get; set; }
    public int Health { get; set; }
    public int Attack { get; set; }
    public int MoveCost { get; set; }
    public int DeployCost { get; set; } // 新增：部署费用
    public String Race { get; set; }
    public String Type { get; set; }
    public int Move { get; set; }

    public PlayerState owner;
    public GameObject Sprite { get; set; }
    public DynValue LuaOnPlay;
    public DynValue LuaOnPlayEffect;
    public Script LuaScript;
    public String luaCode;

    // public Card(string name, int health, int attack, int moveCost, int deployCost)
    // {
    //     Name = name;
    //     Health = health;
    //     Attack = attack;
    //     MoveCost = moveCost;
    //     DeployCost = deployCost;
    // }
    public Card(GameData gameStateMachine, string luaPath)
    {
        gd = gameStateMachine;
        UserData.RegisterType<Card>();
        UserData.RegisterType<GameData>();
        UserData.RegisterType<Spawn>();
        UserData.RegisterType<PlayerState>();
        UserData.RegisterType<GameStateMachine>();
        UserData.RegisterType<TargetSelectState>();
        LuaScript = new Script();
        LuaScript.Globals["card"] = this;
        luaCode = System.IO.File.ReadAllText(luaPath);
        LuaScript.DoString(luaCode);
        Name = LuaScript.Globals.Get("cardName").String;
        Detail = LuaScript.Globals.Get("skill_text").String;
        Health = (int)LuaScript.Globals.Get("health").Number;
        Attack = (int)LuaScript.Globals.Get("attack").Number;
        MoveCost = (int)LuaScript.Globals.Get("moveCost").Number;
        DeployCost = (int)LuaScript.Globals.Get("deployCost").Number;
        Race = LuaScript.Globals.Get("type").String;
        Type = LuaScript.Globals.Get("skill_type").String;
        Move = (int)LuaScript.Globals.Get("move").Number;
        LuaOnPlay = LuaScript.Globals.Get("OnPlay");
        LuaOnPlayEffect = LuaScript.Globals.Get("OnPlayEffect");
    }

    public override string ToString()
    {
        return $"{Name}\nHP: {Health}\nATK: {Attack}\nMove: {MoveCost}\nDeploy: {DeployCost}";
    }

    /// <summary>
    /// 将卡牌转换为可部署到战场的 Spawn 实例
    /// </summary>
    public Spawn ToSpawn()
    {
        Spawn newSpawn = new Spawn(gd, Name, Health, Attack, MoveCost, Type, Race, Move, luaCode, this);
        newSpawn.Owner = this.owner;
        return newSpawn;
    }
    public void onPlay(GameData gd)
    {
        if (LuaOnPlay.Type == DataType.Function)
        {
            LuaScript.Call(LuaOnPlay, this, gd);
        }
    }
    public void onPlayEffect(List<Spawn> targets, GameData gd)
    {
        if (LuaOnPlay.Type == DataType.Function)
        {
            LuaScript.Call(LuaOnPlayEffect, targets, this, gd);
        }
    }
}

