using UnityEngine;

public class AttackState : IState
{
    private enum AttackPhase//普通攻击阶段
    {
        Attacking,//正在播放当前段攻击
        Attack3Back,//第三段的后半段动画
        AttackEnd,//当前连招结束动画
    }
    private AttackPhase curPhase;
    private PlayerAnimationType curAnim;//当前播放的动画

    private int comboIndex;//当前是第几段攻击
    private bool hasNextAttackInput;//是否输入了下一段攻击
    private bool isMoveAttack;//是否使用跑动攻击
    private bool isNearAttack;//第三段是否使用近距离版本
    private bool canEnterNextAtk;//是否可以进入下一段动画
    private PlayerController player;

    public AttackState(PlayerController playerController)
    {
        this.player = playerController;
    }

    public void OnEnter()
    {
        comboIndex = player.AttackStartIndex;
        hasNextAttackInput = false;
        isMoveAttack = player.IsMoveAttack;
        isNearAttack = false;
        canEnterNextAtk = false;

        PlayCurAtk();
    }

    public void OnExit()
    {
        isNearAttack = false;
        comboIndex = 0;
        hasNextAttackInput = false;
        canEnterNextAtk = false;
    }

    public void OnUpdate()
    {
        if (curPhase == AttackPhase.AttackEnd)
        {
            if (TryInterruptAttackEnd())//结束动画被其他操作打断
            {
                return;
            }
        }
        if (curPhase == AttackPhase.Attacking && comboIndex < 3 && player.Input.IsAttackPressed)//如果当前阶段是攻击中,并且不是第三段攻击,而且有攻击输入
        {
            hasNextAttackInput = true;
        }

        if (curPhase == AttackPhase.Attacking && comboIndex < 3 && hasNextAttackInput && canEnterNextAtk)
        {
            comboIndex++;
            hasNextAttackInput = false;
            PlayCurAtk();
            return;
        }

        if (!player.IsAnimFinished(curAnim))//如果当前动画未播放完
        {
            return;
        }
        switch (curPhase)
        {
            case AttackPhase.Attacking:
                UpdateAttacking();
                break;

            case AttackPhase.Attack3Back:
                PlayAttackEnd();
                break;

            case AttackPhase.AttackEnd:
                player.StateMachine.ChangeState(StateEnum.Idle);
                break;
        }
    }

    private void UpdateAttacking()//当前动画播放完毕
    {
        if (comboIndex == 3)
        {
            PlayAttack3Back();
            return;
        }
        if (hasNextAttackInput)
        {
            comboIndex++;
            hasNextAttackInput = false;
            PlayCurAtk();
        }
        else
        {
            PlayAttackEnd();
        }
    }

    private void PlayCurAtk()
    {
        curPhase = AttackPhase.Attacking;
        canEnterNextAtk = false;
        switch (comboIndex)
        {
            case 1:
                if (isMoveAttack)
                {
                    curAnim = PlayerAnimationType.MoveNormalAttack1;
                }
                else
                {
                    curAnim = PlayerAnimationType.NormalAttack1;
                }
                break;

            case 2:
                if (isMoveAttack)
                {
                    curAnim = PlayerAnimationType.MoveNormalAttack2;
                }
                else
                {
                    curAnim = PlayerAnimationType.NormalAttack2;
                }
                break;
            case 3:
                isNearAttack = player.HasNearEnemy();

                if (isMoveAttack)
                {
                    if (isNearAttack)
                    {
                        curAnim = PlayerAnimationType.MoveNormalAttack3Near;
                    }
                    else
                    {
                        curAnim = PlayerAnimationType.MoveNormalAttack3;
                    }
                }
                else
                {
                    if (isNearAttack)
                    {
                        curAnim = PlayerAnimationType.NormalAttack3Near;
                    }
                    else
                    {
                        curAnim = PlayerAnimationType.NormalAttack3;
                    }
                }
                break;
        }
        player.RotateToMoveDirection();
        player.PlayAnim(curAnim);
    }

    private void PlayAttack3Back()//第三段后半段动画
    {
        curPhase = AttackPhase.Attack3Back;
        if (isMoveAttack)
        {
            if (isNearAttack)
            {
                curAnim = PlayerAnimationType.MoveNormalAttack3BackNear;
            }
            else
            {
                curAnim = PlayerAnimationType.MoveNormalAttack3Back;
            }
        }
        else
        {
            if (isNearAttack)
            {
                curAnim = PlayerAnimationType.NormalAttack3BackNear;
            }
            else
            {
                curAnim = PlayerAnimationType.NormalAttack3Back;
            }
        }
        player.PlayAnim(curAnim);
    }

    private void PlayAttackEnd()//对应的结束动画
    {
        curPhase = AttackPhase.AttackEnd;

        switch (comboIndex)
        {
            case 1:
                if (isMoveAttack)
                {
                    curAnim = PlayerAnimationType.MoveNormalAttack1End;
                }
                else
                {
                    curAnim = PlayerAnimationType.NormalAttack1End;
                }
                break;

            case 2:
                curAnim = PlayerAnimationType.NormalAttack2End;
                break;

            case 3:
                if (isMoveAttack)
                {
                    curAnim = PlayerAnimationType.MoveNormalAttack3End;
                }
                else
                {
                    curAnim = PlayerAnimationType.NormalAttack3End;
                }
                break;
        }

        player.PlayAnim(curAnim);
    }

    private bool TryInterruptAttackEnd()//检查攻击结束动画是否被打断
    {
        if (player.Input.IsExSkillPressed)//按下大招
        {
            player.ChangeExSkillState();
            return true;
        }
        if (player.Input.IsEvadePressed)//End时按下闪避
        {
            player.ChangeEvadeState(PlayerAnimationType.EvadeBack);
            return true;
        }

        if (player.Input.IsAttackPressed)//End时再次按下普通攻击
        {
            bool useMoveAttack = isMoveAttack && player.Input.HasMoveInput;
            player.ChangeAttackState(useMoveAttack);//从第一段重新开始
            return true;
        }

        if (player.Input.HasMoveInput)//End时输入移动
        {
            player.RotateToMoveDirection();
            player.StateMachine.ChangeState(StateEnum.Walk);
            return true;
        }
        if (player.Input.IsSkillPressed)//技能输入
        {
            player.ChangeSkillState();
            return true;
        }
        return false;
    }

    public void OpenAttackComboWindow()//接口用于外接修改canEnterNextAtk
    {
        if (curPhase != AttackPhase.Attacking)
        {
            return;
        }
        if (comboIndex >= 3)
        {
            return;
        }

        canEnterNextAtk = true;
    }
}
