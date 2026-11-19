using System.Collections.Generic;
using UnityEngine;

public class EnemyPool
{
    private List<EnemyManage> enemyList;//怪物列表
    private EnemyFactory enemyFactory;//怪物工厂
    private Transform enemyParent;
    public EnemyPool(Transform Parent)
    {
        enemyParent = Parent;
        enemyFactory = new EnemyFactory();
        enemyList = new List<EnemyManage>();
    }

    public EnemyManage GetEnemy(Vector3 position)//获取怪物
    {
        foreach (EnemyManage enemy in enemyList)
        {
            if (enemy == null)
            {
                continue;
            }
            if (!enemy.gameObject.activeSelf)//如果找到已经死亡且被禁用的怪物
            {
                enemy.transform.position = position;
                enemy.transform.rotation = Quaternion.identity;

                enemy.gameObject.SetActive(true);
                return enemy;
            }
        }
        EnemyManage newEnemy = enemyFactory.CreateEnemy(position, enemyParent);
        if (newEnemy == null)
        {
            return null;
        }
        enemyList.Add(newEnemy);
        return newEnemy;
    }

    public void ReleaseEnemy(EnemyManage enemy)//主动回收怪物
    {
        if (enemy == null)
        {
            return;
        }
        enemy.gameObject.SetActive(false);
    }

    public int GetActiveCount()//获取当前存活的怪物数量
    {
        int count = 0;
        foreach (EnemyManage enemy in enemyList)
        {
            if (enemy == null)
            {
                continue;
            }
            if (enemy.gameObject.activeSelf)
            {
                count++;
            }
        }
        return count;
    }

    public int GetPoolCount()//获取对象池中创建了怪物的数量
    {
        return enemyList.Count;
    }
}
