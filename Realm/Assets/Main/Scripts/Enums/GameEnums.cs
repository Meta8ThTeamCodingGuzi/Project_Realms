using UnityEngine;

public enum StatType
{
    Level,
    MaxHealth,
    Health,
    MaxMana,
    Mana,
    Damage,
    Defense,
    MoveSpeed,
    AttackSpeed,
    AttackRange,
    ExpRange,
    DropExp
}

public enum SkillID 
{
    TestSkill
}
public enum SkillStatType
{
    #region Common Skill Stats

    SkillLevel,
    ManaCost,
    Damage,
    Duration,
    Cooldown,
    HomingLevel,

    #endregion

    #region Projectile Related Skills

    ProjectileSpeed,
    ProjectileRange,
    ProjectileScale,
    ProjectileCount,
    ExplosionRadius,
    IsHoming,
    HomingRange,
    PierceCount,
    ShotInterval,
    InnerInterval,
    
    #endregion

    #region Area Related Stats
    
    SpawnRange,
    SpawnScale,
    SpawnCount,
    SpawnInterval,
    IsSpawnAtCursor,
    IsSpawnAtEnemy,

    #endregion

    #region Buff Related Stats
    
    BuffValue,
    DeBuffAreaScale,
    
    #endregion

}

public enum SourceType
{
    None,
    BaseStats,
    Equipment,
    Skill,
    Buff,
    Debuff,
}


public enum StatModifierType
{
    Flat,
    PercentAdd,
    PercentMult
}

public enum Playerjob
{
    knight,
    Archer
}