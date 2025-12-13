using UnityEngine;
using System.Collections.Generic;

public class Enemy_Noob : Enemy
{
    [SerializeField] private FieldOfView fieldOfView;
    protected override void Awake()
    {
        base.Awake();

        chaseDistance = 10.0f;

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
        
        //!!!原因がわからないけど取り敢えずMovementInputの反時計回りはちょうど敵の移動方向である
        Vector3 desiredAimDirection = new Vector3(-MovementInput.y, MovementInput.x, 0f);
        fieldOfView.SetAimDirection(desiredAimDirection);
        fieldOfView.SetOrigin(transform.position);
    }

}
