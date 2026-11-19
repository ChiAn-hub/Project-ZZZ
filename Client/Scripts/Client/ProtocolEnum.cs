using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum ProtocolEnum
{
    None = 0,//没有消息
    MsgSecret = 1,//秘钥协议
    MsgPing = 2,//心跳包协议
    MsgRegister = 3,//注册协议
    MsgLogin = 4,//登录协议
    MsgSetName = 5,//昵称协议
    MsgPlayerInfo = 6,//玩家信息协议  
    MsgBag = 7,//背包协议
    MsgShop = 8,//商店协议
    MsgBuyItem = 9,//购买物品协议
    MsgRecycleItems = 10,//回收物品协议
    MsgCharacterInfo = 11,//获取角色信息协议
    MsgEquipItem = 12,//穿戴装备协议
    MsgQuest = 13,//任务协议
    MsgMoveDistance = 14,//上报移动距离
    MsgCharacterHp = 15,//角色生命值协议
    MsgCharacterRevive = 16,//角色复活协议
    MsgCharacterLevelUp = 17,//角色升级
    MsgKillEnemy = 18,//击杀敌人任务协议
    MsgExpandBag = 19,//背包扩容协议
}
public enum RegisterType//注册的结果
{
    Success,//注册成功
    Failed,//注册失败
    UserExist,//用户已存在
}
public enum LoadType//登录的结果
{
    Success,//登录成功
    Failed,//登录失败
    PasswordError,//密码错误
    NoUser,//没有该用户
}
public enum SetNameType//设置昵称的结果
{
    Success,//设置成功
    Failed,//设置失败
}
public enum BuyResult
{
    None = 0,
    Success = 1,
    NotLogin = 2,
    InvalidCount = 3,
    ProductNotFound = 4,
    NotEnoughGold = 5,
    ServerError = 6
}
public enum RecycleResult
{
    None = 0,
    Success = 1,
    NotLogin = 2,
    InvalidSelection = 3,
    ItemNotFound = 4,
    ItemNotRecyclable = 5,
    ServerError = 6,
    ItemEquipped = 7
}
public enum EquipAction
{
    Equip = 1,
    Unequip = 2
}

public enum EquipResult
{
    None = 0,
    Success = 1,
    NotLogin = 2,
    InvalidAction = 3,
    InvalidCharacter = 4,
    ItemNotOwned = 5,
    NotWeapon = 6,
    ServerError = 7
}
public enum QuestAction
{
    GetList = 1,
    Accept = 2,
    GiveUp = 3,
    GetReward = 4//领取奖励
}

public enum QuestOperationResult
{
    None = 0,
    Success = 1,
    NotLogin = 2,
    QuestNotFound = 3,
    AlreadyAccepted = 4,
    InvalidState = 5,
    ServerError = 6,
    InvalidAction = 7,
    InvalidAmount = 8,
}

public enum LevelUpResult
{
    None = 0,
    Success = 1,
    NotLogin = 2,
    InvalidCharacter = 3,
    InvalidItems = 4,
    NotEnoughItems = 5,
    MaxLevel = 6,
    ServerError = 7
}

public enum ExpandBagResult
{
    None = 0,
    Success = 1,
    NotLogin = 2,
    InvalidCount = 3,
    MaxCapacity = 4,
    ServerError = 5
}