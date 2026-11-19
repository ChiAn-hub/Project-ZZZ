using System;
using UnityEngine;

public class PlayerInfoController : Singleton<PlayerInfoController>
{
    public void RequestPlayerInfo(Action<bool> callback = null)
    {
        ProtoManager.Instance.CSPlayerInfo((success, playerInfo) =>
        {
            if (!success || playerInfo == null)
            {
                Debug.LogError("获取玩家信息失败");
                callback?.Invoke(false);
                return;
            }

            PlayerModel.Instance.ApplyData(playerInfo);
            callback?.Invoke(true);
        });
    }
}
