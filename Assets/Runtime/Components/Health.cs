using System;
using Unity.Entities;

public struct Health : IComponentData
{
    public float Value;
    public float MaxValue;
    public float Regen;
}


public struct Mana : IComponentData
{
    public float Value;
    public float MaxValue;
    public float Regen;
}


[Serializable]
public struct DamageDealer : IComponentData
{
    public float Value;
}