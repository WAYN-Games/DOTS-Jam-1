using Unity.Entities;
using UnityEngine;

class DamageDealerAuthoring : MonoBehaviour
{
    public DamageDealer damageDealer;
}

class DamageDealerBaker : Baker<DamageDealerAuthoring>
{
    public override void Bake(DamageDealerAuthoring authoring)
    {
        Entity bakingEntity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(bakingEntity, authoring.damageDealer);
    }
}
