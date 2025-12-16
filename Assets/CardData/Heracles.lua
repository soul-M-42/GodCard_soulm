cardName = "Heracles"
health = 2
attack = 3
moveCost = 1
deployCost = 2
move = 1
type = "Ghost"
skill_type = "Melee"
skill_text = "Ghost - Melee\nHero's Name: When placed, suppresses all enemy monsters for one turn; damage dealt to monsters +2."

function OnPlay(card, gd)
    gd:debugLog(card.Name .. " used Hero's Name skill")
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
    if spawn_2.isMonster() then
        spawn_2:getHurt(2)
    end
end

function OnInterWithPlayer(spawn, player, gd)
    gd:debugLog("Lua " .. spawn.Name .. " inter with " .. player.Name)
    player:getHurt(spawn.Attack)
end

function OnDeath(spawn, gd)
    gd:debugLog("called onDeath")
end
