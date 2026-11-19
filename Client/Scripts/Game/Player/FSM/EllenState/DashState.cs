using UnityEngine;

public class DashState : IState
{
    private enum DashPhase//冲刺阶段
    {
        DashStart,//冲刺开始
        Dashing,//正在冲刺
        DashEnd,//冲刺结束
        DashEvade,//冲刺闪避
        DashAttack1,//冲刺第一段攻击
        DashAttack2,//冲刺第二段攻击
        DashAttackEnd,//冲刺攻击结束
    }
    private PlayerController player;
    private DashPhase curPhase;
    private PlayerAnimationType curAnim;

    private bool hasNextDashEvadeInput;//是否输入了下一次冲刺闪避
    private bool connectNormalAttack3;//是否连接普通攻击第三段
    private float dashEvadeTime;//冲刺闪避时间

    private const float DashEvadeInterruptTime = 0.55f;

    public DashState(PlayerController player)
    {
        this.player = player;
    }

    public void OnEnter()
    {
        connectNormalAttack3 = false;
        dashEvadeTime = 0;
        hasNextDashEvadeInput = false;

        ChangePhase(DashPhase.DashStart);
    }

    public void OnExit()
    {
        connectNormalAttack3 = false;
        dashEvadeTime = 0;
        hasNextDashEvadeInput = false;
    }

    public void OnUpdate()
    {
        switch (curPhase)
        {
            case DashPhase.DashStart:
                UpdateDashStart();
                break;

            case DashPhase.Dashing:
                UpdateDashing();
                break;

            case DashPhase.DashEnd:
                UpdateDashEnd();
                break;

            case DashPhase.DashEvade:
                UpdateDashEvade();
                break;

            case DashPhase.DashAttack1:
                UpdateDashAttack1();
                break;

            case DashPhase.DashAttack2:
                UpdateDashAttack2();
                break;

            case DashPhase.DashAttackEnd:
                UpdateDashAttackEnd();
                break;
        }
    }
    private void UpdateDashStart()//更新冲刺开始
    {
        if (!player.IsAnimFinished(curAnim))
        {
            return;
        }

        if (player.Input.HasMoveInput)
        {
            ChangePhase(DashPhase.Dashing);
        }
        else
        {
            ChangePhase(DashPhase.DashEnd);
        }
    }

    private void UpdateDashing()//更新冲刺
    {
        if (player.Input.IsExSkillPressed)
        {
            player.ChangeExSkillState();
            return;
        }

        if (player.Input.IsEvadePressed)
        {
            ChangePhase(DashPhase.DashEvade);
            return;
        }

        if (player.Input.IsAttackPressed)
        {
            ChangePhase(DashPhase.DashAttack1);
            return;
        }

        if (player.Input.IsSkillPressed)
        {
            player.ChangeSkillState();
            return;
        }

        if (!player.Input.HasMoveInput)
        {
            ChangePhase(DashPhase.DashEnd);
            return;
        }

        player.RotateToMoveDirection();
    }

    private void UpdateDashEnd()//更新冲刺结束
    {
        if (TryInterruptDashEnd())
        {
            return;
        }

        if (player.IsAnimFinished(curAnim))
        {
            player.StateMachine.ChangeState(StateEnum.Idle);
        }
    }

    private void UpdateDashEvade()//更新冲刺闪避
    {
        dashEvadeTime += Time.deltaTime;

        if (player.Input.IsEvadePressed)
        {
            hasNextDashEvadeInput = true;//提前记录下一次闪避输入
        }

        if (dashEvadeTime < DashEvadeInterruptTime)
        {
            return;
        }

        if (hasNextDashEvadeInput)
        {
            ChangePhase(DashPhase.DashEvade);//连续播放冲刺闪避
            return;
        }

        if (player.Input.HasMoveInput)
        {
            player.RotateToMoveDirection();
            ChangePhase(DashPhase.Dashing);
            return;
        }

        if (player.IsAnimFinished(curAnim))
        {
            ChangePhase(DashPhase.DashEnd);
        }
    }

    private void UpdateDashAttack1()//更新冲刺第一段攻击
    {

        if (!player.IsAnimFinished(curAnim))
        {
            return;
        }

        ChangePhase(DashPhase.DashAttack2);

    }

    private void UpdateDashAttack2()//更新冲刺第二段攻击
    {
        if (player.Input.IsAttackPressed)
        {
            connectNormalAttack3 = true;
        }

        if (!player.IsAnimFinished(curAnim))
        {
            return;
        }

        if (connectNormalAttack3)
        {
            player.ChangeAttackState(false, 3);
        }
        else
        {
            ChangePhase(DashPhase.DashAttackEnd);
        }
    }

    private void UpdateDashAttackEnd()//更新冲刺攻击结束
    {
        if (TryInterruptDashAttackEnd())
        {
            return;
        }

        if (player.IsAnimFinished(curAnim))
        {
            player.StateMachine.ChangeState(StateEnum.Idle);
        }
    }

    private bool TryInterruptDashEnd()//检查冲刺结束动画是否被打断
    {
        if (player.Input.IsExSkillPressed)
        {
            player.ChangeExSkillState();
            return true;
        }

        if (player.Input.IsEvadePressed)
        {
            player.ChangeEvadeState(PlayerAnimationType.EvadeBack);
            return true;
        }

        if (player.Input.IsSkillPressed)
        {
            player.ChangeSkillState();
            return true;
        }

        if (player.Input.IsAttackPressed)
        {
            player.ChangeAttackState(false);
            return true;
        }

        if (player.Input.HasMoveInput)
        {
            player.RotateToMoveDirection();
            player.StateMachine.ChangeState(StateEnum.Walk);
            return true;
        }

        return false;
    }
    private bool TryInterruptDashAttackEnd()//检查冲刺攻击结束是否被打断
    {
        if (player.Input.IsExSkillPressed)
        {
            player.ChangeExSkillState();
            return true;
        }

        if (player.Input.IsEvadePressed)
        {
            player.ChangeEvadeState(PlayerAnimationType.EvadeBack);
            return true;
        }

        if (player.Input.IsSkillPressed)
        {
            player.ChangeSkillState();
            return true;
        }

        if (player.Input.IsAttackPressed)
        {
            player.ChangeAttackState(false);
            return true;
        }

        if (player.Input.HasMoveInput)
        {
            player.RotateToMoveDirection();
            player.StateMachine.ChangeState(StateEnum.Walk);
            return true;
        }

        return false;
    }
    private void ChangePhase(DashPhase nextPhase)
    {
        curPhase = nextPhase;
        switch (curPhase)
        {
            case DashPhase.DashStart:
                curAnim = PlayerAnimationType.DashStart;
                break;
            case DashPhase.Dashing:
                curAnim = PlayerAnimationType.Dash;
                break;
            case DashPhase.DashEnd:
                curAnim = PlayerAnimationType.DashEnd;
                break;
            case DashPhase.DashEvade:
                dashEvadeTime = 0f;//重新计算本次闪避时间
                hasNextDashEvadeInput = false;//清除上一次缓存
                curAnim = PlayerAnimationType.DashEvade;
                break;
            case DashPhase.DashAttack1:
                curAnim = PlayerAnimationType.DashAttack1;
                break;
            case DashPhase.DashAttack2:
                curAnim = PlayerAnimationType.DashAttack2;
                break;
            case DashPhase.DashAttackEnd:
                curAnim = PlayerAnimationType.DashAttackEnd;
                break;
        }
        player.PlayAnim(curAnim);
    }
}
