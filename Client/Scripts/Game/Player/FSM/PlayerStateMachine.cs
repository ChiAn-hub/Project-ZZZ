using System.Collections.Generic;
using UnityEngine;

public enum StateEnum//当前玩家状态
{
    Idle,//待机状态
    Walk,//移动状态
    Evade,//闪避状态
    Run,//跑步状态
    Dash,//冲刺状态
    Attack,//攻击状态
    Skill,//释放技能状态
    EXSkill,//大招状态
    NormalHit,//普通受击状态
    HitFly,//击飞状态
    Death,//死亡状态
    EnterStart,//进入角色界面动画
}

public interface IState//状态接口,规定状态方法
{
    public void OnEnter();//进入状态
    public void OnExit();//退出状态
    public void OnUpdate();//更新状态    
}

public class PlayerStateMachine//玩家状态机
{
    private IState curState;//记录当前状态
    private Dictionary<StateEnum, IState> states;//用字典存储每个状态和它所对应的方法
    public PlayerStateMachine()
    {
        states = new Dictionary<StateEnum, IState>();
    }
    public void AddState(StateEnum stateType, IState state)
    {
        if (states.ContainsKey(stateType))//如果字典中存在该状态则退出
        {
            return;
        }
        states.Add(stateType, state);
    }
    public void ChangeState(StateEnum stateType)//切换状态
    {
        if (!states.ContainsKey(stateType))//如果字典中没有该状态
        {
            return;
        }
        if (curState != null)
        {
            curState.OnExit();//执行当前状态的退出操作
        }
        curState = states[stateType];//获取新状态
        curState.OnEnter();//执行新状态的进入操作
    }
    public void Update()//更新状态
    {
        if (curState != null)
        {
            curState.OnUpdate();//执行当前状态的更新操作
        }
    }
}
