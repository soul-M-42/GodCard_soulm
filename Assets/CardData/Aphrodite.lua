cardName = "Aphrodite"
health = 3
attack = 1
moveCost = 1
deployCost = 2
move = 1
type = "God"
skill_type = "Ranged"
skill_text = "God - Ranged\nRomantic Curse: The attacked unit takes 1 unit less damage"

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
    spawn_2:getHurt(spawn_1.Attack)
    spawn_2:atkPlus(-1)
end

function OnInterWithPlayer(spawn, player, gd)
    gd:debugLog("Lua " .. spawn.Name .. " inter with " .. player.Name)
    player:getHurt(attack)
end

function OnDeath(spawn, gd) 
    gd:debugLog("called onDeath")
end
