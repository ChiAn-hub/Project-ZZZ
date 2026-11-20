using System.Collections.Generic;
using UnityEngine;

public enum EnemyStateType//怪物状态类型
{
    Idle,//待机
    Patrol,//巡逻
    Chase,//追击玩家
    Attack,//攻击
    Cooldown,//攻击冷却
    Hit,//受击
    Death,//死亡
}

public enum EnemyStateResult//执行状态结果
{
    Running,//状态正在执行
    Success,//状态正常完成
    Failure,//状态执行失败
}

public interface IEnemyState//怪物状态更新接口
{
    void OnEnter();
    void OnExit();
    void OnUpdate();
}

public class EnemyStateMachine
{
    private IEnemyState curState;//怪物当前状态
    private EnemyStateType curStateType;//怪物当前状态类型
    private EnemyStateResult curStateResult;//怪物当前状态执行结果
    private Dictionary<EnemyStateType, IEnemyState> states;
    public EnemyStateType CurStateType//用于行为树获取当前状态类型
    {
        get { return curStateType; }
    }
    public EnemyStateResult CurStateResult//用于行为树获取当前状态执行结果
    {
        get { return curStateResult; }
    }
    public EnemyStateMachine()
    {
        states = new Dictionary<EnemyStateType, IEnemyState>();
    }
    public void AddState(EnemyStateType stateType, IEnemyState state)
    {
        if (states.ContainsKey(stateType))//如果字典中存在该状态则退出
        {
            return;
        }
        states.Add(stateType, state);
    }
    public void ChangeState(EnemyStateType stateType, bool Restart = false)
    {
        if (!states.ContainsKey(stateType))//如果字典中没有该状态,则退出
        {
            return;
        }
        if (!Restart && curState != null && curStateType == stateType)//如果当前状态和要切换的状态相同则退出
        {
            return;
        }
        if (curState != null)//如果当前状态不为空
        {
            curState.OnExit();//执行当前状态的退出操作
        }
        curState = states[stateType];//获取对应状态
        curStateType = stateType;
        curStateResult = EnemyStateResult.Running;
        curState.OnEnter();
    }
    public void SetStateResult(EnemyStateResult stateResult)
    {
        curStateResult = stateResult;
    }
    public void Update()
    {
        if (curState != null)
        {
            curState.OnUpdate();
        }
    }
}
