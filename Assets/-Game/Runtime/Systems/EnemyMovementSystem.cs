using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct EnemyMovementSystem : ISystem
{
    ComponentLookup<LocalToWorld> _positionComponentLookUp;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _positionComponentLookUp = SystemAPI.GetComponentLookup<LocalToWorld>(true);

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        _positionComponentLookUp.Update(ref state);

        foreach (var (characterControl, target, enemy, characterTransform) in 
            SystemAPI.Query<RefRW<ThirdPersonCharacterControl>, RefRW<Target>, RefRO<Enemy>, RefRO<LocalTransform>>())
        {
            if (Entity.Null.Equals(target.ValueRO.Value)) continue;

            if (!_positionComponentLookUp.HasComponent(target.ValueRO.Value))
            {
                target.ValueRW.Value = Entity.Null;
                continue;
            }

            var targetPosition = _positionComponentLookUp[target.ValueRO.Value];


            characterControl.ValueRW.MoveVector = math.select(float3.zero, math.normalizesafe(targetPosition.Position - characterTransform.ValueRO.Position), math.distance(targetPosition.Position, characterTransform.ValueRO.Position) > enemy.ValueRO.AttackRange*1.1f);

        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
