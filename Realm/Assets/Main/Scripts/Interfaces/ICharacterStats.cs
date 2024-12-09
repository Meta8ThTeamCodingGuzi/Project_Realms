public interface ICharacterStats
{
    void InitializeStats();
    /// <summary>
    /// Ư�� ������ ���� ���� ���� ��ȯ.
    /// ������ ���� ���� �ʿ��� �� ���.
    /// </summary>
    /// <param name="statType">Ȯ���ϰ��� �ϴ� ���� Ÿ��</param>
    /// <returns>��� �����ڰ� ����� ���� ���� ��</returns>
    /// <example>
    /// float currentHealth = stats.GetStatValue(StatType.Health);
    /// float attackPower = stats.GetStatValue(StatType.Attack);
    /// </example>
    float GetStatValue(StatType statType);

    /// <summary>
    /// Ư�� ���ȿ� �����ڸ� �߰�.
    /// </summary>
    /// <param name="statType">�����ϰ��� �ϴ� ���� Ÿ��</param>
    /// <param name="modifier">�߰��� ���� ������</param>
    void AddModifier(StatType statType, StatModifier modifier);

    /// <summary>
    /// Ư�� ���ȿ��� �����ڸ� ����.
    /// </summary>
    /// <param name="statType">�����ڸ� ������ ���� Ÿ��</param>
    /// <param name="modifier">������ ���� ������</param>
    void RemoveModifier(StatType statType, StatModifier modifier);

    /// <summary>
    /// Ư�� ������ Stat ��ü�� ���� ��ȯ.
    /// ���� �����ڸ� �����ϰų� Ư�� ��ó�� ȿ���� ������ �� ���.
    /// GetStatValue�� �޸� ���� ��ü ��ü�� �����Ͽ� �� ������ ��� ����.
    /// </summary>
    /// <param name="statType">������ ���� Ÿ��</param>
    /// <returns>�ش� ������ Stat ��ü</returns>
    /// <example>
    /// // Ư�� ����� ��� ȿ�� ����
    /// stats.GetStat(StatType.Attack).RemoveAllModifiersFromSource(equipmentItem);
    /// 
    /// // ���� ȿ���� �ִ��� Ȯ��
    /// bool hasBuff = stats.GetStat(StatType.Defense).HasModifierFromSource(SourceType.Buff);
    /// </example>
    Stat GetStat(StatType statType);
}
