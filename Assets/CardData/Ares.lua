cardName = "Ares"
health = 5
attack = 3
moveCost = 1
deployCost = 3
move = 1
type = "God"
skill_type = "Melee"
skill_text = "God - Melee"

function OnPlay(card, gd) 
    gd:debugLog("called onPlay")
end

function OnMove(spawn, gd) 
    gd:debugLog("called onMove")
end

function OnInterWithSpawn(spawn_1, spawn_2, gd)
    gd:debugLog("Lua " .. spawn_1.Name  .. " inter with " .. spawn_2.Name)
    spawn_1:getHurt(spawn_2.Attack)
    spawn_2:getHurt(spawn_1.Attack)
end

function OnInterWithPlayer(spawn, player, gd)
    gd:debugLog("Lua " .. spawn.Name .. " inter with player " .. player.Name)
    player:getHurt(attack)
end

function OnDeath(spawn, gd) 
    gd:debugLog("called onDeath")
end
