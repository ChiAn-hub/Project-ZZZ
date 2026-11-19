using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using ProtoBuf;
using SimpleServer.Proto;

[ProtoContract]
public class MsgSecret : MsgBase
{
    public MsgSecret()
    {
        ProtocolType = ProtocolEnum.MsgSecret;
    }
    [ProtoMember(1)]
    public override ProtocolEnum ProtocolType { get; set; }
    [ProtoMember(2)]
    public string SecretKey { get; set; }
}

[ProtoContract]
public class MsgPing : MsgBase
{
    public MsgPing()
    {
        ProtocolType = ProtocolEnum.MsgPing;
    }
    [ProtoMember(1)]
    public override ProtocolEnum ProtocolType { get; set; }
}
[ProtoContract]
public class MsgRegister : MsgBase
{
    public MsgRegister()
    {
        ProtocolType = ProtocolEnum.MsgRegister;
    }
    [ProtoMember(1)]
    public override ProtocolEnum ProtocolType { get; set; }
    [ProtoMember(2)]
    public string Account { get; set; }
    [ProtoMember(3)]
    public string Password { get; set; }
    [ProtoMember(4)]
    public RegisterType RegisterType { get; set; }
}
[ProtoContract]
public class MsgLogin : MsgBase
{
    public MsgLogin()
    {
        ProtocolType = ProtocolEnum.MsgLogin;
    }
    [ProtoMember(1)]
    public override ProtocolEnum ProtocolType { get; set; }
    [ProtoMember(2)]
    public string Account { get; set; }
    [ProtoMember(3)]
    public string Password { get; set; }
    [ProtoMember(4)]
    public LoadType LoadType { get; set; }
    [ProtoMember(5)]
    public bool IsSetName { get; set; }
}
[ProtoContract]
public class MsgSetName : MsgBase
{
    public MsgSetName()
    {
        ProtocolType = ProtocolEnum.MsgSetName;
    }
    [ProtoMember(1)]
    public override ProtocolEnum ProtocolType { get; set; }
    [ProtoMember(2)]
    public string Name { get; set; }
    [ProtoMember(3)]
    public SetNameType SetNameType { get; set; }
}
[ProtoContract]
public class MsgPlayerInfo : MsgBase
{
    public MsgPlayerInfo()
    {
        ProtocolType = ProtocolEnum.MsgPlayerInfo;
    }

    [ProtoMember(1)]
    public override ProtocolEnum ProtocolType { get; set; }

    [ProtoMember(2)]
    public bool Success { get; set; }

    [ProtoMember(3)]
    public string PlayerName { get; set; }

    [ProtoMember(4)]
    public int Level { get; set; }

    [ProtoMember(5)]
    public long Exp { get; set; }

    [ProtoMember(6)]
    public long Gold { get; set; }
    [ProtoMember(7)]
    public long Money { get; set; }
    [ProtoMember(8)]
    public long Sp { get; set; }
}
[ProtoContract]
public class BagItemData
{
    [ProtoMember(1)]
    public int ItemId { get; set; }
    [ProtoMember(2)]
    public int Count { get; set; }
    [ProtoMember(3)]
    public int EquippedCharacterId { get; set; }
}

[ProtoContract]
public class MsgBag : MsgBase
{
    public MsgBag()
    {
        ProtocolType = ProtocolEnum.MsgBag;
        Items = new List<BagItemData>();
    }

    [ProtoMember(1)]
    public override ProtocolEnum ProtocolType { get; set; }

    [ProtoMember(2)]
    public bool Success { get; set; }

    [ProtoMember(3)]
    public List<BagItemData> Items { get; set; }
    [ProtoMember(4)]
    public int Capacity { get; set; }
}

[ProtoContract]
public class MsgExpandBag : MsgBase
{
    public MsgExpandBag()
    {
        ProtocolType = ProtocolEnum.MsgExpandBag;
    }
    [ProtoMember(1)]
    public override ProtocolEnum ProtocolType { get; set; }
    [ProtoMember(2)]
    public int AddCount { get; set; }//客户端发送，表示要扩容的数量
    [ProtoMember(3)]
    public ExpandBagResult Result { get; set; }//服务器返回结果
    [ProtoMember(4)]
    public int Capacity { get; set; }//服务器返回扩容后的容量
}

[ProtoContract]
public class ShopProductData
{
    [ProtoMember(1)]
    public int ProductId { get; set; }

    [ProtoMember(2)]
    public int ItemId { get; set; }

    [ProtoMember(3)]
    public int UnitCount { get; set; }

    [ProtoMember(4)]
    public long Price { get; set; }
}

[ProtoContract]
public class MsgShop : MsgBase
{
    public MsgShop()
    {
        ProtocolType = ProtocolEnum.MsgShop;
        Products = new List<ShopProductData>();
    }

    [ProtoMember(1)]
    public override ProtocolEnum ProtocolType { get; set; }

    [ProtoMember(2)]
    public bool Success { get; set; }

    [ProtoMember(3)]
    public List<ShopProductData> Products { get; set; }
}
[ProtoContract]
public class MsgBuyItem : MsgBase
{
    public MsgBuyItem()
    {
        ProtocolType = ProtocolEnum.MsgBuyItem;
        Items = new List<BagItemData>();
    }
    [ProtoMember(1)]
    public override ProtocolEnum ProtocolType { get; set; }
    [ProtoMember(2)]
    public int ProductId { get; set; }//客户端发送，表示购买哪个商品
    [ProtoMember(3)]
    public int BuyCount { get; set; }//客户端发送，表示购买多少份商品
    [ProtoMember(4)]
    public BuyResult Result { get; set; }//服务器返回结果
    [ProtoMember(5)]
    public long Gold { get; set; }//服务器返回购买后的金币
    [ProtoMember(6)]
    public List<BagItemData> Items { get; set; }//服务器返回购买后的完整背包
}
[ProtoContract]
public class MsgRecycleItems : MsgBase
{
    public MsgRecycleItems()
    {
        ProtocolType = ProtocolEnum.MsgRecycleItems;
        ItemIds = new List<int>();
        Items = new List<BagItemData>();
    }

    [ProtoMember(1)]
    public override ProtocolEnum ProtocolType { get; set; }

    [ProtoMember(2)]
    public List<int> ItemIds { get; set; }// 客户端发送：要回收的物品ID

    [ProtoMember(3)]
    public RecycleResult Result { get; set; }// 服务端返回：处理结果

    [ProtoMember(4)]
    public long Gold { get; set; }// 服务端返回：回收后的金币

    [ProtoMember(5)]
    public List<BagItemData> Items { get; set; }// 服务端返回：回收后的完整背包
}
[ProtoContract]
public class MsgCharacterInfo : MsgBase
{
    public MsgCharacterInfo()
    {
        ProtocolType = ProtocolEnum.MsgCharacterInfo;
    }

    [ProtoMember(1)]
    public override ProtocolEnum ProtocolType { get; set; }

    [ProtoMember(2)]
    public bool Success { get; set; }

    [ProtoMember(3)]
    public int CharacterId { get; set; }

    [ProtoMember(4)]
    public int Level { get; set; }

    [ProtoMember(5)]
    public long Exp { get; set; }

    [ProtoMember(6)]
    public int CurHp { get; set; }

    [ProtoMember(7)]
    public int MaxHp { get; set; }

    [ProtoMember(8)]
    public int Attack { get; set; }

    [ProtoMember(9)]
    public int Impact { get; set; }

    [ProtoMember(10)]
    public int Defense { get; set; }

    [ProtoMember(11)]
    public int CriticalRate { get; set; }

    [ProtoMember(12)]
    public int CriticalDamage { get; set; }
    [ProtoMember(13)]
    public int EquippedItemId { get; set; }
}
[ProtoContract]
public class MsgEquipItem : MsgBase
{
    public MsgEquipItem()
    {
        ProtocolType = ProtocolEnum.MsgEquipItem;
    }

    [ProtoMember(1)]
    public override ProtocolEnum ProtocolType { get; set; }

    [ProtoMember(2)]
    public EquipAction Action { get; set; }

    [ProtoMember(3)]
    public int CharacterId { get; set; }

    [ProtoMember(4)]
    public int ItemId { get; set; }

    [ProtoMember(5)]
    public EquipResult Result { get; set; }
}
[ProtoContract]
public class QuestProgressData//单个任务数据
{
    [ProtoMember(1)]
    public int QuestId { get; set; }

    [ProtoMember(2)]
    public QuestState State { get; set; }

    [ProtoMember(3)]
    public double CurNum { get; set; }
}

[ProtoContract]
public class MsgQuest : MsgBase
{
    public MsgQuest()
    {
        ProtocolType = ProtocolEnum.MsgQuest;
        Quests = new List<QuestProgressData>();
    }

    [ProtoMember(1)]
    public override ProtocolEnum ProtocolType { get; set; }

    [ProtoMember(2)]
    public QuestAction Action { get; set; }//客户端执行什么操作

    [ProtoMember(3)]
    public int QuestId { get; set; }//对应任务ID

    [ProtoMember(4)]
    public QuestOperationResult Result { get; set; }//操作结果

    [ProtoMember(5)]
    public List<QuestProgressData> Quests { get; set; }//玩家当前全部任务
}
[ProtoContract]
public class MsgMoveDistance : MsgBase
{
    public MsgMoveDistance()
    {
        ProtocolType = ProtocolEnum.MsgMoveDistance;
    }

    [ProtoMember(1)]
    public override ProtocolEnum ProtocolType { get; set; }

    [ProtoMember(2)]
    public double Distance { get; set; }

    [ProtoMember(3)]
    public QuestOperationResult Result { get; set; }
}

[ProtoContract]
public class MsgCharacterHp : MsgBase
{
    public MsgCharacterHp()
    {
        ProtocolType = ProtocolEnum.MsgCharacterHp;
    }

    [ProtoMember(1)]
    public override ProtocolEnum ProtocolType { get; set; }

    [ProtoMember(2)]
    public int CharacterId { get; set; }//角色ID

    [ProtoMember(3)]
    public int CurHp { get; set; }//当前血量

    [ProtoMember(4)]
    public bool Success { get; set; }//是否同步成功
}

[ProtoContract]
public class MsgCharacterRevive : MsgBase
{
    public MsgCharacterRevive()
    {
        ProtocolType = ProtocolEnum.MsgCharacterRevive;
    }

    [ProtoMember(1)]
    public override ProtocolEnum ProtocolType { get; set; }

    [ProtoMember(2)]
    public int CharacterId { get; set; }//角色ID

    [ProtoMember(3)]
    public bool Success { get; set; }//是否重生成功

    [ProtoMember(4)]
    public int CurHp { get; set; }//重生后的当前血量

    [ProtoMember(5)]
    public int MaxHp { get; set; }//重生后的最大血量
}

[ProtoContract]
public class ExpItemUseData
{
    [ProtoMember(1)]
    public int ItemId { get; set; }

    [ProtoMember(2)]
    public int Count { get; set; }
}

[ProtoContract]
public class MsgCharacterLevelUp : MsgBase
{
    public MsgCharacterLevelUp()
    {
        ProtocolType = ProtocolEnum.MsgCharacterLevelUp;
        Items = new List<ExpItemUseData>();
    }

    [ProtoMember(1)]
    public override ProtocolEnum ProtocolType { get; set; }

    [ProtoMember(2)]
    public int CharacterId { get; set; }

    [ProtoMember(3)]
    public List<ExpItemUseData> Items { get; set; }

    [ProtoMember(4)]
    public LevelUpResult Result { get; set; }
}

[ProtoContract]
public class MsgKillEnemy : MsgBase//击杀敌人
{
    public MsgKillEnemy()
    {
        ProtocolType = ProtocolEnum.MsgKillEnemy;
    }

    [ProtoMember(1)]
    public override ProtocolEnum ProtocolType { get; set; }

    [ProtoMember(2)]
    public QuestOperationResult Result { get; set; }
}