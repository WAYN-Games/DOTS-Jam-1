using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct ProjectileMoveSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach(var (projectile,localTranform) in SystemAPI.Query<RefRO<DamageDealer>, RefRW<LocalTransform>>())
        {
            localTranform.ValueRW.Position += localTranform.ValueRO.Forward() * projectile.ValueRO.Speed * SystemAPI.Time.DeltaTime;
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
