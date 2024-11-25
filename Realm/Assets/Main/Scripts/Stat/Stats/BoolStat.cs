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
        // Bool 스탯은 포인트 투자로 true로만 변경 가능
        investedPoints++;
        baseValue = true;
        isDirty = true;
    }

    protected override void CalculateFinalValue()
    {
        // modifier의 값이 양수면 true, 음수면 false로 강제
        var forceTrue = modifiers.Any(m => m.Value > 0);
        var forceFalse = modifiers.Any(m => m.Value < 0);

        if (forceTrue && !forceFalse) lastValue = true;
        else if (forceFalse && !forceTrue) lastValue = false;
        else lastValue = baseValue;
    }
}