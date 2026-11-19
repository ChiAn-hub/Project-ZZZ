using UnityEngine;

public class HitFlyState : IState
{
    private PlayerController player;
    public HitFlyState(PlayerController player)
    {
        this.player = player;
    }

    public void OnEnter()
    {
        player.PlayAnim(PlayerAnimationType.HitFly);
    }

    public void OnExit()
    {

    }

    public void OnUpdate()
    {
        if (!player.IsAnimFinished(PlayerAnimationType.HitFly))
        {
            return;
        }

        player.StateMachine.ChangeState(StateEnum.Idle);
    }
}
