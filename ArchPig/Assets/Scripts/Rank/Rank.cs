using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum RankType
{
    None = 0,
    RoleLevel,
    RolePower

}

public class Rank
{
    public Rank(string Guid, RankType type)
    {
        this.Guid = Guid;
        this.type = type;
    }
    public string Guid;
    public RankType type;
}

public class RoleLevelRank : Rank
{
    public RoleLevelRank(string Guid, RankType type, string name, int level) : base(Guid, type)
    {
        this.name = name;
        this.level = level;
    }
    public string name;
    public int level;
}

public class RolePowerRank : Rank
{
    public RolePowerRank(string Guid, RankType type, string name, int power) : base(Guid, type)
    {
        this.name = name;
        this.power = power;
    }
    public string name;
    public int power;
}