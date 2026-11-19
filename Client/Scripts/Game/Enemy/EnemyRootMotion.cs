using UnityEngine;

public class EnemyRootMotion : MonoBehaviour
{
    private const float Gravity = -20f;//怪物重力
    private const float GroundedVelocity = -2f;//贴住地面的向下速度
    private float verticalVelocity;//怪物当前垂直速度

    private Animator animator;//获取动画组件
    private CharacterController ccController;//获取角色控制器
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        animator = GetComponent<Animator>();
        ccController = GetComponent<CharacterController>();
    }

    void OnEnable()
    {
        verticalVelocity = 0f;//对象池重新启用时重置下落速度
    }

    void Update()
    {
        UpdateGravity();//持续处理重力
    }

    private void OnAnimatorMove()//通过动画产生的位移移动怪物
    {
        if (animator == null || ccController == null)
        {
            return;
        }
        if (!ccController.enabled)//角色控制器已关闭
        {
            return;
        }
        Vector3 deltaPosition = animator.deltaPosition;//获取动画产生的位移
        deltaPosition.y = 0f;//锁定Y轴

        ccController.Move(deltaPosition);//移动
    }
    private void UpdateGravity()//处理怪物重力
    {
        if (ccController == null || !ccController.enabled)
        {
            return;
        }

        if (ccController.isGrounded && verticalVelocity < 0f)//怪物贴地且速度小于0
        {
            verticalVelocity = GroundedVelocity;//让怪物保持贴地
        }
        else
        {
            verticalVelocity += Gravity * Time.deltaTime;//增加下落速度
        }

        Vector3 gravityMove = Vector3.up * verticalVelocity * Time.deltaTime;
        ccController.Move(gravityMove);
    }
}
