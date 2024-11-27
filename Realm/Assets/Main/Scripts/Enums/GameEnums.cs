using UnityEngine;

public enum StatType
{
    Level,
    MaxHealth,
    Health,
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
    #region 스킬 공통 스탯

    SkillLevel,
    Damage,
    Duration,

    #endregion

    #region 투사체스킬관련 스탯

    [Tooltip("투사체 속도")]
    ProjectileSpeed,
    [Tooltip("투사체 사거리")]
    ProjectileRange,
    [Tooltip("투사체 크기")]
    ProjectileScale,
    [Tooltip("한번 발사에 몇개 발사할지")]
    ProjectileCount,
    [Tooltip("폭발 반경(폭발 투사체일경우)")]
    ExplosionRadius,
    [Tooltip("유도기능 활성화 (인스펙터에서 BoolStat으로 체크할것!)")]
    IsHoming,
    [Tooltip("유도사거리")]
    HomingRange,
    [Tooltip("투사체 관통 횟수")]
    PierceCount,
    [Tooltip("발사 간격")]
    ShotInterval,
    [Tooltip("연사 속도")]
    InnerInterval,

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