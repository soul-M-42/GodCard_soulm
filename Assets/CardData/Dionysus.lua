cardName = "Dionysus"
health = 5
attack = 2
moveCost = 1
deployCost = 4
move = 1
type = "God"
skill_type = "Melee"
skill_text = "God - Melee\nFeast: Grants all allied units +1 health."

function OnPlay(card, gd)
    gd:debugLog(card.Name .. " used Feast skill")
    local ownerPlayer = card.owner
    local allySpawns = ownerPlayer.Spawns
    for _, ally in ipairs(allySpawns) do
        ally.Health = ally.Health + 1
        gd:debugLog(card.Name .. " granted +1 HP to " .. ally.Name)
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
    spawn_1:getHurt(spawn_2.Attack)
end

function OnInterWithPlayer(spawn, player, gd)
    gd:debugLog("Lua " .. spawn.Name .. " inter with " .. player.Name)
    player:getHurt(spawn.Attack)
end

function OnDeath(spawn, gd)
    gd:debugLog("called onDeath")
end
