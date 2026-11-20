using System;
using System.Collections.Generic;

public enum BTNodeState//行为树节点的执行状态
{
    Success,//执行成功
    Failure,//执行失败
    Running,//正在执行
}

public abstract class BTNode//抽象类行为树节点规定每个节点必须的方法实现,用于多态调用
{
    public abstract BTNodeState Tick();//每个节点的执行结果
}

public class SelectBTNode : BTNode//选择节点,根节点,用于判断是否有分支执行成功
{
    private List<BTNode> childList;//子分支列表,用于存储每个分支
    public SelectBTNode(List<BTNode> childList)//构造函数
    {
        if (childList == null)
        {
            this.childList = new List<BTNode>();
            return;
        }
        this.childList = childList;
    }
    public override BTNodeState Tick()
    {
        for (int i = 0; i < childList.Count; i++)
        {
            BTNodeState state = childList[i].Tick();//获取每个分支执行结果
            if (state == BTNodeState.Success)
            {
                return BTNodeState.Success;
            }
            if (state == BTNodeState.Running)
            {
                return BTNodeState.Running;
            }
        }
        return BTNodeState.Failure;
    }
}

public class SequenceBTNode : BTNode//顺序节点,每个分支的头部,用于判断该分支是否全部执行成功
{
    private List<BTNode> childList;//子节点列表,用于存储该分支的所有节点
    public SequenceBTNode(List<BTNode> childList)//构造函数,传入子节点列表
    {
        if (childList == null)
        {
            this.childList = new List<BTNode>();
            return;
        }
        this.childList = childList;
    }
    public override BTNodeState Tick()
    {
        for (int i = 0; i < childList.Count; i++)
        {
            BTNodeState state = childList[i].Tick();//获取每个分支执行结果
            if (state == BTNodeState.Failure)
            {
                return BTNodeState.Failure;
            }

            if (state == BTNodeState.Running)
            {
                return BTNodeState.Running;
            }
        }
        return BTNodeState.Success;
    }
}

public class ConditionNode : BTNode//条件节点,用于判断条件是否成立
{
    private Func<bool> condition;//委托,返回值为布尔,用于判断行为节点的条件是否成立,使用委托可以存储每个行为不同条件判断方法
    public ConditionNode(Func<bool> condition)
    {
        this.condition = condition;//创建节点时存储对应的条件判断逻辑
    }
    public override BTNodeState Tick()
    {
        if (condition == null)
        {
            return BTNodeState.Failure;
        }

        if (condition())
        {
            return BTNodeState.Success;
        }

        return BTNodeState.Failure;
    }
}

public class ActionNode : BTNode//行为节点,用于执行具体的操作,通知FSM切换状态
{
    private Func<BTNodeState> action;//委托,返回值为该行为节点的状态,用于判断该行为的执行结果,使用委托可以存储每个行为的具体逻辑
    public ActionNode(Func<BTNodeState> action)
    {
        this.action = action;//创建行为节点时,传入具体的行为逻辑
    }
    public override BTNodeState Tick()
    {
        if (action == null)
        {
            return BTNodeState.Failure;
        }

        return action();
    }
}