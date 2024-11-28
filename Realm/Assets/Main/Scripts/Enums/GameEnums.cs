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

    [Tooltip("��ų ����")]
    SkillLevel,
    [Tooltip("��ų ������")]
    Damage,
    [Tooltip("��ų ���ӽð�")]
    Duration,
    [Tooltip("��ų ��Ÿ��")]
    Cooldown,
    [Tooltip("����� ȣ�� Ȱ��ȭ���� (������ 0���� ���� ��Ź�帳�ϴ�)")]
    HomingLevel,
    
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

    #region ������ų ���� ����
    
    [Tooltip("��ȯ ��Ÿ�")]
    SpawnRange,
    [Tooltip("��ȯ�� ��ü ũ��")]
    SpawnScale,
    [Tooltip("�ѹ��� ��ȯ�� ����")]
    SpawnCount,
    [Tooltip("��ȯ ����")]
    SpawnInterval,
    [Tooltip("���콺 Ŀ������ ��ȯ���� (0�̸� false)")]
    IsSpawnAtCursor,
    [Tooltip("���� ��ġ�� �����Ͽ� ��ȯ���� (0�̸� false)")]
    IsSpawnAtEnemy,

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