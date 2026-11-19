using UnityEngine;

public class DeathState : IState
{
    private PlayerController player;

    public DeathState(PlayerController playerController)
    {
        this.player = playerController;
    }

    public void OnEnter()
    {
        PlayerInput.SetCanControl(false);
        player.PlayAnim(PlayerAnimationType.Death);
    }

    public void OnExit()
    {
        PlayerInput.SetCanControl(true);
    }

    public void OnUpdate()
    {

    }
}
