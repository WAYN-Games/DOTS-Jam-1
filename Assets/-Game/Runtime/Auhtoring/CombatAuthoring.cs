using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

class CombatAuthoring : MonoBehaviour
{
    public float Health;
    public float Mana;
    public float HealthRegen;
    public float ManaRegen;
    public List<Skill> Skills = new List<Skill>();
}

[Serializable]
public class Skill
{
    public GameObject Projectile;
    public float Cooldown;
}


class CombatAuthoringBaker : Baker<CombatAuthoring>
{
    public override void Bake(CombatAuthoring authoring)
    {
        Entity bakingEntity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(bakingEntity, new Health()
        {
            MaxValue = authoring.Health,
            Regen = authoring.HealthRegen,
            Value = authoring.Health
        });

        AddComponent(bakingEntity, new Mana()
        {
            MaxValue = authoring.Mana,
            Regen = authoring.ManaRegen,
            Value = authoring.Mana
        });

        AddComponent<CombatInput>(bakingEntity);

        
        var skillbuffer = AddBuffer<SkillCastInput>(bakingEntity);

        foreach(var skill in authoring.Skills)
        {
            skillbuffer.Add(
            new SkillCastInput()
            {
                Cooldown = skill.Cooldown,
                SkillCooldown = skill.Cooldown,
                Value = GetEntity(skill.Projectile, TransformUsageFlags.Dynamic)
            }
            );
        }



    }
}

