using System;
using UnityEngine;

public class PlayerSessionModel : Singleton<PlayerSessionModel>
{
    public bool IsSetName { get; set; }
    public string PlayerName { get; set; }

    public void Clear()//清理数据
    {
        IsSetName = false;
        PlayerName = null;
    }
}
