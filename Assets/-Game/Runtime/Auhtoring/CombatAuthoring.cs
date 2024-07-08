using Unity.Entities;
using UnityEngine;

class CombatAuthoring : MonoBehaviour
{
    public float Health;
    public float Mana;
    public float HealthRegen;
    public float ManaRegen;
    public GameObject projectile;
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

    }
}