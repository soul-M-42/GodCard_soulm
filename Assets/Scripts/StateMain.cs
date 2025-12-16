using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.GPUDriven;
using UnityEngine.Rendering;
using UnityEngine.UIElements;




public class GameStateMachine : MonoBehaviour
{
    [Header("Game Parameters")]
    public int initialDrawCards = 4;

    [Header("System References")]
    public GameState currentState;
    public Renderer gameRenderer;
    // public MainMenuState mainMenuState;
    public StartState startState;
    public DrawCardState drawCardState;
    public PlayCardState playCardState;
    public TargetSelectState targetSelectState;
    public BattleState battleState;
    public EndState endState;
    public GameData gameData;
    public InputHandler inputHandler;

    public void Start()
    {
        // 初始化所有状态
        startState = new StartState(this);
        // mainMenuState = new MainMenuState(this);
        drawCardState = new DrawCardState(this);
        playCardState = new PlayCardState(this);
        targetSelectState = new TargetSelectState(this);
        battleState = new BattleState(this);
        endState = new EndState(this);
        gameData = new GameData(this);
        gameRenderer.player1Avatar.player = gameData.Player1;
        gameRenderer.player2Avatar.player = gameData.Player2;
        // Debug.Log(gameData.gameStateMachine);

        ChangeState(startState); // 初始状态：主菜单
    }

    public void Update()
    {
        currentState?.Update();
    }

    public void ChangeState(GameState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
        String newStateName = newState.GetType().Name;
        // Debug.Log($"切换到状态：{newStateName}");
    }
    public void doExitGame()
    {
        // Debug.Log("Game Quit");
        Application.Quit();
    }
    public void doResetGame()
    {
        Start();
        // startState = new StartState(this);
        // ChangeState(startState);
    }
    public void endTurn()
    {
        ChangeState(drawCardState);
    }
}
/// <summary>
/// 主菜单state
/// </summary>
// public class MainMenuState : GameState
// {
//     public MainMenuState(GameStateMachine sm) : base(sm) { }

//     public override void Enter()
//     {
//         Debug.Log("进入主菜单");
//         // 显示主菜单UI
//     }

//     public override void Update()
//     {
//         // 监听按钮点击事件
//         if (Input.GetKeyDown(KeyCode.Space))
//         {
//             stateMachine.ChangeState(stateMachine.startState);
//         }
//     }

//     public override void Exit()
//     {
//         Debug.Log("离开主菜单");
//         // 隐藏主菜单UI
//     }
// }

/// <summary>
/// 新游戏开始-创立场地-双方发牌阶段
/// </summary>
public class StartState : GameState
{
    public StartState(GameStateMachine sm) : base(sm) { }

    public override void Enter()
    {
        GameData gd = stateMachine.gameData;
        /// 发牌，目前为随机
        List<Card> randomCards_1 = CardFactory.GenerateRandomCardsFromLua(stateMachine.initialDrawCards, gd.cardPath, gd);
        foreach (var card in randomCards_1)
        {
            card.owner = gd.Player1;
            gd.Player1.addCardToHand(card);
        }
        List<Card> randomCards_2 = CardFactory.GenerateRandomCardsFromLua(stateMachine.initialDrawCards, gd.cardPath, gd);
        foreach (var card in randomCards_2)
        {
            card.owner = gd.Player2;
            gd.Player2.addCardToHand(card);
        }
        // Debug.Log(gd);
        gd.CurrentPlayer = (UnityEngine.Random.value < 0.5f) ? gd.Player1 : gd.Player2;
        stateMachine.gameRenderer.RenderAll();
    }

    public override void Update()
    {
        // 监听按钮点击事件
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // stateMachine.ChangeState(stateMachine.drawCardState);
            stateMachine.endTurn();
        }
    }

    public override void Exit()
    {
    }
}
/// <summary>
/// 抽牌阶段，之后转到PlayCardState打牌阶段
/// </summary>
public class DrawCardState : GameState
{
    public DrawCardState(GameStateMachine sm) : base(sm) { }

    public override void Enter()
    {
        Debug.Log("抽牌阶段开始");
        GameData gd = stateMachine.gameData;
        stateMachine.gameRenderer.Logger.LogMsg($"{gd.CurrentPlayer.Name}，抽牌!");
        // 执行抽卡逻辑
        List<Card> randomCard = CardFactory.GenerateRandomCardsFromLua(1, gd.cardPath, gd);
        foreach (var card in randomCard)
        {
            card.owner = gd.CurrentPlayer;
            gd.CurrentPlayer.addCardToHand(card);
        }
        gd.CurrentPlayer.MaxMana += 1;
        gd.CurrentPlayer.CurrentMana = stateMachine.gameData.CurrentPlayer.MaxMana;
        stateMachine.gameRenderer.RenderAll();
    }

    public override void Update()
    {
        stateMachine.ChangeState(stateMachine.playCardState);
    }

    public override void Exit()
    {
        Debug.Log("抽牌结束");
    }
}
/// <summary>
/// 打牌/出牌阶段，之后转到下一个player抽牌阶段
/// </summary>
public class PlayCardState : GameState
{
    public PlayCardState(GameStateMachine sm) : base(sm) { }
    InputHandler inputHandler;
    GameData gd;
    PlayerState currentPlayer;
    MouseTarget clickLast, clickNow;
    

    public override void Enter()
    {
        Debug.Log("进入出牌/移动棋子阶段");
        // 允许玩家选择卡牌
        inputHandler = stateMachine.inputHandler;
        gd = stateMachine.gameData;
        currentPlayer = gd.CurrentPlayer;
        currentPlayer.recoverAllSpawnMove();
        stateMachine.gameRenderer.RenderAll();
    }

    public override void Update()
    {
        stateMachine.gameRenderer.interArrowSprite.SetPos("to", Input.mousePosition, true);
        if (Input.GetMouseButtonDown(1))
        {
            // 右键重置
            clickLast = null;
            clickNow = null;
        }

        if (Input.GetMouseButtonDown(0))
        {
            // 第一次点击：只记录 clickLast
            MouseTarget target = inputHandler.CurrentMouseTarget;
            if (target.Spawn != null)
            {
                stateMachine.gameRenderer.interArrowSprite.Activate();
                stateMachine.gameRenderer.interArrowSprite.SetPos("from", target.Spawn.Sprite.transform.position, false);
            }
            if (clickLast == null)
            {
                clickLast = target;
                return;
            }

            // 第二次点击：记录 clickNow
            clickNow = inputHandler.CurrentMouseTarget;
            bool interactionDone = false;

            // 出牌逻辑
            if (clickLast.Card != null && clickNow.Region != null &&
                clickLast.Card.owner == currentPlayer && clickNow.Region.Owner == currentPlayer)
            {
                gd.handToPrepare(clickLast.Card);
                interactionDone = true;
            }

            // 移动 spawn
            else if (clickLast.Spawn != null)
            {
                if(clickNow.Region != null)
                {
                    if (clickNow.Spawn == null)
                    {
                        gd.spawnMove(clickLast.Spawn, clickNow.Region);
                    }
                    else
                    {
                        gd.spawnInteract(clickLast.Spawn, clickNow.Spawn);
                    }
                    interactionDone = true;
                }
                if(clickNow.Player != null)
                {
                    gd.spawnPlayerInteract(clickLast.Spawn, clickNow.Player);
                    interactionDone = true;
                }
            }

            // 若交互成功，重置
            if (interactionDone)
            {
                clickLast = null;
                clickNow = null;
                stateMachine.gameRenderer.interArrowSprite.Deactivate();
            }
            else
            {
                // 不成功：将当前点击作为新的起点（很常见的交互模式）
                clickLast = clickNow;
                clickNow = null;
            }

            stateMachine.gameRenderer.RenderAll();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            stateMachine.endTurn();
        }
    }

    public override void Exit()
    {
        stateMachine.gameRenderer.interArrowSprite.Deactivate();
        GameData gd = stateMachine.gameData;
        gd.CurrentPlayer = (gd.CurrentPlayer == gd.Player1) ? gd.Player2 : gd.Player1;
        
    }
}

public class TargetSelectState : GameState
{
    public TargetSelectState(GameStateMachine sm) : base(sm) { }
    InputHandler inputHandler;
    GameData gd;
    PlayerState currentPlayer;
    MouseTarget clickLast, clickNow;
    public List<Spawn> selectedSpawn;
    public Card onEffectCard;
    public int targetNum;
    public override void Enter()
    {
        Debug.Log("进入目标选择阶段");
        // 允许玩家选择技能目标
        inputHandler = stateMachine.inputHandler;
        gd = stateMachine.gameData;
        currentPlayer = gd.CurrentPlayer;
        selectedSpawn = new List<Spawn>();
    }

    public override void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            clickLast = null;
            clickNow = null;
        }
        if (Input.GetMouseButtonDown(0))
        {
            clickLast = clickNow;
            clickNow = inputHandler.CurrentMouseTarget;
            if (clickNow != null && clickNow.Spawn != null)
            {
                Debug.Log($"Selected {clickNow.Spawn.Name}");
                selectedSpawn.Add(clickNow.Spawn);
            }
            {
                onEffectCard.onPlayEffect(selectedSpawn, gd);
                // stateMachine.ChangeState(stateMachine.playCardState);
                stateMachine.currentState = stateMachine.playCardState;
            }
        }
    }
    public override void Exit()
    {
       
    }
}

public class BattleState : GameState
{
    public BattleState(GameStateMachine sm) : base(sm) { }

    public override void Enter()
    {
        Debug.Log("进入战斗结算阶段");
        // 执行战斗逻辑
    }

    public override void Update()
    {
        // 模拟战斗结束
        if (Input.GetKeyDown(KeyCode.B))
        {
            stateMachine.ChangeState(stateMachine.endState);
        }
    }

    public override void Exit()
    {
        Debug.Log("战斗阶段结束");
    }
}

public class EndState : GameState
{
    public EndState(GameStateMachine sm) : base(sm) { }

    public override void Enter()
    {
        Debug.Log("游戏结束！");
    }

    public override void Update()
    {
        // 按R重新开始
        if (Input.GetKeyDown(KeyCode.R))
        {
            stateMachine.ChangeState(stateMachine.startState);
        }
    }
}

