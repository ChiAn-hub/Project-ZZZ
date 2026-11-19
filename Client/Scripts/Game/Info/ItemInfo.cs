using System.Collections.Generic;
using UnityEngine;

public enum itemType
{
    Weapon = 1,
    Other = 3,
    CharactorExp = 2,
    WeaponExp = 4,
    BangBoo = 5
}

public class ItemInfo
{
    public char quality;
    public int itemId;
    public itemType itemType;
    public string name;
    public string path;
    public string content;
}
