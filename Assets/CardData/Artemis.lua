cardName = "Artemis"
health = 3
attack = 3
moveCost = 2
deployCost = 3
move = 1
type = "God"
skill_type = "Ranged"
skill_text = "God - Ranged\nAgility: When placed, deals 2 damage to each of two random enemy units."

function OnPlay(card, gd) 
    gd:debugLog("called onPlay")
    local opponentPlayer = card.owner.opponent
    local enemySpawns = opponentPlayer.Spawns
    if #enemySpawns > 0 then
        local targets = opponentPlayer:getRandomSpawns(2)
        for _, target in ipairs(targets) do
            target:getHurt(2)
            gd:debugLog(card.Name .. " dealt 2 damage to " .. target.Name)
        end
    end
end

function OnMove(spawn, gd) 
    gd:debugLog("called onMove")
end

function OnInterWithSpawn(spawn_1, spawn_2, gd)
    local spawn_1_name = spawn_1.Name 
    local spawn_2_name = spawn_2.Name
    gd:debugLog("Lua " .. spawn_1_name .. " inter with " .. spawn_2_name)
    spawn_2:getHurt(spawn_1.Attack)
end

function OnInterWithPlayer(spawn, player, gd)
    gd:debugLog("Lua " .. spawn.Name .. " inter with " .. player.Name)
    player:getHurt(attack)
end

function OnDeath(spawn, gd) 
    gd:debugLog("called onDeath")
end
