using UnityEngine;

public class PlayerAnimationEvent : MonoBehaviour
{
    private PlayerEffectManager effectManager;
    private Transform effectOrigin;//特效原点

    private void Awake()
    {
        effectManager = GetComponent<PlayerEffectManager>();

        PlayerController player = GetComponentInParent<PlayerController>();
        effectOrigin = player.transform;
    }

    public void OpenAttackComboWindow()//打开普通攻击连段窗口
    {
        if (PlayerController.Instance == null)
        {
            return;
        }

        PlayerController.Instance.OpenAttackComboWindow();
    }

    public void AttackHit(float damageRate)//攻击命中动画事件
    {
        if (PlayerController.Instance == null)
        {
            return;
        }

        PlayerController.Instance.AttackHit(damageRate);
    }

    public void OpenSkillComboWindow()
    {
        if (PlayerController.Instance == null) return;

        PlayerController.Instance.OpenSkillComboWindow();
    }

    public void OpenHitInterruptWindow()
    {
        if (PlayerController.Instance == null)
        {
            return;
        }

        PlayerController.Instance.OpenHitInterruptWindow();
    }

    public void PlayAttackEffect(int effectId)
    {
        if (effectManager == null)
        {
            return;
        }

        effectManager.PlayEffect((PlayerEffectType)effectId, effectOrigin.position, effectOrigin.rotation);
    }

}