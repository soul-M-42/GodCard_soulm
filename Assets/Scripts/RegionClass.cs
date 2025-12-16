using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Region
{
    // 区域名称
    public string Name { get; set; }

    // 区域中的 Spawn 对象列表
    public List<Spawn> Spawns { get; private set; }

    // 区域的拥有者（玩家）
    public PlayerState Owner { get; set; }

    // 构造函数
    public Region(string name, PlayerState owner)
    {
        Name = name;
        Owner = owner;
        Spawns = new List<Spawn>();
    }

    public int OwnerAsInt()
    {
        if(Owner == null)
        {
            return 2;
        }
        else{
            if(Owner.Name == "Player1")
            {
                return 0;
            }
            if(Owner.Name == "Player2")
            {
                return 1;
            }
        }
        return 3;
    }

    // 添加 Spawn
    public void AddSpawn(Spawn spawn)
    {
        if (spawn != null && !Spawns.Contains(spawn))
        {
            Spawns.Add(spawn);
            spawn.Region = this;
        }
        updateOwner();
    }

    private void updateOwner()
    {
        if(Name.Contains("Prepare"))
        {
            return;
        }
        PlayerState owner = null;

        for (int i = 0; i < Spawns.Count; i++)
        {
            if (Spawns[i] == null)
                continue;

            if (owner == null)
            {
                owner = Spawns[i].Owner;
            }
            else if (owner != Spawns[i].Owner)
            {
                owner = null;
                break;
            }
        }

        this.Owner = owner;
    }

    // 移除 Spawn
    public void RemoveSpawn(Spawn spawn)
    {
        if (spawn != null && Spawns.Contains(spawn))
        {
            Spawns.Remove(spawn);
        }
        updateOwner();
    }

    // 清空区域内的所有 Spawn
    public void ClearSpawns()
    {
        Spawns.Clear();
    }

    public bool isPrepare()
    {
        return Name.Contains("Prepare");
    }
    public bool isWay()
    {
        return Name.Contains("Way");
    }
    // 调试输出
    public override string ToString()
    {
        return $"Region: {Name}, Owner: {Owner}, Spawns: {Spawns.Count}";
    }
}
