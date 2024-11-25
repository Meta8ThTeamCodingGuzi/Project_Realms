public enum StatType
{
    Level,
    MaxHealth,
    Health,
    Attack,
    Defense,
    MoveSpeed,
    AttackSpeed,
    AttackRange,
    ExpRange,
}

public enum SkillStatType 
{
    Damage,
    Duration,
    #region 투사체스킬관련 스탯
    ProjectileSpeed,
    ProjectileRange,
    ProjectileScale,
    ProjectileCount,
    ShotInterval,
    InnerInterval,
    //폭발 투사체일 경우
    ExplosionRadius,
    //유도사거리
    HomingRange,
    PierceCount,
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