using UnityEngine;

public class EnemyHitState : IEnemyState
{
    private EnemyManage enemy;
    public EnemyHitState(EnemyManage enemy)
    {
        this.enemy = enemy;
    }
    public void OnEnter()
    {
        enemy.PlayAnim(EnemyAnimType.Hit);
    }

    public void OnExit()
    {

    }

    public void OnUpdate()
    {
        if (!enemy.IsAnimFinished(EnemyAnimType.Hit))
        {
            return;
        }
        if (enemy.FindPlayer())//受伤结束后发现玩家
        {
            enemy.StateMachine.ChangeState(EnemyStateType.Chase);//追击
        }
        else
        {
            enemy.StateMachine.ChangeState(EnemyStateType.Idle);//待机
        }
    }
}
