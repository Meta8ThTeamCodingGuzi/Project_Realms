/*
 StatModifierType�� ��� ���:
 
 1. Flat: �������� ���ϰų� ���� �ܼ�����.
 ����) �⺻�� 100�� Flat +30 ���� => 130
 
 2. PercentAdd: ���� �ۼ�Ʈ �����ڵ��� ���� ���� �� �� ���� ����.
 ����) �⺻�� 100�� PercentAdd +20%, +30% ����
      => 100 * (1 + 0.2 + 0.3) = 100 * 1.5 = 150
 
 3. PercentMult: ������ �ۼ�Ʈ �����ڸ� ���������� ����.
 ����) �⺻�� 100�� PercentMult +20%, +30% ����
      => 100 * (1 + 0.2) * (1 + 0.3) = 100 * 1.2 * 1.3 = 156

 ���� ��� ����: Flat -> PercentAdd -> PercentMult
 ����) �⺻�� 100�� Flat +20, PercentAdd +50%, PercentMult +30% ����
      1) 100 + 20 = 120 (Flat)
      2) 120 * (1 + 0.5) = 180 (PercentAdd)
      3) 180 * (1 + 0.3) = 234 (PercentMult)
*/
public class StatModifier
{
    public readonly float Value;
    public readonly StatModifierType Type;
    public readonly object Source;
    public readonly SourceType SourceType;

    public StatModifier(float value, StatModifierType type, object source = null, SourceType sourceType = SourceType.None)
    {
        Value = (type == StatModifierType.PercentAdd || type == StatModifierType.PercentMult)
            ? value / 100f
            : value;
        Type = type;
        Source = source;
        SourceType = sourceType;
    }

    public StatModifier(float value, StatModifierType type, SourceType sourceType)
        : this(value, type, null, sourceType)
    {
    }
}