using UnityEngine;
using System.Collections.Generic;

public class Enemy_Timid : Enemy
{
    
    protected override void Awake()
    {
        base.Awake();

        chaseDistance = 15f;
        cqbDistance = 3f;

        shootRange = 10f;
        shootNum = 1;

        canMelee = false;
        canBlock = false;

        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        moveState = new Enemy_MoveState(this, stateMachine, "move");
        chaseState = new Enemy_ChaseState(this, stateMachine, "chase");
        shootState = new Enemy_ShootState(this, stateMachine, "shoot");
        alertState = new Enemy_AlertState(this, stateMachine, "alert");
        searchState = new Enemy_SearchState(this, stateMachine, "search");
        knockbackState = new Enemy_KnockbackState(this, stateMachine, "knockback");
        faintState = new Enemy_FaintState(this, stateMachine, "faint");
        susState = new Enemy_SuspiciousState(this, stateMachine, "sus");
        hearState = new Enemy_HearState(this, stateMachine, "hear");
        investigateState = new Enemy_InvestigateState(this, stateMachine, "investigate");
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
