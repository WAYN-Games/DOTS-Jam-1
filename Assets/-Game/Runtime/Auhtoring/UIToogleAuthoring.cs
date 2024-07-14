using Unity.Entities;
using UnityEngine;

class UIToogleAuthoring    : MonoBehaviour
{
    
}

class UIToogleAuthoringBaker : Baker<UIToogleAuthoring>
{
    public override void Bake(UIToogleAuthoring authoring)
    {
        Entity bakingEntity = GetEntity(authoring, TransformUsageFlags.Dynamic);

        AddComponent<UIToogleComponent>(bakingEntity);   
    
    }
}




public struct UIToogleComponent : IComponentData
{

}
