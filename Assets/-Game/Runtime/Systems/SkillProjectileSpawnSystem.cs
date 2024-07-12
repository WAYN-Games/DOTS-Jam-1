
using Unity.Burst;
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
public partial struct SkillExecutionSystem : ISystem
{
    private ComponentLookup<LocalTransform> _transformLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BeginInitializationEntityCommandBufferSystem.Singleton>();
        state.RequireForUpdate<PhysicsWorldSingleton>();
        _transformLookup = SystemAPI.GetComponentLookup<LocalTransform>(false);
    }
    
    public void OnUpdate(ref SystemState state)
    {
        PhysicsWorldSingleton physicsWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>();
        EntityCommandBuffer ecbBos = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        _transformLookup.Update(ref state);

        foreach (var (input,position,skillCastInputs,animator,entity) in SystemAPI.Query<RefRO<CombatInput>,RefRO<LocalTransform>,DynamicBuffer<SkillCastData>
            ,SystemAPI.ManagedAPI.UnityEngineComponent<Animator>>().WithEntityAccess())
        {

            var proxy = skillCastInputs;

            for (int i = 0; i < skillCastInputs.Length; i++)
            {
                 var skillCastInput = skillCastInputs[i];

                if (input.ValueRO.SkillCast != i) continue;

                if (skillCastInput.Cooldown > 0) continue;

                skillCastInput.Cooldown = skillCastInput.SkillCooldown;
                animator.Value.Play("CharacterArmature|Punch");

                float3 aimDirection = position.ValueRO.Forward();
                
                if (!input.ValueRO.autoAttack) { 
                    if (!physicsWorld.CastRay(input.ValueRO.Value, out var hit)) continue;                
                    aimDirection = math.normalizesafe(hit.Position - position.ValueRO.Position);
                    var lookDirection = aimDirection;
                    lookDirection.y = 0;
                    _transformLookup[entity] = LocalTransform.FromPositionRotationScale(
                        _transformLookup[entity].Position,
                        quaternion.LookRotationSafe(lookDirection, math.up()),
                        _transformLookup[entity].Scale
                        );

                    _transformLookup[entity] = 
                        _transformLookup[entity].RotateY(45);
                }


                Debug.Log($"{position.ValueRO.Position + aimDirection} / {aimDirection}");
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