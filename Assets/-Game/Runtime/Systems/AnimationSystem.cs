using Unity.Burst;
using Unity.CharacterController;
using Unity.Entities;
using UnityEngine;
using Unity.Mathematics;

partial struct AnimationSystem : ISystem
{

    public void OnUpdate(ref SystemState state)
    {
        foreach(var (animator,character) in 
            SystemAPI.Query<SystemAPI.ManagedAPI.UnityEngineComponent<Animator>, 
            KinematicCharacterAspect>())
        {
            animator.Value.SetFloat("Speed", math.length(character.CharacterBody.ValueRO.RelativeVelocity));
            
        }
    }

}
