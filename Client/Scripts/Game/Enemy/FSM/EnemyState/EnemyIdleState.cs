using UnityEngine;

public class EnemyIdleState : IEnemyState
{
    private EnemyManage enemy;//对应的怪物信息
    private float idleTime;//已经待机的时间
    private float targetIdleTime;//本次需要待机的时间
    public EnemyIdleState(EnemyManage enemy)
    {
        this.enemy = enemy;
    }
    public void OnEnter()//进入待机状态
    {
        enemy.PlayAnim(EnemyAnimType.Idle);//播放待机动画
        idleTime = 0;//重置待机时间
        targetIdleTime = Random.Range(enemy.EnemyModel.MinIdleTime, enemy.EnemyModel.MaxIdleTime);//随机待机时间
    }

    public void OnExit()
    {
        idleTime = 0;
    }

    public void OnUpdate()
    {
        idleTime += Time.deltaTime;
        if (idleTime < targetIdleTime)//如果已经达到待机时间
        {
            return;
        }
        enemy.StateMachine.SetStateResult(EnemyStateResult.Success);//设置状态结果为成功
    }
}
