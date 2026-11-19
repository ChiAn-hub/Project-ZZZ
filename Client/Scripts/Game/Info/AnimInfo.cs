using System;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerAnimationType
{
    Idle = 0,
    IdleAFK = 1,
    EnterStart = 2,

    WalkStart = 3,
    WalkStartEnd = 4,
    Walk = 5,
    WalkEnd = 6,

    Run = 7,
    RunEnd = 8,

    EvadeFront = 9,
    EvadeBack = 10,
    TurnBack = 11,

    LevelUp = 12,
    NormalHit = 13,
    HitFly = 14,
    Death = 15,

    NormalAttack1 = 16,
    NormalAttack1End = 17,

    MoveNormalAttack1 = 18,
    MoveNormalAttack1End = 19,

    NormalAttack2 = 20,
    NormalAttack2End = 21,
    MoveNormalAttack2 = 22,

    NormalAttack3 = 23,
    NormalAttack3Back = 24,
    NormalAttack3Near = 25,
    NormalAttack3BackNear = 26,
    NormalAttack3End = 27,

    MoveNormalAttack3 = 28,
    MoveNormalAttack3Back = 29,
    MoveNormalAttack3Near = 30,
    MoveNormalAttack3BackNear = 31,
    MoveNormalAttack3End = 32,
    Skill1 = 33,
    Skill1End = 34,

    Skill2 = 35,
    Skill2End = 36,

    Skill3 = 37,
    Skill3End = 38,

    EXSkill = 39,
    EXSkillEnd = 40,
    DashStart = 41,
    Dash = 42,
    DashEnd = 43,
    DashEvade = 44,
    DashAttack1 = 45,
    DashAttack2 = 46,
    DashAttackEnd = 47,
}

[Serializable]
public class AnimInfo
{
    public PlayerAnimationType animationType;
    public string stateName;
}

[Serializable]
public class AnimList
{
    public List<AnimInfo> animations;
}