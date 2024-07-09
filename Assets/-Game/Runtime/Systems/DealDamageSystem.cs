using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using UnityEngine;
using Unity.Mathematics;


partial struct RegenSystem : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var hpRW in SystemAPI.Query<RefRW<Health>>())
        {
            ref Health hp = ref hpRW.ValueRW;
            hp.Value += hp.Regen * SystemAPI.Time.DeltaTime;
            hp.Value = math.min(hp.Value, hp.MaxValue);
        }

        foreach (var manaRW in SystemAPI.Query<RefRW<Mana>>())
        {
            ref Mana mana = ref manaRW.ValueRW;
            mana.Value += mana.Regen * SystemAPI.Time.DeltaTime;
            mana.Value = math.min(mana.Value, mana.MaxValue);
        }


    }


}

    partial struct DealDamageSystem : ISystem
{

    private ComponentLookup<Health> _healthLookup;
    private ComponentLookup<DamageDealer> _damageDealerLookup;


    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _healthLookup = SystemAPI.GetComponentLookup<Health>(false);
        _damageDealerLookup = SystemAPI.GetComponentLookup<DamageDealer>(true);

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        SimulationSingleton simulation = SystemAPI.GetSingleton<SimulationSingleton>();
        EntityCommandBuffer ecb = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        _healthLookup.Update(ref state);
        _damageDealerLookup.Update(ref state);
        state.Dependency = new DamageJob()
        {
            HealthLookup = _healthLookup,
            DamageDealerLookup = _damageDealerLookup,
            ecb = ecb
        }.Schedule(simulation, state.Dependency);
        



    }


    public struct DamageJob : ITriggerEventsJob
    {
        public ComponentLookup<Health> HealthLookup;
        [ReadOnly]
        public ComponentLookup<DamageDealer> DamageDealerLookup;
        public EntityCommandBuffer ecb;

        public void Execute(TriggerEvent triggerEvent)
        {
            var (healthEntity, damageDealerEntity) = IdentifyEntityPair(triggerEvent);

            if (!ShouldProcess(healthEntity, damageDealerEntity)) return;

            Health hp = HealthLookup[healthEntity];
            DamageDealer damage = DamageDealerLookup[damageDealerEntity];
            hp.Value -= damage.Value;
            HealthLookup[healthEntity] = hp;
            if (hp.Value < 0)
            {
                ecb.DestroyEntity(healthEntity);
            }

            ecb.DestroyEntity(damageDealerEntity);
        }

        private static bool ShouldProcess(Entity entityA, Entity entityB)
        {
            return !Entity.Null.Equals(entityA) && !Entity.Null.Equals(entityB);
        }


        private (Entity, Entity) IdentifyEntityPair(TriggerEvent triggerEvent)
        {
            Entity entityA = Entity.Null;
            Entity entityB = Entity.Null;

            // For each entity involved in the even, we check if it is available in one of the lookup 
            if (HealthLookup.HasComponent(triggerEvent.EntityA))
                entityA = triggerEvent.EntityA;
            if (HealthLookup.HasComponent(triggerEvent.EntityB))
                entityA = triggerEvent.EntityB;
            if (DamageDealerLookup.HasComponent(triggerEvent.EntityA))
                entityB = triggerEvent.EntityA;
            if (DamageDealerLookup.HasComponent(triggerEvent.EntityB))
                entityB = triggerEvent.EntityB;
            return (entityA, entityB);
        }
    }


}
