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
    #region ��ų ���� ����

    SkillLevel,
    Damage,
    Duration,

    #endregion

    #region ����ü��ų���� ����

    [Tooltip("����ü �ӵ�")]
    ProjectileSpeed,
    [Tooltip("����ü ��Ÿ�")]
    ProjectileRange,
    [Tooltip("����ü ũ��")]
    ProjectileScale,
    [Tooltip("�ѹ� �߻翡 � �߻�����")]
    ProjectileCount,
    [Tooltip("���� �ݰ�(���� ����ü�ϰ��)")]
    ExplosionRadius,
    [Tooltip("������� Ȱ��ȭ (�ν����Ϳ��� BoolStat���� üũ�Ұ�!)")]
    IsHoming,
    [Tooltip("������Ÿ�")]
    HomingRange,
    [Tooltip("����ü ���� Ƚ��")]
    PierceCount,
    [Tooltip("�߻� ����")]
    ShotInterval,
    [Tooltip("���� �ӵ�")]
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