using System;
using UnityEngine;

public class EllenDataModel : Singleton<EllenDataModel>
{
    private int characterId = 1;//角色ID
    private int curHp;
    private int maxHp;
    private int attack;
    private int impact;
    private int defense;
    private long exp;
    private int level;
    private float critical;
    private float criticalDamage;
    private int equippedItemId;

    public int CharacterId => characterId;
    public int CurHp => curHp;
    public int MaxHp => maxHp;
    public int Attack => attack;
    public int Impact => impact;
    public int Defense => defense;
    public long Exp => exp;
    public int Level => level;
    public float Critical => critical;
    public float CriticalDamage => criticalDamage;
    public int EquippedItemId => equippedItemId;
    public event Action DataChanged;
    public event Action<int, int> HpChanged;//当前血量、最大血量
    public void ApplyData(MsgCharacterInfo data)
    {
        if (data == null || !data.Success)
        {
            return;
        }

        characterId = data.CharacterId;

        curHp = data.CurHp;
        maxHp = data.MaxHp;
        attack = data.Attack;
        impact = data.Impact;
        defense = data.Defense;

        exp = data.Exp;
        level = data.Level;

        critical = data.CriticalRate / 100f;
        criticalDamage = data.CriticalDamage / 100f;
        equippedItemId = data.EquippedItemId;

        HpChanged?.Invoke(curHp, maxHp);
        DataChanged?.Invoke();
    }

    public int TakeDamage(int enemyAtk)
    {
        if (enemyAtk <= 0 || IsDead())
        {
            return 0;
        }
        int damage = Mathf.Max(1, enemyAtk - defense);

        SetHp(curHp - damage);

        return damage;
    }

    private void SetHp(int hp)//设置当前血量
    {
        int newHp = Mathf.Clamp(hp, 0, maxHp);

        if (curHp == newHp)
        {
            return;
        }

        curHp = newHp;

        HpChanged?.Invoke(curHp, maxHp);
        DataChanged?.Invoke();
    }

    public void ApplyRevive(int hp, int hpMax)//应用重生后的血量
    {
        maxHp = Mathf.Max(1, hpMax);
        curHp = Mathf.Clamp(hp, 0, maxHp);

        HpChanged?.Invoke(curHp, maxHp);
        DataChanged?.Invoke();
    }

    public bool IsDead()//判断角色是否死亡
    {
        return curHp <= 0;
    }

}