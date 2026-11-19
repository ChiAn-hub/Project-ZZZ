using UnityEngine;

public class PlayerRootMotion : MonoBehaviour
{
    private Animator animator;
    private CharacterController characterController;

    private float unreportedDistance;//记录玩家移动距离
    private bool isSendingDistance;//是否正在发送移动距离,防止多次发送

    private const float ReportDistance = 1f;//每隔多远发送一次移动距离
    private const float MaxDistancePerReport = 5f;//最大发送距离

    private void Awake()
    {
        animator = transform.GetComponent<Animator>();
        characterController = GetComponentInParent<CharacterController>();
    }

    private void OnAnimatorMove()//当角色动画播放位移时,控制根节点移动
    {
        if (!PlayerInput.CanControl)//玩家不可控制时,不进行移动
        {
            return;
        }
        if (characterController == null)//未找到组件时不进行位移
        {
            return;
        }

        Vector3 deltaPosition = animator.deltaPosition;//获取动画每帧产生的位移
        bool useVerticalRootMotion = false;//是否使用动画的Y轴位移

        if (PlayerController.Instance != null)
        {
            PlayerAnimationType curAnim = PlayerController.Instance.CurAnim;
            useVerticalRootMotion = curAnim == PlayerAnimationType.EXSkill || curAnim == PlayerAnimationType.EXSkillEnd;
        }

        if (!useVerticalRootMotion)
        {
            deltaPosition.y = 0f;//普通动画锁定高度
        }

        if (PlayerController.Instance != null && PlayerController.Instance.CurAnim == PlayerAnimationType.TurnBack)//如果玩家当前动画是转身时
        {
            float rotationScale = PlayerController.Instance.TurnBackRotationScale;//获取转身时旋转的倍率

            Quaternion scaledDeltaRotation = Quaternion.SlerpUnclamped(Quaternion.identity, animator.deltaRotation, rotationScale);//扩大动画的旋转大小
            characterController.transform.rotation *= scaledDeltaRotation;//更新根节点的旋转大小
        }

        Vector3 oldPosition = characterController.transform.position;//获取当前位置

        characterController.Move(deltaPosition);//移动到新的位置
        Vector3 newPosition = characterController.transform.position;//获取新的位置
        Vector3 distance = newPosition - oldPosition;//获取移动的向量

        distance.y = 0f;//升空距离不计算为移动任务距离

        float distanceMoved = distance.magnitude;//获取移动的向量的模
        if (distanceMoved > 0.001f)
        {
            bool hasMoveQuest = QuestModel.Instance.HasActiveQuestTarget(QuestTarget.MoveDistance);//判断是否有移动任务
            if (!hasMoveQuest)//没有移动任务
            {
                unreportedDistance = 0f;//重置未汇报的距离
                return;
            }
            unreportedDistance += distanceMoved;//移动距离累加
            TryReportMoveDistance();//尝试汇报移动距离
        }
    }
    private void TryReportMoveDistance()//尝试汇报移动距离
    {
        if (isSendingDistance)//如果正在发送中，则不发送
        {
            return;
        }
        if (unreportedDistance < ReportDistance)//如果玩家移动距离小于最低发送距离，则不发送
        {
            return;
        }
        float sendDistance = Mathf.Min(unreportedDistance, MaxDistancePerReport);//计算发送距离
        isSendingDistance = true;
        ProtoManager.Instance.CSMoveDistance(sendDistance, (msg) =>
        {
            if (msg != null && msg.Result == QuestOperationResult.Success)
            {
                unreportedDistance -= sendDistance;
                unreportedDistance = Mathf.Max(0, unreportedDistance);//减去发送的距离
                QuestModel.Instance.UpdateProgress(QuestTarget.MoveDistance, sendDistance);
            }
            else
            {
                Debug.LogWarning("移动任务进度上报失败");
            }
            isSendingDistance = false;//发送完成
        });
    }
}