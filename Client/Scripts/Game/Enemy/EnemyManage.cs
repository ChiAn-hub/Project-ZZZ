using System;
using System.Collections.Generic;
using UnityEngine;



public class EnemyManage : MonoBehaviour
{
    private int playerLayer;
    private bool isHitWall;//是否碰到墙壁
    private bool isOnHit;//是否进入受击状态

    [SerializeField]
    private Transform attackPoint;//怪物攻击检测位置
    private Transform playerTarget;//检测到玩家
    private Animator animator;//获取怪物身上的动画组件
    private EnemyModel enemyModel;
    private CharacterController ccController;//获取怪物身上的控制器组件

    private EnemyStateMachine stateMachine;//创建状态机
    private BehaviorTree behaviorTree;//创建行为树

    private readonly Collider[] playerHitResults = new Collider[5];//检测结果缓存
    private Dictionary<EnemyAnimType, int> animDict = new Dictionary<EnemyAnimType, int>();//用字典存储动画类型和对应的哈希值
    public bool IsHitWall
    {
        get
        {
            return isHitWall;
        }
    }
    public Transform PlayerTarget
    {
        get
        {
            return playerTarget;
        }
    }
    public EnemyStateMachine StateMachine
    {
        get
        {
            return stateMachine;
        }
    }

    public CharacterController CharacterController
    {
        get
        {
            return ccController;
        }
    }
    public EnemyModel EnemyModel
    {
        get
        {
            return enemyModel;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        animator = GetComponent<Animator>();
        ccController = GetComponent<CharacterController>();
        enemyModel = new EnemyModel();
        playerLayer = LayerMask.GetMask("Player");

        ReadEnemyInfo();//读取怪物动画信息

        stateMachine = new EnemyStateMachine();
        stateMachine.AddState(EnemyStateType.Idle, new EnemyIdleState(this));
        stateMachine.AddState(EnemyStateType.Patrol, new EnemyPatrolState(this));
        stateMachine.AddState(EnemyStateType.Chase, new EnemyChaseState(this));
        stateMachine.AddState(EnemyStateType.Attack, new EnemyAttackState(this));
        stateMachine.AddState(EnemyStateType.Cooldown, new EnemyCooldownState(this));
        stateMachine.AddState(EnemyStateType.Hit, new EnemyHitState(this));
        stateMachine.AddState(EnemyStateType.Death, new EnemyDeathState(this));

        CreateBehaviorTree();//创建怪物行为树
    }

    void OnEnable()//启用时
    {
        isHitWall = false;
        isOnHit = false;
        playerTarget = null;
        enemyModel.ResetData();//重置数据
        stateMachine.ChangeState(EnemyStateType.Idle, true);//切换到Idle状态
    }

    // Update is called once per frame
    void Update()
    {
        if (stateMachine == null || behaviorTree == null)
        {
            return;
        }

        behaviorTree.Update();//行为树决定怪物处于哪个状态
        stateMachine.Update();//FSM执行当前状态的具体逻辑
    }
    private void ReadEnemyInfo()
    {
        TextAsset textAsset = Resources.Load("Config/EnemyAnim") as TextAsset;
        EnemyAnimList animList = JsonUtility.FromJson<EnemyAnimList>(textAsset.text);
        foreach (EnemyAnimInfo animInfo in animList.animations)
        {
            string animName = "Base Layer." + animInfo.animName;//拼接完整动画状态名
            int animHash = Animator.StringToHash(animName);//获取动画状态的哈希值
            if (animDict.ContainsKey(animInfo.animType))
            {
                continue;
            }
            animDict.Add(animInfo.animType, animHash);//保存动画类型和哈希值
        }
    }

    private bool IsStateRunning(EnemyStateType stateType)//判断当前状态是否正在运行
    {
        return stateMachine.CurStateType == stateType && stateMachine.CurStateResult == EnemyStateResult.Running;
    }

    private bool IsStateSuccess(EnemyStateType stateType)//判断当前状态是否成功
    {
        return stateMachine.CurStateType == stateType && stateMachine.CurStateResult == EnemyStateResult.Success;
    }

    private BTNodeState KeepCurrentState()//用于告诉状态树保持当前状态
    {
        return BTNodeState.Running;
    }

    private bool HasValidPlayerTarget()//判断玩家目标是否有效
    {
        if (!FindPlayer())//找不到玩家
        {
            return false;
        }

        if (IsPlayerInDetectionRange())//玩家在检测范围内
        {
            return true;
        }

        ClearPlayerTarget();//玩家离开检测范围,清空旧目标
        return false;
    }

    private BTNodeState ChangeEnemyState(EnemyStateType stateType)//更改状态
    {
        stateMachine.ChangeState(stateType);
        return BTNodeState.Running;
    }

    private BTNodeState ChangeToHitState()//进入受击状态
    {
        isOnHit = false;
        stateMachine.ChangeState(EnemyStateType.Hit, true);////连续受击时重新播放受击动画
        return BTNodeState.Running;
    }

    private BTNode CreateChangeStateBranch(Func<bool> condition, EnemyStateType stateType)//根据条件和状态类型创建和更改状态分支
    {
        return new SequenceBTNode(new List<BTNode>()
        {
            new ConditionNode(condition),//条件节点
            new ActionNode(() => ChangeEnemyState(stateType))//动作节点
        });
    }

    private BTNode CreateRunningStateBranch(EnemyStateType stateType)
    {
        return new SequenceBTNode(new List<BTNode>()
        {
            new ConditionNode(() => IsStateRunning(stateType)),//条件节点
            new ActionNode(KeepCurrentState)//动作节点
        });
    }

    private void CreateBehaviorTree()//创建怪物行为树
    {
        BTNode hitBranch = new SequenceBTNode(new List<BTNode>()
        {
            new ConditionNode(() => isOnHit),//是否收到受击请求
            new ActionNode(ChangeToHitState)//进入受击状态
        });

        BTNode combatBranch = new SequenceBTNode(new List<BTNode>()//创建一个战斗分支的顺序节点
        {

            new ConditionNode(HasValidPlayerTarget),//判断是否存在有效玩家目标
            new SelectBTNode(new List<BTNode>()//创建一个新的分支
            {
                new SequenceBTNode(new List<BTNode>()//新分支下的顺序节点
                {
                    new ConditionNode(IsPlayerInAttackRange),//玩家进入攻击范围
                    new ActionNode(() => ChangeEnemyState(EnemyStateType.Attack))//切换状态为攻击
                }),
                new ActionNode(() => ChangeEnemyState(EnemyStateType.Chase))//不在攻击范围则追击
            })
        });

        BTNode rootNode = new SelectBTNode(new List<BTNode>()//创建行为树根节点
        {
            CreateChangeStateBranch(()=> enemyModel.IsDead(), EnemyStateType.Death),//死亡分支优先级最高
            hitBranch,//收到伤害时进入受击状态
            CreateRunningStateBranch(EnemyStateType.Hit),//受击动画没有结束时,保持受击状态
            CreateRunningStateBranch(EnemyStateType.Attack),//攻击动画没有结束时,保持攻击状态
            CreateChangeStateBranch(()=>IsStateSuccess(EnemyStateType.Attack),EnemyStateType.Cooldown),//攻击完成后进入攻击冷却
            CreateRunningStateBranch(EnemyStateType.Cooldown),//冷却没有完成时,保持冷却状态
            combatBranch,//发现玩家后,在攻击和追击之间选择
            CreateRunningStateBranch(EnemyStateType.Patrol),//巡逻没有结束时,保持巡逻状态
            CreateChangeStateBranch(()=>IsStateSuccess(EnemyStateType.Idle),EnemyStateType.Patrol),//待机完成后进入巡逻
            new ActionNode(() => ChangeEnemyState(EnemyStateType.Idle))//没有其他行为可以执行时进入待机
        });

        behaviorTree = new BehaviorTree(rootNode);
    }

    public void PlayAnim(EnemyAnimType animType)//播放动画
    {
        int animHash;
        if (!animDict.TryGetValue(animType, out animHash))//查找对应的动画值
        {
            Debug.LogError("没有找到对应的动画");
            return;
        }
        animator.CrossFadeInFixedTime(animHash, 0.15f, 0);//播放对应动画
    }
    public bool IsAnimFinished(EnemyAnimType animType)//判断动画是否播放完毕
    {
        int animHash;
        if (!animDict.TryGetValue(animType, out animHash))//查找对应的动画值
        {
            Debug.LogError("没有找到对应的动画");
            return false;
        }
        AnimatorStateInfo clipInfo = animator.GetCurrentAnimatorStateInfo(0);
        bool isCurAnim = clipInfo.fullPathHash == animHash;//判断是否为同一个动画
        bool isFinished = clipInfo.normalizedTime >= 1f;//判断动画是否播放完成
        bool isTransitioning = animator.IsInTransition(0);//判断是否处于过渡中
        return isCurAnim && isFinished && !isTransitioning;
    }
    public void ClearHitWall()//清除是否碰到墙壁的标志
    {
        isHitWall = false;
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!hit.collider.CompareTag("Wall"))
        {
            return;
        }
        Vector3 horizontalNormal = hit.normal;//获取碰撞面的法线
        horizontalNormal.y = 0;//清空Y轴,如果是地面的话值为(0,0,0)
        if (horizontalNormal.sqrMagnitude < 0.01f)//计算出法线长度,排除站在地面上
        {
            return;
        }
        isHitWall = true;//碰到了墙
    }
    public bool FindPlayer()//发现玩家
    {
        if (playerTarget != null)
        {
            return true;
        }
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, enemyModel.DetectionRange, playerLayer);
        foreach (Collider targetCollider in hitColliders)
        {
            if (!targetCollider.CompareTag("Player"))
            {
                continue;
            }

            playerTarget = targetCollider.transform;
            return true;
        }
        return false;
    }
    public float GetPlayerDistance()//获取怪物与玩家的水平距离
    {
        if (playerTarget == null)
        {
            return float.MaxValue;
        }

        Vector3 distance = playerTarget.position - transform.position;
        distance.y = 0f;//只计算水平距离

        return distance.magnitude;
    }
    public bool IsPlayerInDetectionRange()//玩家是否在怪物的检测范围内
    {
        if (playerTarget == null)
        {
            return false;
        }

        return GetPlayerDistance() <= enemyModel.DetectionRange;
    }
    public bool IsPlayerInAttackRange()//玩家是否在怪物攻击范围内
    {
        if (playerTarget == null)
        {
            return false;
        }

        return GetPlayerDistance() <= enemyModel.AtkRange;
    }
    public void LookAtPlayer()//怪物朝向玩家
    {
        if (playerTarget == null)
        {
            return;
        }
        Vector3 direction = playerTarget.position - transform.position;
        direction.y = 0f;
        if (direction.sqrMagnitude < 0.01f)//距离平方值 
        {
            return;
        }
        transform.rotation = Quaternion.LookRotation(direction);
    }
    public void ClearPlayerTarget()//清空玩家对象
    {
        playerTarget = null;
    }
    public int TakeDamage(int atk)//怪物收到攻击
    {
        if (enemyModel.IsDead())
        {
            return 0;
        }
        int damage = enemyModel.TakeDamage(atk);
        if (damage <= 0)
        {
            return 0;
        }
        if (enemyModel.IsDead())
        {
            isOnHit = false;

            bool hasKillEnemyQuest = QuestModel.Instance.HasActiveQuestTarget(QuestTarget.KillEnemy);
            if (hasKillEnemyQuest)
            {
                ProtoManager.Instance.CSKillEnemy((msg) =>
                {
                    if (msg == null || msg.Result != QuestOperationResult.Success)
                    {
                        Debug.LogError("击杀敌人进度上报失败");
                        return;
                    }

                    QuestModel.Instance.UpdateProgress(QuestTarget.KillEnemy, 1);
                });
            }
        }
        else
        {
            isOnHit = true;
        }

        return damage;
    }

    public void NormalAttackHit()//普通攻击命中帧
    {
        AttackPlayer(false);
    }

    public void HitFlyAttackHit()//击飞攻击命中帧
    {
        AttackPlayer(true);
    }

    private void AttackPlayer(bool isHitFly)//检测并攻击角色
    {
        if (attackPoint == null || enemyModel == null || enemyModel.IsDead())
        {
            return;
        }

        int hitCount = Physics.OverlapSphereNonAlloc(attackPoint.position, enemyModel.AtkRange, playerHitResults, playerLayer);//用相交球检测角色并存储在数组里

        for (int i = 0; i < hitCount; i++)
        {
            Collider hitCollider = playerHitResults[i];

            if (hitCollider == null)
            {
                continue;
            }

            PlayerController player = hitCollider.GetComponentInParent<PlayerController>();

            if (player == null)
            {
                continue;
            }

            player.TakeDamage(enemyModel.Atk, isHitFly);

            return;//防止玩家身上多个碰撞体导致重复扣血
        }
    }
}
