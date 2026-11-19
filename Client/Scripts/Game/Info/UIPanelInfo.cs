using System;
using System.Collections.Generic;
using UnityEngine;
public enum UIPanelType
{
    LoadingPanel,
    LoadMainPanel,
    LoadPanel,
    RegisterPanel,
    SetNamePanel,
    QuestPanel,
    OpenQuestPanel,
    MainPanel,
    CityPanel,
    BagPanel,
    ShopPanel,
    OpenShopPanel,
    PlayerPanel,
    SystemPanel
}
[Serializable]
public class UIPanelInfo : ISerializationCallbackReceiver
{
    public UIPanelType panelType;
    public string panelTypeName;
    public string path;

    public void OnAfterDeserialize()
    {
        panelType = (UIPanelType)Enum.Parse(typeof(UIPanelType), panelTypeName);
    }

    public void OnBeforeSerialize()
    {

    }
}
[Serializable]
public class UIPanelList
{
    public List<UIPanelInfo> infoList;
}