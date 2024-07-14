using Unity.Entities;
using UnityEngine;

class PlayerAuthoring : MonoBehaviour
{
    
}

class PlayerAuthoringBaker : Baker<PlayerAuthoring>
{
    public override void Bake(PlayerAuthoring authoring)
    {
        Entity bakingEntity = GetEntity(authoring, TransformUsageFlags.Dynamic);

        AddComponent<PlayerTag>(bakingEntity);   
    }
}

public struct PlayerTag : IComponentData { }
