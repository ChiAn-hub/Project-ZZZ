using UnityEngine;

public class EvadeState : IState
{
    private PlayerController player;//获取玩家控制器
    private PlayerAnimationType evadeAnim;//记录本次闪避动画
    private float evadeTime;//记录闪避播放时间

    private const float MoveInterruptTime = 0.52f;//有移动输入时，可以切换状态的时间
    private const float IdleTransitionTime = 3.0f;//没有移动输入时，进入待机状态的时间

    public EvadeState(PlayerController player)
    {
        this.player = player;
    }

    public void OnEnter()
    {
        evadeAnim = player.CurAnim;//获取要播放的闪避动画
        evadeTime = 0f;//重置闪避计时
        player.PlayAnim(evadeAnim);//播放闪避动画
    }

    public void OnExit()
    {
        evadeTime = 0f;//退出闪避时重置计时
    }

    public void OnUpdate()
    {
        evadeTime += Time.deltaTime;

        if (evadeTime >= MoveInterruptTime && player.Input.HasMoveInput)
        {
            player.StateMachine.ChangeState(StateEnum.Run);//有移动输入则进入跑步状态
            return;
        }

        if (evadeTime >= IdleTransitionTime)
        {
            player.StateMachine.ChangeState(StateEnum.Idle);//没有移动输入则进入待机状态
        }
    }
}