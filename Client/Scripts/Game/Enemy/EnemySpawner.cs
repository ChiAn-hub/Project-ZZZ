using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private const int MinActiveCount = 3;//最少同时存在数量
    private const int MaxActiveCount = 4;//最多同时存在数量
    private const int MaxSpawnCount = 10;//累计最多生成数量
    private const int MaxPositionTryCount = 10;//寻找出生位置的最大尝试次数

    private float spawnRadius = 5f;//生成半径
    private float checkRadius = 0.8f;//检测生成点位置是否被占用
    private float checkInterval = 0.5f;//检查是否生成怪物的间隔时间

    private EnemyPool enemyPool;//怪物对象池
    private int targetActiveCount;//目标数量
    private int totalSpawnCount;//已经生成数量
    private float checkTime;//检查计时器
    private void Awake()
    {
        enemyPool = new EnemyPool(transform);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetActiveCount = Random.Range(MinActiveCount, MaxActiveCount + 1);
        TryCreatEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        if (totalSpawnCount >= MaxSpawnCount)
        {
            return;//累计达到10只后不再检查生成
        }
        checkTime += Time.deltaTime;
        if (checkTime <= checkInterval)
        {
            return;
        }
        checkTime = 0;
        TryCreatEnemy();
    }
    private bool IsPositionAvailable(Vector3 position)//判断位置是否可用
    {
        Collider[] colliders = Physics.OverlapSphere(position, checkRadius);
        foreach (Collider target in colliders)
        {
            if (target.CompareTag("Enemy"))
            {
                return false;//不能与其他怪物重叠
            }

            if (target.CompareTag("Player"))
            {
                return false;//不能生成在玩家身上
            }

            if (target.CompareTag("Wall"))
            {
                return false;//不能生成在墙体中
            }
        }
        return true;
    }
    private bool TryGetSpawnPosition(out Vector3 spawnPosition)//尝试获取生成位置
    {
        for (int i = 0; i < MaxPositionTryCount; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;//随机生成一个圆内点
            Vector3 position = transform.position;
            position.x += randomCircle.x;
            position.z += randomCircle.y;
            if (IsPositionAvailable(position))
            {
                spawnPosition = position;
                return true;
            }
        }
        spawnPosition = Vector3.zero;
        return false;
    }
    private void TryCreatEnemy()//尝试生成敌人
    {
        while (enemyPool.GetActiveCount() < targetActiveCount && totalSpawnCount < MaxSpawnCount)
        {
            Vector3 spawnPosition;

            if (!TryGetSpawnPosition(out spawnPosition))
            {
                return;//没有找到合适的位置，本次停止生成
            }

            EnemyManage enemy = enemyPool.GetEnemy(spawnPosition);

            if (enemy == null)
            {
                return;//工厂创建失败，防止while死循环
            }

            totalSpawnCount++;//每次出场都增加累计数量  
        }
    }
}
