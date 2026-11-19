using System;
using System.Collections.Generic;

public enum PlayerEffectType
{
    None = 0,

    SwordSlash1 = 1,//普通攻击第一段
    SwordSlash2 = 2,//普通攻击第二段
    SwordSlash6 = 3,//普通攻击第三段
    SwordSlashCombo7 = 4,//普通攻击第三段Near

    Prick3 = 5,//技能第一段和第二段
    SwordSlash16 = 6,//技能第三段

    SpikesAttack = 7,//大招
    SwordSlashCombo1 = 8,//大招第二段
}

[Serializable]
public class PlayerEffectInfo
{
    public PlayerEffectType effectType;//特效类型
    public string prefabPath;//Resources中的Prefab路径

}

[Serializable]
public class PlayerEffectList
{
    public List<PlayerEffectInfo> effects;
}