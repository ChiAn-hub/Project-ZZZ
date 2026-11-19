using UnityEngine;

public class EnemyCooldownState : IEnemyState
{
    private enum CooldownPhase//冷却期间的移动阶段
    {
        WalkStart,//移动起步
        Walking,//持续移动
        Waiting,//玩家离开后原地等待
    }
    private EnemyManage enemy;
    private CooldownPhase curPhase;//当前的阶段
    private float cooldownTime;//已经冷却的时间
    private bool isWalkForward;//本次冷却是否向前走
    public EnemyCooldownState(EnemyManage enemy)
    {
        this.enemy = enemy;
    }
    public void OnEnter()
    {
        cooldownTime = 0;
        isWalkForward = Random.value < 0.5f;//随机选择前移或后移
        enemy.LookAtPlayer();
        ChangePhase(CooldownPhase.WalkStart);
    }

    public void OnExit()
    {
        cooldownTime = 0;
    }

    public void OnUpdate()
    {
        cooldownTime += Time.deltaTime;
        if (!enemy.IsPlayerInDetectionRange())//如果玩家离开了检测范围
        {
            if (curPhase != CooldownPhase.Waiting)
            {
                ChangePhase(CooldownPhase.Waiting);//停止移动但继续计算冷却
            }
            if (cooldownTime >= enemy.EnemyModel.AtkCooldown)
            {
                enemy.ClearPlayerTarget();
                enemy.StateMachine.ChangeState(EnemyStateType.Idle);
            }
            return;
        }
        enemy.LookAtPlayer();
        if (cooldownTime >= enemy.EnemyModel.AtkCooldown)
        {
            CheckNextState();
            return;
        }
        switch (curPhase)
        {
            case CooldownPhase.WalkStart:
                UpdateWalkStart();
                break;

            case CooldownPhase.Walking:
                UpdateWalking();
                break;
        }
    }

    private void UpdateWalkStart()
    {
        EnemyAnimType startAnim;
        if (isWalkForward)
        {
            startAnim = EnemyAnimType.WalkForwardStart;
        }
        else
        {
            startAnim = EnemyAnimType.WalkBackwardStart;
        }
        if (enemy.IsAnimFinished(startAnim))
        {
            ChangePhase(CooldownPhase.Walking);
        }
    }

    private void UpdateWalking()
    {

    }

    private void CheckNextState()//判断冷却结束后的状态
    {
        if (enemy.IsPlayerInAttackRange())
        {
            enemy.StateMachine.ChangeState(EnemyStateType.Attack);//继续攻击
        }
        else
        {
            enemy.StateMachine.ChangeState(EnemyStateType.Chase);//追击玩家
        }
    }

    private void ChangePhase(CooldownPhase nextPhase)
    {
        curPhase = nextPhase;
        switch (curPhase)
        {
            case CooldownPhase.WalkStart:
                if (isWalkForward)
                {
                    enemy.PlayAnim(EnemyAnimType.WalkForwardStart);
                }
                else
                {
                    enemy.PlayAnim(EnemyAnimType.WalkBackwardStart);
                }
                break;

            case CooldownPhase.Walking:
                if (isWalkForward)
                {
                    enemy.PlayAnim(EnemyAnimType.WalkForward);
                }
                else
                {
                    enemy.PlayAnim(EnemyAnimType.WalkBackward);
                }
                break;

            case CooldownPhase.Waiting:
                enemy.PlayAnim(EnemyAnimType.Idle);
                break;
        }
    }
}
