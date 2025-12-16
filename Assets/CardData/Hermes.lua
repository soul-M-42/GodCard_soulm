cardName = "Hermes"
health = 2
attack = 1
moveCost = 0
deployCost = 1
move = 1
type = "God"
skill_type = "Melee"
skill_text = "God - Melee"

function OnPlay(card, gd) 
    -- print(card.Name .. " casts Fire Slash!")
    gd:debugLog("called onPlay")
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
    player:getHurt(attack)
end

function OnDeath(spawn, gd) 
    gd:debugLog("called onDeath")
end
