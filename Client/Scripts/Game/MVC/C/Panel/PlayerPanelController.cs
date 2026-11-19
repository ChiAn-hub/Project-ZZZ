using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPanelController : Singleton<PlayerPanelController>
{
    private const int EllenId = 1;
    public void RequestEllenInfo(Action<bool> callback = null)
    {
        ProtoManager.Instance.CSCharacterInfo(EllenId, (success, data) =>
        {
            if (!success || data == null)
            {
                Debug.LogError("获取Ellen信息失败");
                callback?.Invoke(false);
                return;
            }

            EllenDataModel.Instance.ApplyData(data);
            callback?.Invoke(true);
        });
    }
    public void EquipEllen(int itemId, Action<bool> callback = null)
    {
        ChangeEquipment(itemId, EquipAction.Equip, callback);
    }
    public void UnequipEllen(Action<bool> callback = null)
    {
        ChangeEquipment(0, EquipAction.Unequip, callback);
    }
    private void ChangeEquipment(int itemId, EquipAction action, Action<bool> callback)
    {
        ProtoManager.Instance.CSEquipItem(EllenId, itemId, action, (msg) =>
        {

            if (msg == null || msg.Result != EquipResult.Success)
            {
                Debug.LogError("装备操作失败：" + (msg == null ? "没有收到服务器消息" : msg.Result.ToString()));
                callback?.Invoke(false);
                return;
            }
            RefreshAfterEquipment(callback);
        });
    }
    private void RefreshAfterEquipment(Action<bool> callback)
    {
        RequestEllenInfo((characterSuccess) =>
        {
            ProtoManager.Instance.CSBag((bagSuccess, items, capacity) =>
            {
                if (bagSuccess)
                {
                    BagModel.Instance.ReplaceItems(items, capacity);
                }

                callback?.Invoke(characterSuccess && bagSuccess);
            });
        }
    );
    }

    public void UpdateEllenHp(Action<bool> callback = null)//同步Ellen血量
    {
        EllenDataModel model = EllenDataModel.Instance;

        ProtoManager.Instance.CSCharacterHp(model.CharacterId, model.CurHp, (msg) =>
            {
                if (msg == null || !msg.Success)
                {
                    Debug.LogError("同步Ellen血量失败");
                    callback?.Invoke(false);
                    return;
                }
                callback?.Invoke(true);
            });
    }

    public void LevelUpEllen(List<ExpItemUseData> items, Action<bool> callback = null)
    {
        ProtoManager.Instance.CSCharacterLevelUp(EllenId, items, (msg) =>
        {
            if (msg == null || msg.Result != LevelUpResult.Success)
            {
                Debug.LogError("角色升级失败");

                callback?.Invoke(false);
                return;
            }

            RefreshAfterLevelUp(callback);
        });
    }

    private void RefreshAfterLevelUp(Action<bool> callback)//升级成功后刷新
    {
        RequestEllenInfo((characterSuccess) =>//获取Ellen信息
        {
            ProtoManager.Instance.CSBag((bagSuccess, items, capacity) =>//获取背包
                {
                    if (bagSuccess)
                    {
                        BagModel.Instance.ReplaceItems(items, capacity);//刷新背包
                    }

                    callback?.Invoke(characterSuccess && bagSuccess);//回调
                });
        });
    }
}
