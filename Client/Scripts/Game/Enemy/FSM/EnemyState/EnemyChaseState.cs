using UnityEngine;

public class EnemyChaseState : IEnemyState
{
    private enum ChasePhase//追击阶段
    {
        WalkStart,//播放起步动画
        Walking,//持续追击
    }
    private EnemyManage enemy;
    private ChasePhase curPhase;//当前追击阶段
    public EnemyChaseState(EnemyManage enemy)
    {
        this.enemy = enemy;
    }
    public void OnEnter()
    {
        enemy.LookAtPlayer();
        ChangePhase(ChasePhase.WalkStart);
    }

    public void OnExit()
    {

    }

    public void OnUpdate()
    {

        enemy.LookAtPlayer();//追击时持续朝向玩家

        switch (curPhase)
        {
            case ChasePhase.WalkStart:
                UpdateWalkStart();
                break;

            case ChasePhase.Walking:
                UpdateWalking();
                break;
        }
    }
    private void UpdateWalkStart()//更新追击起步阶段
    {
        if (enemy.IsAnimFinished(EnemyAnimType.WalkForwardStart))
        {
            ChangePhase(ChasePhase.Walking);//起步结束后持续行走
        }
    }

    private void UpdateWalking()
    {

    }
    private void ChangePhase(ChasePhase phase)
    {
        curPhase = phase;
        switch (curPhase)
        {
            case ChasePhase.WalkStart:
                enemy.PlayAnim(EnemyAnimType.WalkForwardStart);
                break;

            case ChasePhase.Walking:
                enemy.PlayAnim(EnemyAnimType.WalkForward);
                break;
        }
    }
}
