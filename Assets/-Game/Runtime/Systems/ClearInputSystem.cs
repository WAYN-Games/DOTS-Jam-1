
using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct SkillCleanupSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var input in SystemAPI.Query<DynamicBuffer<SkillCastInput>>())
        {
            var proxy = input;
            for (int i = 0; i < input.Length; i++)
            {
                var skillCastInput = proxy[i];
                skillCastInput.Cooldown -= SystemAPI.Time.DeltaTime;
                proxy[i] = skillCastInput;
            }
        } 
    }
}