using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// This class handles the player inputs.
/// </summary>
public class CombatInputManager : MonoBehaviour
{
    public InputAction AimInput;

    [Tooltip("The physics layer used by the ray cast to find hte world position")]
    public LayerMask BelongsTo;
    [Tooltip("The physics layer the tower can be placed on")]
    public LayerMask CollidesWith;

    public InputAction Skill1;



    /// <summary>
    /// The main camera used to compute teh world position from the screen position. 
    /// </summary>
    private Camera _camera;

    private CollisionFilter _filter;
    private EntityGameObject _entityGO;



    private void OnEnable()
    {
        // Setup input management
        AimInput.Enable();
        Skill1.Enable();

        // Cache the main camera of the scene
        _camera = Camera.main;

        // Initialize the collision filter that will be used by the physics system to find the position in the world where the tower will be spawned 
        _filter = CollisionFilter.Default;
        _filter.BelongsTo = (uint)BelongsTo.value;
        _filter.CollidesWith = (uint)CollidesWith.value;
       
    }

    private void Update()
    {

        if (_entityGO == null)
        {
            _entityGO = GetComponent<EntityGameObject>();
            return;
        }

        // We get the position of the mouse
        var screenPosition = AimInput.ReadValue<Vector2>();

        // convert it to a ray coming from the camera
        UnityEngine.Ray ray = _camera.ScreenPointToRay(screenPosition);

        RaycastInput raycastInput = new RaycastInput()
        {
            Start = ray.origin,
            End = ray.GetPoint(_camera.farClipPlane),
            Filter = _filter
        };

        int castSkill = -1;

        if (Skill1.inProgress)
        {
            castSkill = 0;
        }


        _entityGO.World.EntityManager.SetComponentData(_entityGO.Entity, new CombatInput()
        {
            Value = raycastInput,
            SkillCast = castSkill
        }) ;

    }


    private void OnDisable()
    {
        AimInput.Disable();
    }

    
}
public struct CombatInput : IComponentData
{
    public RaycastInput Value;
    public bool autoAttack;
    public int SkillCast;
}

public struct SkillCastData : IBufferElementData
{
    public Entity Value;
    public float Cooldown;
    public float SkillCooldown;
}
