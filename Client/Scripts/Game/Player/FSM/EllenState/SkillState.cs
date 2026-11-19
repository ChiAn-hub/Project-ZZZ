using UnityEngine;

public class SkillState : IState
{
    private enum SkillPhase//技能阶段
    {
        Skill,//正在播放技能
        SkillEnd,//正在播放技能结束动画
    }
    private PlayerController player;
    private SkillPhase curPhase;
    private PlayerAnimationType curAnim;

    private int skillIndex;//当前第几段技能
    private bool hasNextSkillInput;//是否输入了下一段技能
    private bool canEnterNextSkill;//是否可以进入下一段技能
    public SkillState(PlayerController player)
    {
        this.player = player;
    }
    public void OnEnter()
    {
        skillIndex = 1;
        hasNextSkillInput = false;
        canEnterNextSkill = false;

        PlayCurSkill();
    }

    public void OnExit()
    {
        skillIndex = 1;
        hasNextSkillInput = false;
        canEnterNextSkill = false;
    }


    public void OnUpdate()
    {
        if (curPhase == SkillPhase.SkillEnd)
        {
            if (TryInterruptSkillEnd())//如果被打断
            {
                return;
            }
        }
        if (curPhase == SkillPhase.Skill && skillIndex < 3 && player.Input.IsSkillPressed)
        {
            hasNextSkillInput = true;//记录下一段技能输入
        }
        if (curPhase == SkillPhase.Skill && skillIndex < 3 && hasNextSkillInput && canEnterNextSkill)
        {
            skillIndex++;
            hasNextSkillInput = false;
            PlayCurSkill();
            return;
        }
        if (!player.IsAnimFinished(curAnim))
        {
            return;
        }
        switch (curPhase)
        {
            case SkillPhase.Skill:
                UpdateSkill();
                break;

            case SkillPhase.SkillEnd:
                player.StateMachine.ChangeState(StateEnum.Idle);
                break;
        }
    }

    private bool TryInterruptSkillEnd()//打断技能结束
    {
        if (player.Input.IsExSkillPressed)//按下大招
        {
            player.ChangeExSkillState();
            return true;
        }
        if (player.Input.IsAttackPressed)
        {
            player.ChangeAttackState(false);
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
        if (player.Input.HasMoveInput)
        {
            player.RotateToMoveDirection();
            player.StateMachine.ChangeState(StateEnum.Walk);
            return true;
        }
        return false;
    }

    private void PlaySkillEnd()//播放对应动画结尾
    {
        curPhase = SkillPhase.SkillEnd;
        switch (skillIndex)
        {
            case 1:
                curAnim = PlayerAnimationType.Skill1End;
                break;
            case 2:
                curAnim = PlayerAnimationType.Skill2End;
                break;
            case 3:
                curAnim = PlayerAnimationType.Skill3End;
                break;
        }
        player.PlayAnim(curAnim);
    }

    private void PlayCurSkill()//播放当前技能动画
    {
        canEnterNextSkill = false;
        curPhase = SkillPhase.Skill;
        switch (skillIndex)
        {
            case 1:
                curAnim = PlayerAnimationType.Skill1;
                break;
            case 2:
                curAnim = PlayerAnimationType.Skill2;
                break;
            case 3:
                curAnim = PlayerAnimationType.Skill3;
                break;
        }
        player.RotateToMoveDirection();
        player.PlayAnim(curAnim);
    }

    private void UpdateSkill()//当技能播放结束
    {
        if (skillIndex < 3 && hasNextSkillInput)
        {
            skillIndex++;
            hasNextSkillInput = false;
            PlayCurSkill();
        }
        else
        {
            PlaySkillEnd();
        }
    }

    public void OpenSkillComboWindow()
    {
        if (curPhase != SkillPhase.Skill)
        {
            return;
        }

        if (skillIndex >= 3)
        {
            return;
        }

        canEnterNextSkill = true;
    }
}
