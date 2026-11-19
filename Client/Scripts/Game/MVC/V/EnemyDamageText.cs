using UnityEngine;

public class EnemyDamageText : MonoBehaviour
{

    private Vector3 damageNumberOffset = new Vector3(0f, 2.5f, 0f);

    private EnemyManage enemy;
    private EnemyModel enemyModel;

    private void Start()
    {
        enemy = GetComponent<EnemyManage>();

        if (enemy == null)
        {
            return;
        }

        enemyModel = enemy.EnemyModel;
        enemyModel.Damaged += ShowDamageNumber;
    }

    private void OnDestroy()
    {
        if (enemyModel != null)
        {
            enemyModel.Damaged -= ShowDamageNumber;
        }
    }

    private void ShowDamageNumber(int damage)//显示伤害数字
    {

        Vector3 randomOffset = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.1f, 0.3f), 0f);

        Vector3 showPosition = transform.position + damageNumberOffset + randomOffset;//随机位置生成伤害数字

        DamageNumberItem damageNumber = DamageNumberPool.Instance.GetDamageNumber(showPosition);

        damageNumber.Init(damage);
    }
}