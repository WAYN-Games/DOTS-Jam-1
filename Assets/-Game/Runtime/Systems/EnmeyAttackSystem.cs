using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct EnmeyAttackSystem : ISystem
{
    ComponentLookup<LocalToWorld> _positionLookup;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _positionLookup = SystemAPI.GetComponentLookup<LocalToWorld>(true);

    }

    public void OnUpdate(ref SystemState state)
    {
        _positionLookup.Update(ref state);

        foreach (var (combatInput, skillCastDataBuffer,target,enemy,localToWorld) in SystemAPI.Query<RefRW<CombatInput>, DynamicBuffer<SkillCastData>,
            RefRO<Target>, RefRO<Enemy>, RefRO<LocalTransform>>().WithAll<Enemy>())
        {
            if (Entity.Null.Equals(target.ValueRO.Value)) continue;

            var targetPosition = _positionLookup[target.ValueRO.Value];


            if(math.distance(targetPosition.Position, localToWorld.ValueRO.Position) > enemy.ValueRO.AttackRange) continue;


            for (int i = 0; i <  skillCastDataBuffer.Length; i++)
            {
                if (skillCastDataBuffer[i].Cooldown > 0) continue;

                combatInput.ValueRW.SkillCast = i;
                combatInput.ValueRW.autoAttack = true;
                break;
            }
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        
    }
}
