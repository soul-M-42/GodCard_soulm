cardName = "Athena"
health = 6
attack = 4
moveCost = 3
deployCost = 6
move = 1
type = "God"
skill_type = "Melee"
skill_text = "God - Melee\nJustice from Above: Instantly deals 5 damage to a designated enemy unit."

function OnPlay(card, gd)
    gd:debugLog(card.Name .. " used Justice from Above skill")
    local opponentPlayer = card.owner.opponent
    local enemySpawns = opponentPlayer.Spawns
    if #enemySpawns > 0 then
        gd.gameStateMachine.currentState = gd.gameStateMachine.targetSelectState
        gd.gameStateMachine.currentState:Enter()
        gd.gameStateMachine.targetSelectState.targetNum = 1
        gd.gameStateMachine.targetSelectState.onEffectCard = card
    else
        gd:debugLog("No enemy units available for Justice from Above.")
    end
end

function OnPlayEffect(spawns, card, gd)
    for _, enemy in ipairs(spawns) do
        enemy.getHurt(5)
    end
end

function OnMove(spawn, gd)
    gd:debugLog("called onMove")
end

function OnInterWithSpawn(spawn_1, spawn_2, gd)
    local spawn_1_name = spawn_1.Name
    local spawn_2_name = spawn_2.Name
    gd:debugLog("Lua " .. spawn_1_name .. " inter with " .. spawn_2_name)
    spawn_1:getHurt(spawn_2.Attack)
    spawn_2:getHurt(spawn_1.Attack)
end

function OnInterWithPlayer(spawn, player, gd)
    gd:debugLog("Lua " .. spawn.Name .. " inter with " .. player.Name)
    player:getHurt(spawn.Attack)
end

function OnDeath(spawn, gd)
    gd:debugLog("called onDeath")
end
