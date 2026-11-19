using UnityEngine;

public class EnemyFactory
{
    private const string Path = "Prefabs/Enemy";//怪物预制体路径
    private GameObject enemyPrefab;//怪物预制体
    public EnemyFactory()
    {
        enemyPrefab = Resources.Load<GameObject>(Path);
        if (enemyPrefab == null)
        {
            Debug.LogError("没有找到预制体：" + Path);
        }
    }

    public EnemyManage CreateEnemy(Vector3 pos, Transform parent)//创建怪物
    {
        if (enemyPrefab == null)
        {
            return null;
        }
        GameObject enemyGO = Object.Instantiate(enemyPrefab, pos, Quaternion.identity, parent);//实例化
        enemyGO.name = enemyPrefab.name;//去掉(Clone)
        EnemyManage enemy = enemyGO.GetComponent<EnemyManage>();
        if (enemy == null)
        {
            Debug.LogError("Enemy预制体上没有EnemyManage");
            Object.Destroy(enemyGO);
            return null;
        }
        return enemy;
    }
}
