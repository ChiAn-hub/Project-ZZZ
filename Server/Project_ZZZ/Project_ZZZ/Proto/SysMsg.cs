using System;
using System.Collections.Generic;
using Project_ZZZ.mysql;
using Project_ZZZ.Net;
using SimpleServer.Proto;

namespace Project_ZZZ.Proto
{
    public class SysMsg
    {
        public static void MsgSecret(ClientSocket clientSocket, MsgBase msgBase)//处理秘钥协议
        {
            Debug.Log("收到秘钥包");
            MsgSecret msgSecret = msgBase as MsgSecret;//将得到的消息转换成对应消息类型
            msgSecret.SecretKey = ServerSocket.Instance.SecretKey;//获取秘钥
            ServerSocket.Instance.SendDate(clientSocket, msgSecret);//发送秘钥
        }

        public static void MsgPing(ClientSocket clientSocket, MsgBase msgBase)//心跳包协议
        {
            Debug.Log("收到心跳包");
            MsgPing msgPing = msgBase as MsgPing;
            clientSocket.lastReceiveTime = ServerSocket.Instance.GetTimeStamp();
            ServerSocket.Instance.SendDate(clientSocket, msgBase);
        }

        public static void MsgRegister(ClientSocket clientSocket, MsgBase msgBase)//没有消息
        {
            Debug.Log("收到注册包");
            MsgRegister msgRegister = msgBase as MsgRegister;
            int userId;
            RegisterType registerType = UserManager.Instance.Register(msgRegister.Account, msgRegister.Password, out userId);
            if (registerType == RegisterType.Success)
            {
                clientSocket.UserId = userId;//记录下对应客户端的数据库ID
            }
            msgRegister.RegisterType = registerType;
            msgRegister.Password = null;
            ServerSocket.Instance.SendDate(clientSocket, msgRegister);
        }

        public static void MsgLogin(ClientSocket clientSocket, MsgBase msgBase)//登录消息
        {
            Debug.Log("收到登录包");
            MsgLogin msgLogin = msgBase as MsgLogin;
            int userId;
            bool isSetName;
            LoadType loadType = UserManager.Instance.Load(msgLogin.Account, msgLogin.Password, out userId, out isSetName);
            if (loadType == LoadType.Success)
            {
                clientSocket.UserId = userId;
            }
            msgLogin.LoadType = loadType;
            msgLogin.IsSetName = isSetName;
            msgLogin.Password = null;
            ServerSocket.Instance.SendDate(clientSocket, msgLogin);
        }
        public static void MsgSetName(ClientSocket clientSocket, MsgBase msgBase)//设置昵称消息
        {
            Debug.Log("收到设置昵称包");
            MsgSetName msgSetName = msgBase as MsgSetName;
            SetNameType result = UserManager.Instance.SetName(clientSocket.UserId, msgSetName.Name);
            msgSetName.SetNameType = result;
            ServerSocket.Instance.SendDate(clientSocket, msgSetName);
        }
        public static void MsgBag(ClientSocket clientSocket, MsgBase msgBase)//获取背包消息
        {
            Debug.Log("收到获取背包");
            MsgBag msgBag = msgBase as MsgBag;
            if (msgBag == null)
            {
                return;
            }
            msgBag.Items = new List<BagItemData>();
            if (clientSocket.UserId <= 0)//如果没有登陆
            {
                msgBag.Success = false;//返回失败
                ServerSocket.Instance.SendDate(clientSocket, msgBag);
                return;
            }
            List<PlayerItem> playerItems = BagManager.Instance.GetItems(clientSocket.UserId);//获取服务器上的背包数据
            int capacity = BagManager.Instance.GetCapacity(clientSocket.UserId);//获取背包容量

            if (playerItems == null || capacity <= 0)
            {
                msgBag.Success = false;//返回失败
            }
            else
            {
                msgBag.Success = true;//返回成功
                msgBag.Items = BuildBagItems(clientSocket.UserId, playerItems);
                msgBag.BagCapacity = capacity;
            }
            ServerSocket.Instance.SendDate(clientSocket, msgBag);
        }
        public static void MsgPlayerInfo(ClientSocket clientSocket, MsgBase msgBase)//获取玩家信息消息
        {
            Debug.Log("收到获取玩家信息");
            MsgPlayerInfo msgPlayerInfo = msgBase as MsgPlayerInfo;
            if (msgPlayerInfo == null)
            {
                return;
            }
            if (clientSocket.UserId <= 0)
            {
                msgPlayerInfo.Success = false;
                ServerSocket.Instance.SendDate(clientSocket, msgPlayerInfo);
                return;
            }
            User user = UserManager.Instance.GetUser(clientSocket.UserId);
            PlayerProfile playerProfile = PlayerManager.Instance.GetPlayerProfile(clientSocket.UserId);
            if (playerProfile == null || user == null)
            {
                msgPlayerInfo.Success = false;
            }
            else
            {
                msgPlayerInfo.Success = true;
                msgPlayerInfo.PlayerName = user.Name;
                msgPlayerInfo.Level = playerProfile.Level;
                msgPlayerInfo.Exp = playerProfile.Exp;
                msgPlayerInfo.Gold = playerProfile.Gold;
                msgPlayerInfo.Money = playerProfile.Money;
                msgPlayerInfo.Sp = playerProfile.Sp;
            }
            ServerSocket.Instance.SendDate(clientSocket, msgPlayerInfo);
        }
        public static void MsgShop(ClientSocket clientSocket, MsgBase msgBase)//获取商店信息消息
        {
            Debug.Log("收到获取商店信息");
            MsgShop msgShop = msgBase as MsgShop;
            if (msgShop == null)
            {
                return;
            }
            msgShop.Products = new List<ShopProductData>();
            if (clientSocket.UserId <= 0)
            {
                msgShop.Success = false;
                ServerSocket.Instance.SendDate(clientSocket, msgShop);
                return;
            }
            List<ShopProduct> shopProducts = ShopManager.Instance.GetProducts();
            if (shopProducts == null)
            {
                msgShop.Success = false;
            }
            else
            {
                msgShop.Success = true;
                foreach (ShopProduct product in shopProducts)
                {
                    ShopProductData shopProductData = new ShopProductData();
                    shopProductData.ProductId = product.ProductId;
                    shopProductData.ItemId = product.ItemId;
                    shopProductData.Price = product.Price;
                    shopProductData.UnitCount = product.UnitCount;
                    msgShop.Products.Add(shopProductData);
                }
            }
            ServerSocket.Instance.SendDate(clientSocket, msgShop);
        }
        public static void MsgBuyItem(ClientSocket clientSocket, MsgBase msgBase)//获取购买商品信息
        {
            Debug.Log("收到购买物品消息");
            MsgBuyItem msgBuyItem = msgBase as MsgBuyItem;
            if (msgBuyItem == null)
            {
                return;
            }
            msgBuyItem.Items = new List<BagItemData>();
            if (clientSocket.UserId <= 0)
            {
                msgBuyItem.Result = BuyResult.NotLogin;
                ServerSocket.Instance.SendDate(clientSocket, msgBuyItem);
                return;
            }
            long curGold;
            List<PlayerItem> curItems;
            msgBuyItem.Result = ShopManager.Instance.BuyProduct(clientSocket.UserId, msgBuyItem.ProductId, msgBuyItem.BuyCount, out curGold, out curItems);
            if (msgBuyItem.Result == BuyResult.Success)
            {
                msgBuyItem.Gold = curGold;
                msgBuyItem.Items = BuildBagItems(clientSocket.UserId, curItems);
            }
            ServerSocket.Instance.SendDate(clientSocket, msgBuyItem);
        }
        public static void MsgRecycleItems(ClientSocket clientSocket, MsgBase msgBase)//获取回收物品信息
        {
            Debug.Log("收到回收物品消息");
            MsgRecycleItems msg = msgBase as MsgRecycleItems;
            if (msg == null)
            {
                return;
            }
            msg.Items = new List<BagItemData>();
            if (clientSocket.UserId <= 0)
            {
                msg.Result = RecycleResult.NotLogin;
                ServerSocket.Instance.SendDate(clientSocket, msg);
                return;
            }
            long curGold;
            List<PlayerItem> curItems;
            msg.Result = BagManager.Instance.RecycleItems(clientSocket.UserId, msg.ItemIds, out curGold, out curItems);
            if (msg.Result == RecycleResult.Success)
            {
                msg.Gold = curGold;

                msg.Items = BuildBagItems(clientSocket.UserId, curItems);
            }
            ServerSocket.Instance.SendDate(clientSocket, msg);
        }
        public static void MsgCharacterInfo(ClientSocket clientSocket, MsgBase msgBase)//获取角色信息
        {
            Debug.Log("收到获取角色信息消息");
            MsgCharacterInfo msg = msgBase as MsgCharacterInfo;
            if (msg == null)
            {
                return;
            }
            if (clientSocket.UserId <= 0)
            {
                msg.Success = false;
                ServerSocket.Instance.SendDate(clientSocket, msg);
                return;
            }
            PlayerCharacter character = CharacterManager.Instance.GetCharacter(clientSocket.UserId, msg.CharacterId);
            if (character == null)
            {
                msg.Success = false;
            }
            else
            {
                msg.Success = true;
                ItemDefinition equipment = EquipmentManager.Instance.GetEquippedItem(clientSocket.UserId, character.CharacterId);
                msg.EquippedItemId = equipment == null ? 0 : equipment.ItemId;//获取装备的武器属性
                int maxHpBonus = equipment == null ? 0 : equipment.MaxHpBonus;
                int attackBonus = equipment == null ? 0 : equipment.AttackBonus;
                int impactBonus = equipment == null ? 0 : equipment.ImpactBonus;
                int defenseBonus = equipment == null ? 0 : equipment.DefenseBonus;
                int criticalRateBonus = equipment == null ? 0 : equipment.CriticalRateBonus;
                int criticalDamageBonus = equipment == null ? 0 : equipment.CriticalDamageBonus;

                msg.Level = character.Level;//更新玩家属性
                msg.Exp = character.Exp;
                msg.MaxHp = character.BaseMaxHp + maxHpBonus;
                msg.CurHp = Math.Min(character.CurHp, msg.MaxHp);
                msg.Attack = character.BaseAttack + attackBonus;
                msg.Impact = character.BaseImpact + impactBonus;
                msg.Defense = character.BaseDefense + defenseBonus;
                msg.CriticalRate = character.CriticalRate + criticalRateBonus;
                msg.CriticalDamage = character.CriticalDamage + criticalDamageBonus;
            }
            ServerSocket.Instance.SendDate(clientSocket, msg);
        }
        public static void MsgEquipItem(ClientSocket clientSocket, MsgBase msgBase)//收到装备消息
        {
            Debug.Log("收到装备操作消息");
            MsgEquipItem msg = msgBase as MsgEquipItem;
            if (msg == null)
            {
                return;
            }
            msg.Result = EquipmentManager.Instance.ChangeEquipment(clientSocket.UserId, msg.CharacterId, msg.ItemId, msg.Action);
            ServerSocket.Instance.SendDate(clientSocket, msg);
        }
        private static List<BagItemData> BuildBagItems(int userId, List<PlayerItem> playerItems)//辅助功能,将物品信息和穿戴信息总结起来
        {
            List<BagItemData> result = new List<BagItemData>();
            if (playerItems == null)
            {
                return result;
            }
            Dictionary<int, int> equipmentMap = EquipmentManager.Instance.GetEquipmentMap(userId);//获取装备穿戴的情况
            foreach (PlayerItem playerItem in playerItems)//遍历
            {
                BagItemData itemData = new BagItemData();

                itemData.ItemId = playerItem.ItemId;//玩家所拥有的道具Id
                itemData.Count = playerItem.Count;//玩家所拥有的道具数量

                int characterId;

                if (equipmentMap.TryGetValue(playerItem.ItemId, out characterId))//在装备情况中查找该装备是否被穿戴
                {
                    itemData.EquippedCharacterId = characterId;//穿戴了则把对应的角色ID也存进去
                }

                result.Add(itemData);
            }

            return result;
        }
        public static void MsgQuest(ClientSocket clientSocket, MsgBase msgBase)//获取任务消息
        {
            Debug.Log("收到任务消息");
            MsgQuest msg = msgBase as MsgQuest;
            if (msg == null)
            {
                return;
            }
            msg.Quests.Clear();
            if (clientSocket.UserId <= 0)
            {
                msg.Result = QuestOperationResult.NotLogin;
                ServerSocket.Instance.SendDate(clientSocket, msg);
                return;
            }
            switch (msg.Action)//根据对应的操作执行对应的方法
            {
                case QuestAction.Accept:
                    msg.Result = QuestManage.Instance.AcceptQuest(clientSocket.UserId, msg.QuestId);
                    break;
                case QuestAction.GiveUp:
                    msg.Result = QuestManage.Instance.GiveUpQuest(clientSocket.UserId, msg.QuestId);
                    break;
                case QuestAction.GetList:
                    msg.Result = QuestOperationResult.Success;
                    break;
                case QuestAction.GetReward:
                    msg.Result = QuestManage.Instance.GetReward(clientSocket.UserId, msg.QuestId);
                    break;
                default:
                    msg.Result = QuestOperationResult.InvalidAction;
                    break;
            }
            List<PlayerQuest> playerQuests = QuestManage.Instance.GetPlayerAllQuest(clientSocket.UserId);
            if (playerQuests == null)
            {
                msg.Result = QuestOperationResult.ServerError;
                ServerSocket.Instance.SendDate(clientSocket, msg);
                return;
            }
            foreach (PlayerQuest playerQuest in playerQuests)
            {
                QuestProgressData data = new QuestProgressData
                {
                    QuestId = playerQuest.QuestId,
                    State = playerQuest.State,
                    CurNum = playerQuest.CurNum,
                };
                msg.Quests.Add(data);
            }
            ServerSocket.Instance.SendDate(clientSocket, msg);
        }

        public static void MsgMoveDistance(ClientSocket clientSocket, MsgBase msgBase)//接收到移动距离消息
        {
            Debug.Log("收到移动距离消息");
            MsgMoveDistance msg = msgBase as MsgMoveDistance;
            if (msg == null)
            {
                return;
            }
            if (clientSocket.UserId <= 0)
            {
                msg.Result = QuestOperationResult.NotLogin;
                ServerSocket.Instance.SendDate(clientSocket, msg);
                return;
            }
            //处理数据异常,IsNaN处理传进来的是否为非数字类型,IsInfinity处理传进来的是否为无穷大,并处理不合法数据
            if (double.IsNaN(msg.Distance) || double.IsInfinity(msg.Distance) || msg.Distance <= 0 || msg.Distance > 5)
            {
                msg.Result = QuestOperationResult.InvalidAmount;
                ServerSocket.Instance.SendDate(clientSocket, msg);
                return;
            }
            try
            {
                QuestEvent.RaiseProgress(clientSocket.UserId, QuestTarget.MoveDistance, msg.Distance);
                msg.Result = QuestOperationResult.Success;
            }
            catch (Exception e)
            {
                Debug.LogError("更新移动任务失败：" + e);
                msg.Result = QuestOperationResult.ServerError;
            }
            ServerSocket.Instance.SendDate(clientSocket, msg);
        }

        public static void MsgCharacterHp(ClientSocket clientSocket, MsgBase msgBase)//接收到角色血量消息
        {
            Debug.Log("收到同步角色血量消息");

            MsgCharacterHp msg = msgBase as MsgCharacterHp;
            if (msg == null)
            {
                return;
            }
            if (clientSocket.UserId <= 0)//如果没有登陆
            {
                msg.Success = false;
                ServerSocket.Instance.SendDate(clientSocket, msg);
                return;
            }
            PlayerCharacter character = CharacterManager.Instance.GetCharacter(clientSocket.UserId, msg.CharacterId);//获取角色信息
            if (character == null)
            {
                msg.Success = false;
                ServerSocket.Instance.SendDate(clientSocket, msg);
                return;
            }
            if (msg.CurHp < 0 || msg.CurHp >= character.CurHp)
            {
                msg.Success = false;
                msg.CurHp = character.CurHp;

                ServerSocket.Instance.SendDate(clientSocket, msg);
                return;
            }

            msg.Success = CharacterManager.Instance.UpdateHp(clientSocket.UserId, msg.CharacterId, msg.CurHp);//更新血量

            ServerSocket.Instance.SendDate(clientSocket, msg);
        }

        public static void MsgCharacterRevive(ClientSocket clientSocket, MsgBase msgBase)//角色重生
        {
            Debug.Log("收到角色重生消息");

            MsgCharacterRevive msg = msgBase as MsgCharacterRevive;

            if (msg == null)
            {
                return;
            }

            if (clientSocket.UserId <= 0)
            {
                msg.Success = false;
                ServerSocket.Instance.SendDate(clientSocket, msg);
                return;
            }

            PlayerCharacter character = CharacterManager.Instance.GetCharacter(clientSocket.UserId, msg.CharacterId);//获取角色信息

            if (character == null)
            {
                msg.Success = false;
                ServerSocket.Instance.SendDate(clientSocket, msg);
                return;
            }

            if (character.CurHp > 0)//角色还活着时不能使用重生协议
            {
                msg.Success = false;
                ServerSocket.Instance.SendDate(clientSocket, msg);
                return;
            }

            ItemDefinition equipment = EquipmentManager.Instance.GetEquippedItem(clientSocket.UserId, msg.CharacterId);

            int maxHpBonus = equipment == null ? 0 : equipment.MaxHpBonus;

            int maxHp = character.BaseMaxHp + maxHpBonus;

            msg.Success = CharacterManager.Instance.UpdateHp(clientSocket.UserId, msg.CharacterId, maxHp);

            if (msg.Success)
            {
                msg.CurHp = maxHp;
                msg.MaxHp = maxHp;
            }

            ServerSocket.Instance.SendDate(clientSocket, msg);
        }

        public static void MsgCharacterLevelUp(ClientSocket clientSocket, MsgBase msgBase)//角色升级
        {
            Debug.Log("收到角色升级消息");

            MsgCharacterLevelUp msg = msgBase as MsgCharacterLevelUp;

            if (msg == null)
            {
                return;
            }

            msg.Result = CharacterManager.Instance.LevelUp(clientSocket.UserId, msg.CharacterId, msg.Items);

            ServerSocket.Instance.SendDate(clientSocket, msg);
        }

        public static void MsgKillEnemy(ClientSocket clientSocket, MsgBase msgBase)//击杀敌人任务协议
        {
            Debug.Log("收到击杀敌人消息");
            MsgKillEnemy msg = msgBase as MsgKillEnemy;
            if (msg == null)
            {
                return;
            }

            if (clientSocket.UserId <= 0)
            {
                msg.Result = QuestOperationResult.NotLogin;
                ServerSocket.Instance.SendDate(clientSocket, msg);
                return;
            }
            try
            {
                QuestEvent.RaiseProgress(clientSocket.UserId, QuestTarget.KillEnemy, 1);
                msg.Result = QuestOperationResult.Success;
            }
            catch (Exception e)
            {
                Debug.LogError("更新击杀任务失败：" + e);
                msg.Result = QuestOperationResult.ServerError;
            }
            ServerSocket.Instance.SendDate(clientSocket, msg);
        }

        public static void MsgExpandBag(ClientSocket clientSocket, MsgBase msgBase)//背包扩容协议
        {
            Debug.Log("收到背包扩容消息");
            MsgExpandBag msg = msgBase as MsgExpandBag;
            if (msg == null)
            {
                return;
            }
            int capaticy;
            msg.Result = BagManager.Instance.ExpandBag(clientSocket.UserId, msg.AddCount, out capaticy);
            msg.Capacity = capaticy;
            ServerSocket.Instance.SendDate(clientSocket, msg);
        }
    }

}