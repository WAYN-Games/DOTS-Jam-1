
using System.Collections.Generic;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

class Spawner : MonoBehaviour
{
    public List<GameObject> Prefabs = new List<GameObject>();
    public float TimeBetweenSpawns;
    public float Range;
}

class SpawnerBaker : Baker<Spawner>
{
    public override void Bake(Spawner authoring)
    {
        Entity bakingEntity = GetEntity(TransformUsageFlags.Dynamic);

        var buffer = AddBuffer<SpawnableEntities>(bakingEntity);

        foreach(var prefab in authoring.Prefabs)
        {
            buffer.Add(new SpawnableEntities() { Prefab = GetEntity(prefab, TransformUsageFlags.Dynamic) });
        }

        AddComponent(bakingEntity, new SpawnerTimer()
        {
            Time = authoring.TimeBetweenSpawns,
            TimeToNextSpawn = authoring.TimeBetweenSpawns,
            Range = authoring.Range
        });



    }
}
public struct SpawnerTimer : IComponentData
{ 
    public float Time;
    public float TimeToNextSpawn;
    public float Range;
}

public struct SpawnableEntities : IBufferElementData
{
    public Entity Prefab;
}



[BurstCompile]
public partial struct SpawnSystem : ISystem
{
    private Unity.Mathematics.Random _r;

    public void OnCreate(ref SystemState state)
    {
        _r = Unity.Mathematics.Random.CreateFromIndex(123456);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        

        foreach (var (spawnableEntities,timer,position) in SystemAPI.Query < DynamicBuffer<SpawnableEntities>,RefRW<SpawnerTimer>, RefRO < LocalToWorld>>())
        {
            if(timer.ValueRO.TimeToNextSpawn > 0)
            {
                timer.ValueRW.TimeToNextSpawn -= SystemAPI.Time.DeltaTime;
                continue;
            }

            timer.ValueRW.TimeToNextSpawn = timer.ValueRO.Time;

            var index = _r.NextInt(0, spawnableEntities.Length);
            Entity spawn = state.EntityManager.Instantiate(spawnableEntities[index].Prefab);
            state.EntityManager.SetComponentData(spawn,
                LocalTransform.FromPosition(position.ValueRO.Position + _r.NextFloat3Direction() * _r.NextFloat(0, timer.ValueRO.Range)));

        }
    }
}

