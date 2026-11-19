using UnityEngine;

public class EnterStartState : IState
{
    private PlayerController player;
    public EnterStartState(PlayerController player)
    {
        this.player = player;
    }
    public void OnEnter()
    {
        player.PlayAnim(PlayerAnimationType.EnterStart);
    }

    public void OnExit()
    {

    }

    public void OnUpdate()
    {
        if (player.IsAnimFinished(PlayerAnimationType.EnterStart))
        {
            player.StateMachine.ChangeState(StateEnum.Idle);
        }
    }
}
