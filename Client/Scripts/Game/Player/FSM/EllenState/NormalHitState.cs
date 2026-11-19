using UnityEngine;

public class NormalHitState : IState
{
    private PlayerController player;

    private bool canInterrupt;//是否允许打断受击动画
    private bool hasEvadeInput;//是否提前按下闪避键

    public NormalHitState(PlayerController player)
    {
        this.player = player;
    }

    public void OnEnter()
    {
        canInterrupt = false;
        hasEvadeInput = false;

        player.PlayAnim(PlayerAnimationType.NormalHit);
    }

    public void OnExit()
    {
        canInterrupt = false;
        hasEvadeInput = false;
    }

    public void OnUpdate()
    {
        // 提前记录闪避输入
        if (player.Input.IsEvadePressed)
        {
            hasEvadeInput = true;
        }

        if (canInterrupt)
        {
            // 闪避优先级高于普通移动
            if (hasEvadeInput)
            {
                if (player.Input.HasMoveInput)
                {
                    player.ChangeEvadeState(PlayerAnimationType.EvadeFront);
                }
                else
                {
                    player.ChangeEvadeState(PlayerAnimationType.EvadeBack);
                }

                return;
            }

            if (player.Input.HasMoveInput)
            {
                player.RotateToMoveDirection();
                player.StateMachine.ChangeState(StateEnum.Walk);
                return;
            }
        }

        if (!player.IsAnimFinished(PlayerAnimationType.NormalHit))
        {
            return;
        }

        player.StateMachine.ChangeState(StateEnum.Idle);
    }

    public void OpenHitInterruptWindow()
    {
        canInterrupt = true;
    }
}