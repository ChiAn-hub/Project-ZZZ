using UnityEngine;

public class EnemyAttackState : IEnemyState
{
    private EnemyManage enemy;
    private EnemyAnimType curAnimType;
    private EnemyAnimType[] attackAnimations =
    {
      EnemyAnimType.Attack1,
      EnemyAnimType.Attack2,
      EnemyAnimType.Attack3,
      EnemyAnimType.Attack4,
      EnemyAnimType.Attack5
    };
    public EnemyAttackState(EnemyManage enemy)
    {
        this.enemy = enemy;
    }
    public void OnEnter()
    {
        enemy.LookAtPlayer();//看向玩家
        int random = Random.Range(0, attackAnimations.Length);//随机攻击动画
        curAnimType = attackAnimations[random];
        enemy.PlayAnim(curAnimType);
    }

    public void OnExit()
    {

    }

    public void OnUpdate()
    {
        if (!enemy.IsAnimFinished(curAnimType))
        {
            return;
        }
        enemy.StateMachine.SetStateResult(EnemyStateResult.Success);
    }
}
