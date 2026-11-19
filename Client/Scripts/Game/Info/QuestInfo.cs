using System;
using System.Collections.Generic;
using UnityEngine;
public enum QuestState
{
    NoAccept,//任务接取
    Accept,//任务进行中
    FinishButNotGetReward,//任务完成未领取奖励
    FinishAndGetReward,//任务完成并且领取奖励
}
public enum QuestType
{
    Fight,
    Spend,
    CharactorDevelopment,
    WeaponDevelopment,
    Daily,
    Move
}
public enum QuestTarget
{
    None,
    BuyItem,            // 购买商品
    EquipWeapon,        // 穿戴武器
    CharacterLevelUp,   // 角色升级
    KillEnemy,          // 击杀敌人
    EnhanceWeapon,      // 强化武器
    CompleteQuest,      // 完成任务
    ChangeWeapon,       // 更换武器
    MoveDistance        // 移动距离
}
[Serializable]
public class QuestInfo
{
    public int Id;
    public int Num;
    public int CharactorExp;
    public int WeaponExp;
    public int PlayerExp;
    public int PlayerGold;
    public string Type;
    public string Name;
    public string Content;
    public string Target;
    [NonSerialized]
    public QuestTarget TargetType;
}
[Serializable]
public class QuestInfoList
{
    public List<QuestInfo> QuestList;
}
public class PlayerQuest
{
    public QuestState State;
    public float CurNum;
}
