using System;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyAnimType
{
    Idle,//待机

    WalkForward,//向前移动
    WalkBackward,//向后移动

    Attack1,//攻击1
    Attack2,//攻击2
    Attack3,//攻击3
    Attack4,//攻击4
    Attack5,//攻击5

    Hit,//受击
    Death,//死亡
    WalkForwardStart,//向前移动起步
    WalkBackwardStart,//向后移动起步
}

[Serializable]
public class EnemyAnimInfo
{
    public EnemyAnimType animType;
    public string animName;
}

[Serializable]
public class EnemyAnimList
{
    public List<EnemyAnimInfo> animations;
}