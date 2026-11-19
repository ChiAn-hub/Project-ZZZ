using System;
using UnityEngine;

public class PlayerModel : Singleton<PlayerModel>
{
    private string playerName;
    private int playerLevel;
    private long playerExp;
    private long playerGold;
    private long playerSp;
    private long playerMoney;
    public string PlayerName => playerName;
    public int PlayerLevel => playerLevel;
    public long PlayerExp => playerExp;
    public long PlayerGold => playerGold;
    public long PlayerSp => playerSp;
    public long PlayerMoney => playerMoney;

    //观察者模式,用于在数据更新时通知UI
    public event Action<string> NameChanged;
    public event Action<int> LevelChanged;
    public event Action<long> ExpChanged;
    public event Action<long> GoldChanged;
    public event Action<long> SpChanged;
    public event Action<long> MoneyChanged;

    public void ApplyData(MsgPlayerInfo data)//外部更新数据接口
    {
        if (data == null || !data.Success)
        {
            return;
        }
        SetName(data.PlayerName);
        SetLevel(data.Level);
        SetExp(data.Exp);
        SetGold(data.Gold);
        SetMoney(data.Money);
        SetSp(data.Sp);
    }
    private void SetName(string name)//更新名字
    {
        if (playerName == name)
        {
            return;
        }
        playerName = name;
        NameChanged?.Invoke(playerName);
    }
    private void SetLevel(int level)//更新等级
    {
        if (playerLevel == level)
        {
            return;
        }
        playerLevel = level;
        LevelChanged?.Invoke(playerLevel);
    }
    private void SetExp(long exp)//更新经验
    {
        if (playerExp == exp)
        {
            return;
        }
        playerExp = exp;
        ExpChanged?.Invoke(playerExp);
    }
    private void SetGold(long gold)//更新金币
    {
        if (playerGold == gold)
        {
            return;
        }
        playerGold = gold;
        GoldChanged?.Invoke(playerGold);
    }
    private void SetMoney(long money)//更新钱钱
    {
        if (playerMoney == money)
        {
            return;
        }
        playerMoney = money;
        MoneyChanged?.Invoke(playerMoney);
    }
    private void SetSp(long sp)//更新体力
    {
        if (playerSp == sp)
        {
            return;
        }
        playerSp = sp;
        SpChanged?.Invoke(playerSp);
    }
    public void ApplyGold(long gold)//外部更新金币接口
    {
        SetGold(gold);
    }
}
