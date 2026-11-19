using UnityEngine;

public class IdleState : IState//待机状态
{
    private PlayerController player;
    private float idleTime;//待机时间
    private bool isPlayingAfk;//是否播放待机动画
    private const float AfkTime = 8f;//待机时间
    public IdleState(PlayerController player)
    {
        this.player = player;
    }
    public void OnEnter()
    {
        player.PlayAnim(PlayerAnimationType.Idle);//播放待机动画
        idleTime = 0;//重置待机时间
        isPlayingAfk = false;//未进行待机动画
    }

    public void OnExit()
    {
        idleTime = 0f;//重置待机时间
        isPlayingAfk = false;//未进行待机动画
    }

    public void OnUpdate()
    {
        if (player.Input.IsExSkillPressed)//按下大招
        {
            player.ChangeExSkillState();
            return;
        }
        if (player.Input.IsEvadePressed)//待机状态按下闪避键
        {
            player.ChangeEvadeState(PlayerAnimationType.EvadeBack);
            return;
        }

        if (player.Input.IsSkillPressed)//待机状态按下技能键
        {
            player.ChangeSkillState();
            return;
        }

        if (player.Input.IsAttackPressed)//待机状态按下普通攻击
        {
            player.ChangeAttackState(false);
            return;
        }

        if (player.Input.HasMoveInput)//检测是否有移动输入
        {
            player.StateMachine.ChangeState(StateEnum.Walk);
            return;
        }

        if (!isPlayingAfk)//未进行待机动画
        {
            UpdateIdleTimer();//更新待机计时器
        }
        else
        {
            CheckAfkAnimationFinished();//检查待机动画是否结束
        }
    }
    private void UpdateIdleTimer()//更新待机计时器
    {
        idleTime += Time.deltaTime;
        if (idleTime >= AfkTime)
        {
            player.PlayAnim(PlayerAnimationType.IdleAFK);//播放待机动画
            isPlayingAfk = true;//进行待机动画
        }
    }
    private void CheckAfkAnimationFinished()//检查待机动画是否结束
    {
        if (player.IsAnimFinished(PlayerAnimationType.IdleAFK))
        {
            player.PlayAnim(PlayerAnimationType.Idle);//重新播放待机动画
            isPlayingAfk = false;//结束AFK动画
            idleTime = 0;//重新计算待机时间
        }
    }
}
