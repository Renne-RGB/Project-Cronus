using UnityEngine;

public class Enemy_ShootState : EnemyState
{
    public Enemy_ShootState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }
}
