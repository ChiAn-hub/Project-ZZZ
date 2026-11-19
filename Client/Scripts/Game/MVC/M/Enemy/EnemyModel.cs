using System;
using UnityEngine;

public class EnemyModel
{
    private int curHp = 100;//当前生命值
    private int maxHp = 100;//最大生命值
    private int atk = 50;//攻击力
    private int def = 20;//防御力

    private int rewardExp = 100;//经验奖励
    private int rewardGold = 5000;//金币奖励
    private int rewardMoney = 1000;//钻石奖励

    private float detectionRange = 15f;//检测范围
    private float atkRange = 2.5f;//攻击范围
    private float atkCooldown = 5f;//攻击冷却时间
    private float minIdleTime = 2f;//最短待机时间
    private float maxIdleTime = 4f;//最长待机时间
    private float minPatrolDistance = 15f;//最短巡逻距离
    private float maxPatrolDistance = 50f;//最远巡逻距离

    public int CurHp => curHp;
    public int MaxHp => maxHp;
    public int Atk => atk;
    public int Def => def;

    public int RewardExp => rewardExp;
    public int RewardGold => rewardGold;
    public int RewardMoney => rewardMoney;

    public float DetectionRange => detectionRange;
    public float AtkRange => atkRange;
    public float AtkCooldown => atkCooldown;
    public float MinIdleTime => minIdleTime;
    public float MaxIdleTime => maxIdleTime;
    public float MinPatrolDistance
    {
        get
        {
            return minPatrolDistance;
        }
    }

    public float MaxPatrolDistance
    {
        get
        {
            return maxPatrolDistance;
        }
    }

    public event Action<int, int> HpChanged;//当前血量和最大血量发生变化 

    public event Action<int> Damaged;//怪物受到伤害

    public EnemyModel()
    {
        ResetData();//创建怪物数据时恢复默认状态
    }

    public void ResetData()//怪物重新生成时重置数据
    {
        curHp = maxHp;
        HpChanged?.Invoke(curHp, maxHp);//通知血条刷新
    }

    private void SetHp(int hp)//设置怪物当前生命值
    {
        int newHp = Mathf.Clamp(hp, 0, maxHp);
        if (curHp == newHp)
        {
            return;
        }
        curHp = newHp;
        HpChanged?.Invoke(curHp, maxHp);//通知血条刷新
    }

    public int TakeDamage(int atk)
    {
        if (atk <= 0 || IsDead())
        {
            return 0;
        }

        int damage = Mathf.Max(1, atk - def);

        SetHp(curHp - damage);
        Damaged?.Invoke(damage);

        return damage;
    }

    public bool IsDead()//判断怪物是否死亡
    {
        return curHp <= 0;
    }
}