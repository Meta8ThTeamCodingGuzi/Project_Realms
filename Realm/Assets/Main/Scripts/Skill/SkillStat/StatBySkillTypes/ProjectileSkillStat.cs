using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSkillStat : SkillStat
{
    [Header("투사체 스킬 스탯")]
    [SerializeField]
    protected new StatInitializer[] initialStats = new StatInitializer[]
    {
        new StatInitializer { Type = SkillStatType.Damage, BaseValue = 0f },
        new StatInitializer { Type = SkillStatType.ProjectileSpeed, BaseValue = 0f },
        new StatInitializer { Type = SkillStatType.ProjectileRange, BaseValue = 0f },
        new StatInitializer { Type = SkillStatType.PierceCount, BaseValue = 0f },
        new StatInitializer { Type = SkillStatType.ExplosionRadius, BaseValue = 0f},
        new StatInitializer { Type = SkillStatType.HomingRange, BaseValue = 0f },
        new StatInitializer { Type = SkillStatType.InnerInterval, BaseValue = 0f },
        new StatInitializer { Type = SkillStatType.ShotInterval, BaseValue = 0f },
        new StatInitializer { Type = SkillStatType.ProjectileCount, BaseValue = 0f },
    };
}
