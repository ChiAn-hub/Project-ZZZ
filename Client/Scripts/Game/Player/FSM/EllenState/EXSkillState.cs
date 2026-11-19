using UnityEngine;

public class EXSkillState : IState
{
    private enum EXSkillPhase//大招阶段
    {
        EXSkill,//大招第一段
        EXSkillEnd,//大招结束
    }
    private PlayerController player;

    private EXSkillPhase curPhase;
    private PlayerAnimationType curAnim;

    public EXSkillState(PlayerController player)
    {
        this.player = player;
    }
    public void OnEnter()
    {
        ChangePhase(EXSkillPhase.EXSkill);
    }

    public void OnExit()
    {

    }

    public void OnUpdate()
    {
        if (curPhase == EXSkillPhase.EXSkillEnd)
        {
            if (TryInterruptEXSkillEnd())//大招结束动画被打断
            {
                return;
            }
        }

        if (!player.IsAnimFinished(curAnim))
        {
            return;
        }

        switch (curPhase)
        {

            case EXSkillPhase.EXSkill:
                ChangePhase(EXSkillPhase.EXSkillEnd);
                break;

            case EXSkillPhase.EXSkillEnd:
                player.StateMachine.ChangeState(StateEnum.Idle);
                break;
        }
    }

    private void ChangePhase(EXSkillPhase nextPhase)
    {
        curPhase = nextPhase;
        switch (curPhase)
        {
            case EXSkillPhase.EXSkill:
                curAnim = PlayerAnimationType.EXSkill;
                break;
            case EXSkillPhase.EXSkillEnd:
                curAnim = PlayerAnimationType.EXSkillEnd;
                break;
        }
        player.PlayAnim(curAnim);
    }

    private bool TryInterruptEXSkillEnd()//检查大招结束动画是否被打断
    {
        if (player.Input.IsEvadePressed)//按下闪避
        {
            player.ChangeEvadeState(PlayerAnimationType.EvadeBack);
            return true;
        }

        if (player.Input.IsSkillPressed)//按下技能
        {
            player.ChangeSkillState();
            return true;
        }

        if (player.Input.IsAttackPressed)//按下普通攻击
        {
            player.ChangeAttackState(false);
            return true;
        }

        if (player.Input.HasMoveInput)//输入移动
        {
            player.RotateToMoveDirection();
            player.StateMachine.ChangeState(StateEnum.Walk);
            return true;
        }

        return false;
    }
}
