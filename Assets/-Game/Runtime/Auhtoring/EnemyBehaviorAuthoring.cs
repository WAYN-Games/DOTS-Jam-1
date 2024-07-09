using Unity.Entities;
using UnityEngine;

class EnemyBehaviorAuhtoring : MonoBehaviour
{
    public float AggroRange;
    public float AttackRange;
}

class EnemyBehaviorScriptBaker : Baker<EnemyBehaviorAuhtoring>
{
    public override void Bake(EnemyBehaviorAuhtoring authoring)
    {
        Entity bakingEntity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(bakingEntity, new Enemy()
        {
            AggroRange = authoring.AggroRange,
            AttackRange = authoring.AttackRange
        });

        AddComponent(bakingEntity, new Target()
        {
            Value = Entity.Null
        });
    }
}

public struct Target : IComponentData{
    public Entity Value;    
}

public struct Enemy : IComponentData
{
    public float AggroRange;
    public float AttackRange;
}