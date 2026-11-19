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
    MsgCharacterHp = 15,//同步角色血量
    MsgCharacterRevive = 16,//角色重生
    MsgCharacterLevelUp = 17,//角色升级
    MsgKillEnemy = 18,//击杀敌人任务协议
    MsgExpandBag = 19,//背包扩容协议
}