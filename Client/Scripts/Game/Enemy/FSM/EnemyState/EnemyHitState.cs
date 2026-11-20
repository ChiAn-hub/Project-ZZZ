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
        enemy.StateMachine.SetStateResult(EnemyStateResult.Success);
    }
}
