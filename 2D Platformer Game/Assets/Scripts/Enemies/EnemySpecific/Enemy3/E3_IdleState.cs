using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E3_IdleState : IdleState
{
    Enemy3 enemy;

    public E3_IdleState(Entity entity, FiniteStateMachine stateMachine, string animBoolName, D_IdleState stateData, Enemy3 enemy) : base(entity, stateMachine, animBoolName, stateData)
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (isPlayerInMaxAgroRange)
        {
            if (!detectionSoundPlayed)
            {
                enemy.playerDetectedState.PlayDetectionSound();
                detectionSoundPlayed = true;
            }

            stateMachine.ChangeState(enemy.playerDetectedState);
        }
        else if (isIdleTimeOver)
        {
            detectionSoundPlayed = false;
            stateMachine.ChangeState(enemy.moveState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
