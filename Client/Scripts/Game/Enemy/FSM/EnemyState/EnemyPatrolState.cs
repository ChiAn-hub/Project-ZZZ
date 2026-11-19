using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolState : IEnemyState
{
    private enum PatrolPhase//巡逻阶段
    {
        WalkStart,//起步
        Walking,//持续行走
    }

    private EnemyManage enemy;//获取到对应的管理方法
    private PatrolPhase curPhase;//当前的阶段
    private List<Vector3> path = new List<Vector3>();//当前巡逻路径

    private int curPathIndex;//当前正在前往的路径点下标
    private const int MaxFindPathCount = 10;//最多尝试选择目标的次数
    private const float ReachDistance = 0.5f;//到达路径点的距离判断标准
    private const float RotationSpeed = 360f;//每秒旋转角度

    public EnemyPatrolState(EnemyManage enemy)
    {
        this.enemy = enemy;
    }
    public void OnEnter()//进入待机状态
    {
        path.Clear();//清空路径
        curPathIndex = 0;//重置当前路径索引

        enemy.ClearHitWall();

        if (!CreatePatorlPath())//如果没有创建巡逻路径
        {
            enemy.StateMachine.ChangeState(EnemyStateType.Idle);//则待机
            return;
        }

        RotateToCurPathPoint(true);//进入巡逻时立即面向第一个路径点

        ChangePhase(PatrolPhase.WalkStart);//播放起步动画
    }

    public void OnExit()
    {
        path.Clear();//清空路径
        curPathIndex = 0;//重置当前路径索引
    }

    public void OnUpdate()
    {
        if (enemy.FindPlayer())//发现玩家
        {
            enemy.StateMachine.ChangeState(EnemyStateType.Chase);//进入追击状态
            return;
        }
        if (enemy.IsHitWall)//如果碰到了墙
        {
            enemy.ClearHitWall();
            enemy.StateMachine.ChangeState(EnemyStateType.Idle);//进入待机状态
            return;
        }

        if (path == null || curPathIndex >= path.Count)//如果路径为空或者当前索引已经超过了路径长度
        {
            enemy.StateMachine.ChangeState(EnemyStateType.Idle);//进入待机状态
            return;
        }

        switch (curPhase)
        {
            case PatrolPhase.WalkStart:
                UpdateWalkStart();
                break;
            case PatrolPhase.Walking:
                UpdateWalking();
                break;
        }
    }

    private void UpdateWalkStart()//起步状态
    {
        RotateToCurPathPoint(false);
        if (enemy.IsAnimFinished(EnemyAnimType.WalkForwardStart))//如果起步动画播放完成
        {
            ChangePhase(PatrolPhase.Walking);//进入移动状态
        }
    }

    private void UpdateWalking()//移动状态
    {
        if (IsCurPathPointReached())//如果当前路径点已到达
        {
            curPathIndex++;

            if (curPathIndex >= path.Count)
            {
                enemy.StateMachine.ChangeState(EnemyStateType.Idle);
                return;
            }
        }

        RotateToCurPathPoint(false);
    }

    private void ChangePhase(PatrolPhase nextPhase)//切换巡逻状态
    {
        curPhase = nextPhase;
        switch (curPhase)
        {
            case PatrolPhase.WalkStart:
                enemy.PlayAnim(EnemyAnimType.WalkForwardStart);//播放起步动画
                break;
            case PatrolPhase.Walking:
                enemy.PlayAnim(EnemyAnimType.WalkForward);//播放移动动画
                break;
        }
    }

    private bool CreatePatorlPath()//创建巡逻路径
    {
        for (int i = 0; i < MaxFindPathCount; i++)
        {
            Vector2 randomDirection = Random.insideUnitCircle;//随机生成一个方向向量
            if (randomDirection.sqrMagnitude < 0.01f)//如果生成的方向过小，则重新生成
            {
                continue;
            }
            randomDirection.Normalize();//向量长度归一化
            float randomDistance = Random.Range(enemy.EnemyModel.MinPatrolDistance, enemy.EnemyModel.MaxPatrolDistance);//随机生成一个巡逻距离
            Vector3 targetPosition = enemy.transform.position + new Vector3(randomDirection.x, 0f, randomDirection.y) * randomDistance;//获取目标位置相对于怪物的位置
            AStarNode targetNode = AStarGrid.Instance.GetNodeFromWorldPosition(targetPosition);//获取目标位置对应的网格节点
            if (targetNode == null || !targetNode.Walkable)//如果目标位置对应的网格节点不存在或者不可行走
            {
                continue;
            }
            List<Vector3> newPath = AStarPathFinding.Instance.FindPath(enemy.transform.position, targetNode.WorldPos);//寻找从怪物位置到目标位置的路径
            if (newPath.Count == 0)//如果路径为空
            {
                continue;
            }

            path = newPath;//设置路径
            curPathIndex = 0;//设置当前路径索引为0

            return true;
        }
        return false;
    }

    private bool IsCurPathPointReached()//当前路径点是否已到达
    {
        Vector3 distance = path[curPathIndex] - enemy.transform.position;//怪物指向当前路径点的向量
        distance.y = 0;//忽略Y轴
        return distance.sqrMagnitude < ReachDistance * ReachDistance;//判断距离是否小于指定距离
    }

    private void RotateToCurPathPoint(bool immediately)//是否立刻转向当前路径点
    {
        if (path == null || curPathIndex >= path.Count)
        {
            return;
        }
        Vector3 direction = path[curPathIndex] - enemy.transform.position;//怪物指向当前路径点的向量
        direction.y = 0;//忽略Y轴
        if (direction.sqrMagnitude < 0.01f)
        {
            return;//距离过近时不进行旋转
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);//怪物指向当前路径点的旋转量
        if (immediately)//如果需要立即转向
        {
            enemy.transform.rotation = targetRotation;
            return;
        }

        enemy.transform.rotation = Quaternion.RotateTowards(enemy.transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);

    }

}
