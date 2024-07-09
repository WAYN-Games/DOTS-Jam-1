
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

partial struct AggroSystem : ISystem
{
    private ComponentLookup<Enemy> _enemyComponentLookUp;
    private ComponentLookup<Target> _targetComponentLookUp;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

        _enemyComponentLookUp = SystemAPI.GetComponentLookup<Enemy>(true);
        _targetComponentLookUp = SystemAPI.GetComponentLookup<Target>(false);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {

        _enemyComponentLookUp.Update(ref state);
        _targetComponentLookUp.Update(ref state);

        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        foreach (var (localToWorld,playerEntity) in SystemAPI.Query<RefRO<LocalToWorld>>().WithAll<PlayerTag>().WithEntityAccess())
        {
            NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.Temp);
            if(physicsWorldSingleton.OverlapSphere(localToWorld.ValueRO.Position,50f,ref hits, CollisionFilter.Default))
            {
                foreach (var hit in hits)
                {

                    if (!_enemyComponentLookUp.HasComponent(hit.Entity)) continue;
                    var enemy = _enemyComponentLookUp[hit.Entity];
                    if(enemy.AggroRange < hit.Distance) continue;

                    var target = _targetComponentLookUp[hit.Entity];
                    target.Value = playerEntity;
                    _targetComponentLookUp[hit.Entity] = target;
                }

            }
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
