using System;
using System.Collections.Generic;
using UnityEngine;



public class EnemyManage : MonoBehaviour
{

    [SerializeField]
    private Transform attackPoint;//怪物攻击检测位置

    private readonly Collider[] playerHitResults = new Collider[5];//检测结果缓存

    private Animator animator;//获取怪物身上的动画组件
    private CharacterController ccController;//获取怪物身上的控制器组件
    private EnemyStateMachine stateMachine;//创建状态机
    private EnemyModel enemyModel;
    private bool isHitWall;//是否碰到墙壁
    private Transform playerTarget;//检测到玩家
    private int playerLayer;
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
    }

    void OnEnable()//启用时
    {
        isHitWall = false;
        playerTarget = null;
        enemyModel.ResetData();//重置数据
        stateMachine.ChangeState(EnemyStateType.Idle);//切换到Idle状态
    }

    // Update is called once per frame
    void Update()
    {
        if (stateMachine == null)
        {
            return;
        }

        stateMachine.Update();//更新当前状态
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
            stateMachine.ChangeState(EnemyStateType.Death);
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
            stateMachine.ChangeState(EnemyStateType.Hit);
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
