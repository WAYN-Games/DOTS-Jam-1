using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using UnityEngine.SceneManagement;



partial struct UITriggerSystem : ISystem
{
    private ComponentLookup<UIToogleComponent> _sceneSwitchComponentLookUp;
    private ComponentLookup<PlayerTag> _playerTagLookUp;
    private NativeQueue<Entity> _UIToLoadCommand;



    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _sceneSwitchComponentLookUp = SystemAPI.GetComponentLookup<UIToogleComponent>(true);
        _playerTagLookUp = SystemAPI.GetComponentLookup<PlayerTag>(true);
        _UIToLoadCommand = new NativeQueue<Entity>(Allocator.Persistent);

    }

    public void OnUpdate(ref SystemState state)
    {


        SimulationSingleton simulation = SystemAPI.GetSingleton<SimulationSingleton>();

        _playerTagLookUp.Update(ref state);
        _sceneSwitchComponentLookUp.Update(ref state);
        state.Dependency = new InteracitonJob()
        {
            PlayerTagLookUp = _playerTagLookUp,
            SceneSwitchComponentLookUp = _sceneSwitchComponentLookUp,
            UILoadCommand = _UIToLoadCommand
        }.Schedule(simulation, state.Dependency);

        state.Dependency.Complete();
        var array = _UIToLoadCommand.ToArray(Allocator.Temp);

        foreach (var (ui, entity) in SystemAPI.Query<SystemAPI.ManagedAPI.UnityEngineComponent<UIManager>>().WithEntityAccess())
        {
            if (array.Contains(entity))
            {
                ui.Value.ShowUI(true);
            }
            else
            {
                ui.Value.ShowUI(false);
            }
        }

        _UIToLoadCommand.Clear();


    }


    public struct InteracitonJob : ITriggerEventsJob
    {
        [ReadOnly]
        public ComponentLookup<UIToogleComponent> SceneSwitchComponentLookUp;
        [ReadOnly]
        public ComponentLookup<PlayerTag> PlayerTagLookUp;

        public NativeQueue<Entity> UILoadCommand;

        public void Execute(TriggerEvent triggerEvent)
        {
            var (player, uiToogleEntity) = IdentifyEntityPair(triggerEvent);

            if (!ShouldProcess(player, uiToogleEntity)) return;


            UILoadCommand.Enqueue(uiToogleEntity);

        }

        private static bool ShouldProcess(Entity player, Entity sceneSwitch)
        {
            return !Entity.Null.Equals(player) && !Entity.Null.Equals(sceneSwitch);
        }


        private (Entity, Entity) IdentifyEntityPair(TriggerEvent triggerEvent)
        {
            Entity player = Entity.Null;
            Entity sceneSwitch = Entity.Null;

            // For each entity involved in the even, we check if it is available in one of the lookup 
            if (PlayerTagLookUp.HasComponent(triggerEvent.EntityA))
                player = triggerEvent.EntityA;
            if (PlayerTagLookUp.HasComponent(triggerEvent.EntityB))
                player = triggerEvent.EntityB;
            if (SceneSwitchComponentLookUp.HasComponent(triggerEvent.EntityA))
                sceneSwitch = triggerEvent.EntityA;
            if (SceneSwitchComponentLookUp.HasComponent(triggerEvent.EntityB))
                sceneSwitch = triggerEvent.EntityB;
            return (player, sceneSwitch);
        }
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        _UIToLoadCommand.Dispose();
    }
}
