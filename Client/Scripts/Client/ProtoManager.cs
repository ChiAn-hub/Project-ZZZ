using System;
using SimpleServer.Proto;
using UnityEngine;
using System.Collections.Generic;
public class ProtoManager : Singleton<ProtoManager>
{
    public void CSSecret()//请求秘钥
    {
        MsgSecret msg = new MsgSecret();
        NetManager.Instance.AddProto(ProtocolEnum.MsgSecret, CSSecretCallBack);
        NetManager.Instance.Send(msg);
    }
    public void CSSecretCallBack(MsgBase msgBase)//请求秘钥回调
    {
        NetManager.Instance.SecretKey = ((MsgSecret)msgBase).SecretKey;
    }
    public void CSRegister(string Account, string Password, Action<RegisterType> callback)//注册
    {
        MsgRegister msg = new MsgRegister();
        msg.Account = Account;
        msg.Password = Password;
        NetManager.Instance.AddProto(ProtocolEnum.MsgRegister, (msgBase) =>
        {
            MsgRegister msgRegister = (MsgRegister)msgBase;
            callback(msgRegister.RegisterType);
        });
        NetManager.Instance.Send(msg);
    }
    public void CSLogin(string Account, string Password, Action<LoadType, bool> callback)//登录
    {
        MsgLogin msg = new MsgLogin();
        msg.Account = Account;
        msg.Password = Password;
        NetManager.Instance.AddProto(ProtocolEnum.MsgLogin, (msgBase) =>
       {
           MsgLogin msgLogin = (MsgLogin)msgBase;
           callback?.Invoke(msgLogin.LoadType, msgLogin.IsSetName);
       }
       );
        NetManager.Instance.Send(msg);
    }
    public void CSSetName(string Name, Action<SetNameType, string> callback)//设置名字
    {
        MsgSetName msg = new MsgSetName();
        msg.Name = Name;
        NetManager.Instance.AddProto(ProtocolEnum.MsgSetName, (msgBase) =>
        {
            MsgSetName msgSetName = (MsgSetName)msgBase;
            callback(msgSetName.SetNameType, msg.Name);
        }
        );
        NetManager.Instance.Send(msg);
    }
    public void CSBag(Action<bool, List<BagItemData>, int> callback)//获取背包
    {
        MsgBag msgBag = new MsgBag();
        NetManager.Instance.AddProto(ProtocolEnum.MsgBag, (msgBase) =>
        {
            MsgBag msg = (MsgBag)msgBase;
            callback?.Invoke(msg.Success, msg.Items, msg.Capacity);
        });
        NetManager.Instance.Send(msgBag);
    }
    public void CSExpandBag(int addCount, Action<MsgExpandBag> callback)//背包扩容
    {
        MsgExpandBag msg = new MsgExpandBag();
        msg.AddCount = addCount;
        NetManager.Instance.AddProto(ProtocolEnum.MsgExpandBag, (msgBase) =>
        {
            MsgExpandBag msgExpandBag = (MsgExpandBag)msgBase;
            callback?.Invoke(msgExpandBag);
        });
        NetManager.Instance.Send(msg);
    }
    public void CSPlayerInfo(Action<bool, MsgPlayerInfo> callback)//获取玩家信息
    {
        MsgPlayerInfo msg = new MsgPlayerInfo();
        NetManager.Instance.AddProto(ProtocolEnum.MsgPlayerInfo, (msgBase) =>
        {
            MsgPlayerInfo msgPlayerInfo = (MsgPlayerInfo)msgBase;
            if (msgPlayerInfo == null)
            {
                callback?.Invoke(false, null);
            }
            else
            {
                callback?.Invoke(msgPlayerInfo.Success, msgPlayerInfo);
            }
        });
        NetManager.Instance.Send(msg);
    }
    public void CSShop(Action<bool, List<ShopProductData>> callback)//获取商品信息
    {
        MsgShop msg = new MsgShop();
        NetManager.Instance.AddProto(ProtocolEnum.MsgShop, (msgBase) =>
        {
            MsgShop msgShop = (MsgShop)msgBase;
            if (msgShop == null)
            {
                callback?.Invoke(false, null);
                return;
            }
            callback?.Invoke(msgShop.Success, msgShop.Products);
        });
        NetManager.Instance.Send(msg);
    }
    public void CSBuyItem(int productId, int buyCount, Action<MsgBuyItem> callback)////购买商品
    {
        MsgBuyItem msg = new MsgBuyItem();
        msg.ProductId = productId;
        msg.BuyCount = buyCount;
        NetManager.Instance.AddProto(ProtocolEnum.MsgBuyItem, (msgBase) =>
        {
            MsgBuyItem msgBuyItem = (MsgBuyItem)msgBase;
            callback?.Invoke(msgBuyItem);
        });
        NetManager.Instance.Send(msg);
    }
    public void CSRecycleItems(List<int> itemIds, Action<MsgRecycleItems> callback)//回收物品
    {
        MsgRecycleItems msg = new MsgRecycleItems();
        if (itemIds != null)
        {
            msg.ItemIds.AddRange(itemIds);
        }

        NetManager.Instance.AddProto(ProtocolEnum.MsgRecycleItems, (msgBase) =>
        {
            MsgRecycleItems msgRecycleItems = (MsgRecycleItems)msgBase;
            callback?.Invoke(msgRecycleItems);
        });
        NetManager.Instance.Send(msg);
    }
    public void CSCharacterInfo(int characterId, Action<bool, MsgCharacterInfo> callback)//获取角色数据
    {
        MsgCharacterInfo msg = new MsgCharacterInfo();
        msg.CharacterId = characterId;
        NetManager.Instance.AddProto(ProtocolEnum.MsgCharacterInfo, (msgBase) =>
        {
            MsgCharacterInfo msgCharacterInfo = (MsgCharacterInfo)msgBase;
            if (msgCharacterInfo == null)
            {
                callback?.Invoke(false, null);
                return;
            }
            callback?.Invoke(msgCharacterInfo.Success, msgCharacterInfo);
        });
        NetManager.Instance.Send(msg);
    }
    public void CSEquipItem(int characterId, int itemId, EquipAction action, Action<MsgEquipItem> callback)//装备武器
    {
        MsgEquipItem msg = new MsgEquipItem();

        msg.CharacterId = characterId;
        msg.ItemId = itemId;
        msg.Action = action;
        NetManager.Instance.AddProto(ProtocolEnum.MsgEquipItem, (msgBase) =>
       {
           MsgEquipItem result = msgBase as MsgEquipItem;
           callback?.Invoke(result);
       });
        NetManager.Instance.Send(msg);
    }
    public void CSQuest(QuestAction action, int questId, Action<MsgQuest> callback)//任务请求
    {
        MsgQuest msg = new MsgQuest
        {
            Action = action,
            QuestId = questId
        };
        NetManager.Instance.AddProto(ProtocolEnum.MsgQuest, (msgBase) =>
        {
            MsgQuest msgQuest = msgBase as MsgQuest;
            callback?.Invoke(msgQuest);
        });
        NetManager.Instance.Send(msg);
    }
    public void CSMoveDistance(double distance, Action<MsgMoveDistance> callback)//发送移动距离
    {
        MsgMoveDistance msg = new MsgMoveDistance
        {
            Distance = distance
        };
        NetManager.Instance.AddProto(ProtocolEnum.MsgMoveDistance, (msgBase) =>
        {
            MsgMoveDistance msgMove = (MsgMoveDistance)msgBase;
            callback?.Invoke(msgMove);
        });
        NetManager.Instance.Send(msg);
    }
    public void CSCharacterHp(int characterId, int hp, Action<MsgCharacterHp> callback)//发送角色血量
    {
        MsgCharacterHp msg = new MsgCharacterHp();
        msg.CharacterId = characterId;
        msg.CurHp = hp;
        NetManager.Instance.AddProto(ProtocolEnum.MsgCharacterHp, (msgBase) =>
        {
            MsgCharacterHp result = (MsgCharacterHp)msgBase;
            callback?.Invoke(result);
        });
        NetManager.Instance.Send(msg);
    }
    public void CSCharacterRevive(int characterId, Action<MsgCharacterRevive> callback)
    {
        MsgCharacterRevive msg = new MsgCharacterRevive();

        msg.CharacterId = characterId;

        NetManager.Instance.AddProto(ProtocolEnum.MsgCharacterRevive, (msgBase) =>
            {
                MsgCharacterRevive result = msgBase as MsgCharacterRevive;
                callback?.Invoke(result);
            });

        NetManager.Instance.Send(msg);
    }

    public void CSCharacterLevelUp(int characterId, List<ExpItemUseData> items, Action<MsgCharacterLevelUp> callback)//角色升级
    {
        MsgCharacterLevelUp msg = new MsgCharacterLevelUp();

        msg.CharacterId = characterId;

        if (items != null)
        {
            msg.Items.AddRange(items);
        }

        NetManager.Instance.AddProto(ProtocolEnum.MsgCharacterLevelUp, (msgBase) =>
        {
            MsgCharacterLevelUp result = (MsgCharacterLevelUp)msgBase;
            callback?.Invoke(result);
        });

        NetManager.Instance.Send(msg);
    }

    public void CSKillEnemy(Action<MsgKillEnemy> callback)//击杀敌人
    {
        MsgKillEnemy msg = new MsgKillEnemy();
        NetManager.Instance.AddProto(ProtocolEnum.MsgKillEnemy, (msgBase) =>
        {
            MsgKillEnemy Result = (MsgKillEnemy)msgBase;
            callback?.Invoke(Result);
        });
        NetManager.Instance.Send(msg);
    }
}
