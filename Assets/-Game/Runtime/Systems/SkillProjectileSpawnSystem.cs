
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

// We update after the FixedStepSimulationSystemGroup
// which contains the physics update to use the latest PhysicsWorldSingleton available
[UpdateAfter(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(SkillCleanupSystem))]
[BurstCompile]
public partial struct TowerPlacementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<PhysicsWorldSingleton>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        EntityCommandBuffer ecbBos = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        
        foreach (var (input,position,skillCastInputs) in SystemAPI.Query<RefRO<CombatInput>,RefRO<LocalToWorld>,DynamicBuffer<SkillCastData>>())
        {

            var proxy = skillCastInputs;

            for (int i = 0; i < skillCastInputs.Length; i++)
            {
                 var skillCastInput = skillCastInputs[i];

                if (input.ValueRO.SkillCast != i) continue;

                if (skillCastInput.Cooldown > 0) continue;

                skillCastInput.Cooldown = skillCastInput.SkillCooldown;
                float3 aimDirection = position.ValueRO.Forward;

                if (!input.ValueRO.autoAttack) { 
                    if (!physicsWorld.CastRay(input.ValueRO.Value, out var hit)) continue;
                
                     aimDirection = math.normalizesafe(hit.Position - position.ValueRO.Position);
                }
                // instantiate the tower at the position
                Entity e = ecbBos.Instantiate(skillCastInput.Value);
                LocalTransform transform = LocalTransform.FromPositionRotation(
                            position.ValueRO.Position + aimDirection,
                            quaternion.LookRotationSafe(aimDirection, math.up()));                
                ecbBos.SetComponent(e, transform);


                proxy[i] = skillCastInput;

            }

        }

    }
}