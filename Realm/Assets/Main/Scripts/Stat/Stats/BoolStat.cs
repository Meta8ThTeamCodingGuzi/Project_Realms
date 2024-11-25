using System.Linq;

public class BoolStat : Stat
{
    protected bool baseValue;
    protected bool lastValue;

    public BoolStat(bool baseValue = false) : base()
    {
        this.baseValue = baseValue;
    }

    public override object Value
    {
        get
        {
            if (isDirty)
            {
                CalculateFinalValue();
                isDirty = false;
            }
            return lastValue;
        }
    }

    public override void InvestPoint(float increaseAmount)
    {
        // Bool ������ ����Ʈ ���ڷ� true�θ� ���� ����
        investedPoints++;
        baseValue = true;
        isDirty = true;
    }

    protected override void CalculateFinalValue()
    {
        // modifier�� ���� ����� true, ������ false�� ����
        var forceTrue = modifiers.Any(m => m.Value > 0);
        var forceFalse = modifiers.Any(m => m.Value < 0);

        if (forceTrue && !forceFalse) lastValue = true;
        else if (forceFalse && !forceTrue) lastValue = false;
        else lastValue = baseValue;
    }
}