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
    #region ����ü��ų���� ����
    ProjectileSpeed,
    ProjectileRange,
    ProjectileScale,
    ProjectileCount,
    ShotInterval,
    InnerInterval,
    //���� ����ü�� ���
    ExplosionRadius,
    //������Ÿ�
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