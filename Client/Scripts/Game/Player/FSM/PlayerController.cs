using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Animator animator;//获取玩家动画
    private Transform cameraTransform;//获取摄像机位置
    private PlayerInput playerInput;//获取玩家输入
    private AttackState attackState;//普通攻击状态
    private SkillState skillState;//技能状态
    private NormalHitState normalHitState;//普通受击状态
    private PlayerStateMachine stateMachine;//获取状态机

    private PlayerAnimationType curAnim;
    private bool isMoveAttack;//本次是否使用跑动攻击

    public static PlayerController Instance { get; private set; }
    public float TurnBackRotationScale { get; set; } = 1f;//转身动画旋转倍率
    private float nearEnemyRange = 3.0f;//判断敌人是否处于近距离
    private int attackStartIndex = 1;//普通攻击从第几段开始

    [SerializeField]
    private Transform attackPoint;//攻击检测位置
    private float attackRange = 2.5f;//攻击检测范围
    [SerializeField]
    private LayerMask enemyLayer;//怪物层级
    private readonly Collider[] attackHitResults = new Collider[20];
    private readonly HashSet<EnemyManage> hitEnemies = new HashSet<EnemyManage>();


    private readonly Dictionary<PlayerAnimationType, int> animDic = new Dictionary<PlayerAnimationType, int>();//只读字典存储动画

    public int AttackStartIndex
    {
        get
        {
            return attackStartIndex;
        }
    }

    public PlayerAnimationType CurAnim
    {
        get
        {
            return curAnim;
        }
    }

    public PlayerInput Input
    {
        get
        {
            return playerInput;
        }
    }

    public PlayerStateMachine StateMachine
    {
        get
        {
            return stateMachine;
        }
    }

    public bool IsMoveAttack
    {
        get
        {
            return isMoveAttack;
        }
    }

    private void Awake()
    {
        Instance = this;

        cameraTransform = Camera.main.transform;//获取主摄像机位置
        playerInput = GetComponent<PlayerInput>();//获取玩家输入
        stateMachine = new PlayerStateMachine();//创建状态机
        attackState = new AttackState(this);
        skillState = new SkillState(this);
        normalHitState = new NormalHitState(this);

        ReadAnimInfo();//读取动画配置

        stateMachine.AddState(StateEnum.Idle, new IdleState(this));//添加待机状态,和对应的状态脚本
        stateMachine.AddState(StateEnum.Walk, new WalkState(this));//添加行走状态,和对应的状态脚本
        stateMachine.AddState(StateEnum.Evade, new EvadeState(this));//添加闪避状态,和对应状态脚本
        stateMachine.AddState(StateEnum.Run, new RunState(this));//添加跑步状态,和对应的状态脚本
        stateMachine.AddState(StateEnum.Attack, attackState);//添加攻击状态,和对应状态脚本
        stateMachine.AddState(StateEnum.Skill, skillState);//添加技能状态,和对应状态脚本
        stateMachine.AddState(StateEnum.EXSkill, new EXSkillState(this));//添加大招技能状态,和对应状态脚本
        stateMachine.AddState(StateEnum.Dash, new DashState(this));//添加闪击状态,和对应状态脚本
        stateMachine.AddState(StateEnum.Death, new DeathState(this));//添加死亡状态,和对应状态脚本
        stateMachine.AddState(StateEnum.HitFly, new HitFlyState(this));//添加被击飞状态,和对应状态脚本
        stateMachine.AddState(StateEnum.NormalHit, normalHitState);//添加被攻击状态,和对应状态脚本
        stateMachine.AddState(StateEnum.EnterStart, new EnterStartState(this));//添加进入战斗状态,和对应状态脚本
    }

    private void Start()
    {

        PlayerPanelController.Instance.RequestEllenInfo((success) =>
        {
            if (!success)
            {
                Debug.LogError("进入战斗场景时获取角色信息失败");
                stateMachine.ChangeState(StateEnum.Idle);
                return;
            }
            if (EllenDataModel.Instance.IsDead())
            {
                stateMachine.ChangeState(StateEnum.Death);
            }
            else
            {
                stateMachine.ChangeState(StateEnum.Idle);
            }
        });
    }

    private void Update()
    {
        stateMachine.Update();//持续更新状态
    }

    private void ReadAnimInfo()//读取动画配置文件
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Config/Anim");
        AnimList animList = JsonUtility.FromJson<AnimList>(textAsset.text);
        foreach (AnimInfo animInfo in animList.animations)
        {
            string animName = "Base Layer." + animInfo.stateName;//拼接出动画名称
            int animHash = Animator.StringToHash(animName);//将字符串转换为哈希值(整数),减少开销
            if (animDic.ContainsKey(animInfo.animationType))
            {
                continue;
            }
            animDic.Add(animInfo.animationType, animHash);//添加动画
        }
    }
    public Vector3 GetMoveDirection()//获取玩家相对于摄像机的移动方向
    {
        Vector2 input = playerInput.MoveInput;//获取输入的移动方向

        Vector3 forward = cameraTransform.forward;//获取摄像机前方
        Vector3 right = cameraTransform.right;//获取摄像机的右方

        forward.y = 0f;//锁定高度
        right.y = 0f;//锁定高度

        forward.Normalize();
        right.Normalize();

        Vector3 moveDir = forward * input.y + right * input.x;

        if (moveDir.sqrMagnitude < 0.01f)
        {
            return Vector3.zero;
        }

        return moveDir.normalized;
    }
    public void RotateToMoveDirection()//根据输入的移动方向更新朝向
    {
        Vector3 moveDir = GetMoveDirection();

        if (moveDir == Vector3.zero)
        {
            return;
        }

        transform.forward = moveDir;//更新角色朝向
    }

    public void PlayAnim(PlayerAnimationType animType)//播放动画
    {
        int animHash;
        if (!animDic.TryGetValue(animType, out animHash))//查找对应的动画值
        {
            Debug.LogError("没有找到对应的动画");
            return;
        }
        curAnim = animType;//记录当前播放的动画
        animator.CrossFadeInFixedTime(animHash, 0.15f, 0);//播放对应动画
    }

    public void ChangeEvadeState(PlayerAnimationType animType)//进入闪避状态
    {
        if (animType != PlayerAnimationType.EvadeFront && animType != PlayerAnimationType.EvadeBack)
        {
            Debug.LogError("不是闪避动画：" + animType);
            return;
        }
        curAnim = animType;
        stateMachine.ChangeState(StateEnum.Evade);//进入闪避状态
    }

    public void ChangeAttackState(bool moveAttack, int startIndex = 1)
    {
        isMoveAttack = moveAttack;
        attackStartIndex = Mathf.Clamp(startIndex, 1, 3);
        stateMachine.ChangeState(StateEnum.Attack);
    }

    public void ChangeSkillState()
    {
        stateMachine.ChangeState(StateEnum.Skill);
    }

    public void ChangeExSkillState()
    {
        stateMachine.ChangeState(StateEnum.EXSkill);
    }

    public bool HasNearEnemy()//检查附近是否有存活的敌人
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, nearEnemyRange);
        foreach (Collider targetCollider in colliders)
        {
            if (!targetCollider.CompareTag("Enemy"))
            {
                continue;
            }
            EnemyManage enemy = targetCollider.GetComponentInParent<EnemyManage>();
            if (enemy == null)
            {
                continue;
            }
            if (!enemy.EnemyModel.IsDead())
            {
                return true;
            }
        }
        return false;
    }

    public bool IsAnimFinished(PlayerAnimationType animType)//判断动画是否播放完毕
    {
        int animHash;
        if (!animDic.TryGetValue(animType, out animHash))
        {
            Debug.LogError("没有找到动画配置：" + animType);
            return false;
        }
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        bool isCurAnim = stateInfo.fullPathHash == animHash;//判断是不是对应动画
        bool isFinished = stateInfo.normalizedTime >= 1f;//判断动画是否播放完成
        bool isTransitioning = animator.IsInTransition(0);//判断是否处于过渡中
        return isCurAnim && isFinished && !isTransitioning;
    }

    public void OpenAttackComboWindow()//打开普通攻击连段窗口
    {
        if (attackState == null)
        {
            return;
        }

        attackState.OpenAttackComboWindow();
    }

    public void OpenSkillComboWindow()
    {
        if (skillState == null) return;

        skillState.OpenSkillComboWindow();
    }

    public void OpenHitInterruptWindow()
    {
        if (normalHitState == null)
        {
            return;
        }

        normalHitState.OpenHitInterruptWindow();
    }

    public void AttackHit(float damageRate)//攻击命中
    {
        if (damageRate <= 0f || attackPoint == null)
        {
            return;
        }

        int playerAttack = EllenDataModel.Instance.Attack;//获取玩家攻击力

        int attack = Mathf.RoundToInt(playerAttack * damageRate);//根据伤害倍率,计算伤害值

        int hitCount = Physics.OverlapSphereNonAlloc(attackPoint.position, attackRange, attackHitResults, enemyLayer);//检测命中的敌人

        hitEnemies.Clear();

        for (int i = 0; i < hitCount; i++)
        {
            Collider hitCollider = attackHitResults[i];

            if (hitCollider == null)
            {
                continue;
            }

            EnemyManage enemy = hitCollider.GetComponentInParent<EnemyManage>();

            if (enemy == null || hitEnemies.Contains(enemy))
            {
                continue;
            }

            hitEnemies.Add(enemy);
            enemy.TakeDamage(attack);
        }
    }

    public int TakeDamage(int enemyAtk, bool isHitFly = false)//角色受到伤害
    {
        int damage = EllenDataModel.Instance.TakeDamage(enemyAtk);
        if (damage <= 0)
        {
            return 0;
        }

        PlayerPanelController.Instance.UpdateEllenHp((success) =>
        {
            if (!success)
            {
                PlayerPanelController.Instance.RequestEllenInfo();
            }
        });

        if (EllenDataModel.Instance.IsDead())
        {
            stateMachine.ChangeState(StateEnum.Death);
        }
        else if (isHitFly)
        {
            stateMachine.ChangeState(StateEnum.HitFly);
        }
        else
        {
            stateMachine.ChangeState(StateEnum.NormalHit);
        }

        return damage;
    }
}