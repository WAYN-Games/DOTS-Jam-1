using NaughtyAttributes;
using Unity.Entities;
using UnityEngine;

class SceneSwitch : MonoBehaviour
{
    [Scene]
    public int InGameScene;
}

class SceneSwitchBaker : Baker<SceneSwitch>
{
    public override void Bake(SceneSwitch authoring)
    {
        Entity bakingEntity = GetEntity(authoring,TransformUsageFlags.WorldSpace);

        AddComponent(bakingEntity, new SceneSwitchComponent()
        {
            TargetScene = authoring.InGameScene
        });
    }
}




