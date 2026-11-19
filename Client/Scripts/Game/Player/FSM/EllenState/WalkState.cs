using System.Diagnostics;
using UnityEngine;

public class WalkState : IState
{
    private enum WalkPhase//移动状态
    {
        WalkStart,
        WalkStartEnd,
        Walking,
        WalkEnd,
    }

    private PlayerController player;
    private WalkPhase currentPhase;

    public WalkState(PlayerController player)
    {
        this.player = player;
    }

    public void OnEnter()
    {
        ChangePhase(WalkPhase.WalkStart);//切换到WalkStart动画
    }

    public void OnUpdate()
    {
        if (player.Input.IsExSkillPressed)//按下大招
        {
            player.ChangeExSkillState();
            return;
        }
        if (player.Input.IsEvadePressed)//移动状态按下闪避键
        {
            if (currentPhase == WalkPhase.WalkStartEnd ||
                currentPhase == WalkPhase.WalkEnd)
            {
                player.ChangeEvadeState(PlayerAnimationType.EvadeBack);//结束阶段使用后闪
            }
            else
            {
                player.ChangeEvadeState(PlayerAnimationType.EvadeFront);//开始和移动阶段使用前闪
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
            player.ChangeAttackState(false);
            return;
        }
        if (player.Input.HasMoveInput)//按下移动键
        {
            player.RotateToMoveDirection();//对应更新朝向
        }

        switch (currentPhase)//根据动画状态进行更新
        {
            case WalkPhase.WalkStart://开始移动
                UpdateStart();
                break;

            case WalkPhase.Walking://移动中
                UpdateLoop();
                break;

            case WalkPhase.WalkStartEnd://开始移动结束
                UpdateStartEnd();
                break;

            case WalkPhase.WalkEnd://移动结束
                UpdateEnd();
                break;
        }
    }

    public void OnExit()
    {
    }
    private void UpdateStart()//正在播放动画Walk_Start
    {
        if (player.Input.HasMoveInput == false)// 如果松开移动键：
        {
            ChangePhase(WalkPhase.WalkStartEnd);// 切换到WalkStartEnd
            return;
        }
        if (player.IsAnimFinished(PlayerAnimationType.WalkStart) && player.Input.HasMoveInput)// 如果Walk_Start播放完成，并且还按着移动键：
        {
            ChangePhase(WalkPhase.Walking);//切换到Walking
        }
    }
    private void UpdateLoop()
    {
        if (player.Input.HasMoveInput == false)//如果松开移动键：
        {
            ChangePhase(WalkPhase.WalkEnd);//切换到End
        }
    }
    private void UpdateStartEnd()
    {
        if (player.Input.HasMoveInput)// 再次按下移动键，立即打断结束动画
        {
            ChangePhase(WalkPhase.WalkStart);
            return;
        }
        if (player.IsAnimFinished(PlayerAnimationType.WalkStartEnd))//如果Walk_Start_End播放完成
        {
            player.StateMachine.ChangeState(StateEnum.Idle);//切换到IdleState
        }
    }
    private void UpdateEnd()
    {
        if (player.Input.HasMoveInput)
        {
            ChangePhase(WalkPhase.WalkStart);
            return;
        }
        if (player.IsAnimFinished(PlayerAnimationType.WalkEnd))//如果Walk_End播放完成
        {
            player.StateMachine.ChangeState(StateEnum.Idle);//切换到IdleState
        }
    }

    private void ChangePhase(WalkPhase nextPhase)
    {
        currentPhase = nextPhase;

        switch (currentPhase)
        {
            case WalkPhase.WalkStart:
                player.PlayAnim(PlayerAnimationType.WalkStart);
                break;

            case WalkPhase.Walking:
                player.PlayAnim(PlayerAnimationType.Walk);
                break;

            case WalkPhase.WalkStartEnd:
                player.PlayAnim(PlayerAnimationType.WalkStartEnd);
                break;

            case WalkPhase.WalkEnd:
                player.PlayAnim(PlayerAnimationType.WalkEnd);
                break;
        }
    }
}