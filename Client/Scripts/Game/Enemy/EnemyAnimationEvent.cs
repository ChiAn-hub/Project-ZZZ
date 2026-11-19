using UnityEngine;

public class EnemyAnimationEvent : MonoBehaviour
{
    private EnemyManage enemy;

    private void Awake()
    {
        enemy = GetComponent<EnemyManage>();
    }

    public void AttackLookAtPlayer()
    {
        if (enemy == null)
        {
            return;
        }

        enemy.LookAtPlayer();
    }
}
