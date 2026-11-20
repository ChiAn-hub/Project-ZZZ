using UnityEngine;

public class EnemyDeathState : IEnemyState
{
    private EnemyManage enemy;
    public EnemyDeathState(EnemyManage enemy)
    {
        this.enemy = enemy;
    }
    public void OnEnter()
    {
        enemy.ClearPlayerTarget();

        enemy.PlayAnim(EnemyAnimType.Death);
    }

    public void OnExit()
    {

    }

    public void OnUpdate()
    {
        if (!enemy.IsAnimFinished(EnemyAnimType.Death))
        {
            return;
        }

        enemy.StateMachine.SetStateResult(EnemyStateResult.Success);//死亡动画完成

        enemy.gameObject.SetActive(false);//回收到对象池
    }
}
