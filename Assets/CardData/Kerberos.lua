cardName = "Kerberos"
health = 2
attack = 2
moveCost = 1
deployCost = 2
move = 2
type = "Ghost-Monster"
skill_type = "Melee"
skill_text = "Ghost - Melee\nMonster: Act twice per turn."

function OnPlay(card, gd)
    gd:debugLog(card.Name .. " played")
end

function OnMove(spawn, gd)
    gd:debugLog(spawn.Name .. " moved")
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
