using System.Runtime.CompilerServices;
using UnityEngine;


public class RunState : IState
{
    private enum RunPhase
    {
        Running,//正在跑步
        TurnBack,//大角度转向
        RunEnd,//结束跑步
    }
    private PlayerController player;
    private RunPhase curPhase;
    private float noInputTime;//记录没有移动输入的时间

    private const float TurnBackAngle = 120f;//触发转身动画的角度
    private const float TurnBackAnimationAngle = 135f;//动画原本的旋转角度
    private const float RunEndDelay = 0.15f;//允许玩家切换方向的时间
    private float runTime;//持续跑步时间
    private const float DashEnterTime = 3f;//进入冲刺需要的跑步时间

    public RunState(PlayerController player)
    {
        this.player = player;
    }
    public void OnEnter()
    {
        noInputTime = 0f;
        runTime = 0f;
        ChangePhase(RunPhase.Running);
    }

    public void OnExit()
    {
        noInputTime = 0f;
        runTime = 0f;
        player.TurnBackRotationScale = 1f;
    }

    public void OnUpdate()
    {
        if (player.Input.IsExSkillPressed)//按下大招
        {
            player.ChangeExSkillState();
            return;
        }

        if (player.Input.IsEvadePressed)//如果玩家按下闪避键
        {
            if (curPhase == RunPhase.RunEnd)
            {
                player.ChangeEvadeState(PlayerAnimationType.EvadeBack);//跑步结束阶段使用后闪
            }
            else
            {
                player.ChangeEvadeState(PlayerAnimationType.EvadeFront);//跑步阶段使用前闪
            }

            return;
        }
        if (player.Input.IsSkillPressed)
        {
            player.ChangeSkillState();
            return;
        }
        if (player.Input.IsAttackPressed)
        {
            player.ChangeAttackState(true);
            return;
        }
        switch (curPhase)
        {
            case RunPhase.Running:
                UpdateRunning();
                break;

            case RunPhase.RunEnd:
                UpdateRunEnd();
                break;
            case RunPhase.TurnBack:
                UpdateTurnBack();
                break;
        }
    }
    private void UpdateRunning()//更新跑步
    {
        if (!player.Input.HasMoveInput)//当前没有移动输入
        {
            runTime = 0;
            noInputTime += Time.deltaTime;//记录未输入时间

            if (noInputTime >= RunEndDelay)//超过指定时间后
            {
                ChangePhase(RunPhase.RunEnd);//改变状态为跑步结束
            }

            return;
        }

        noInputTime = 0f;//重新检测到输入，重置计时

        runTime += Time.deltaTime;

        if (runTime >= DashEnterTime)
        {
            player.StateMachine.ChangeState(StateEnum.Dash);
            return;
        }

        Vector3 moveDirection = player.GetMoveDirection();//获取新的移动方向

        float angle = Vector3.Angle(player.transform.forward, moveDirection);//计算当前朝向和输入方向的夹角

        if (angle > TurnBackAngle)//转向角度大于120度
        {
            player.TurnBackRotationScale = angle / TurnBackAnimationAngle;//计算动画旋转倍率

            ChangePhase(RunPhase.TurnBack);//播放转身动画
            return;
        }

        player.RotateToMoveDirection();//普通角度直接转向
    }
    private void UpdateRunEnd()//更新停止跑步
    {
        if (player.Input.HasMoveInput)//动画播放时,按下移动键,则进入Walk状态
        {
            player.RotateToMoveDirection();//更新角色朝向
            player.StateMachine.ChangeState(StateEnum.Walk);//进入行走状态
            return;
        }
        if (player.IsAnimFinished(PlayerAnimationType.RunEnd))
        {
            player.StateMachine.ChangeState(StateEnum.Idle);
        }
    }
    private void ChangePhase(RunPhase nextPhase)
    {
        curPhase = nextPhase;
        switch (curPhase)
        {
            case RunPhase.Running:
                player.PlayAnim(PlayerAnimationType.Run);
                break;
            case RunPhase.RunEnd:
                player.PlayAnim(PlayerAnimationType.RunEnd);
                break;
            case RunPhase.TurnBack:
                player.PlayAnim(PlayerAnimationType.TurnBack);
                break;
        }
    }
    private void UpdateTurnBack()//更新转身状态
    {
        if (!player.IsAnimFinished(PlayerAnimationType.TurnBack))//转身动画未播放完
        {
            return;
        }

        player.TurnBackRotationScale = 1f;//恢复默认旋转倍率

        if (player.Input.HasMoveInput)//转身结束后还有移动输入
        {
            ChangePhase(RunPhase.Running);//继续跑步
        }
        else
        {
            ChangePhase(RunPhase.RunEnd);//没有输入则结束跑步
        }
    }
}
