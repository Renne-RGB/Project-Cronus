using UnityEngine;
using System.Collections.Generic;

public class Enemy_Noob : Enemy
{
    protected override void Awake()
    {
        base.Awake();

        chaseDistance = 15f;
        cqbDistance = 3f;
        shootRange = 10f;

        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        moveState = new Enemy_MoveState(this, stateMachine, "move");
        chaseState = new Enemy_ChaseState(this, stateMachine, "chase");
        cqbState = new Enemy_CqbState(this, stateMachine, "cqb");
        shootState = new Enemy_ShootState(this, stateMachine, "shoot");
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();
    }

}
