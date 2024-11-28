using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffSkill : Skill
{
    private BuffSkillStat buffSkillStat;
    private Coroutine BuffCoroutine; 
    private bool isSkillActive = false;

    public override void Initialize()
    {
        base.Initialize();
        buffSkillStat = (BuffSkillStat)skillStat;
        buffSkillStat.InitializeStats();
    }

    protected override void UseSkill()
    {
        if (BuffCoroutine != null)
        {
           StopSkill();
        }
        BuffCoroutine = StartCoroutine(ApplyBuff());
    }
    protected virtual void StopSkill()
    {
        if (BuffCoroutine != null)
        {
            StopCoroutine(BuffCoroutine);
            BuffCoroutine = null;
        }
    }

    protected virtual IEnumerator ApplyBuff()
    {
        print("자식에서 버프 내용 구현하세요");
        yield break;
    }

    private void OnDisable()
    {
        StopSkill();
    }

    public override void LevelUp()
    {
        base.LevelUp();
    }
}
