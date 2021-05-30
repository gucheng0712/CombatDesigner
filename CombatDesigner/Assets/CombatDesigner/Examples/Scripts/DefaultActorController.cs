using Sirenix.OdinInspector;
using System;
using System.Linq;
using UnityEngine;

using CombatDesigner;


public class DefaultActorController : ActorController
{
    public override void Awake()
    {
        base.Awake();
    }

    public override void Init()
    {
        base.Init();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void UpdatePhysics()
    {
        base.UpdatePhysics();
    }

    protected override void UpdateAerialInfo()
    {
        base.UpdateAerialInfo();
    }

    protected override void UpdateAILogic()
    {
        base.UpdateAILogic();
    }

    protected override void UpdateAnimation()
    {
        base.UpdateAnimation();

        // todo falling animation based on the velocity
        model.animFallSpeed = model.velocity.y * 30f;

        model.anim.SetFloat("MoveSpeed", model.animMoveSpeed);
        model.anim.SetFloat("Air", model.animAir);
        model.anim.SetFloat("FallSpeed", model.animFallSpeed);
    }

    protected override void UpdateFriction()
    {
        base.UpdateFriction();
    }

    protected override void UpdateGrounededInfo()
    {
        base.UpdateGrounededInfo();
    }

    protected override void UpdatePlayerLogic()
    {
        base.UpdatePlayerLogic();
    }
}

