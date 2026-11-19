using System;
using System.Collections.Generic;

public class RedPointNode
{
    private string nodeName;//节点名字
    private bool selfShow;//节点自身是否显示
    private RedPointNode parent;//父节点
    private Dictionary<string, RedPointNode> children = new Dictionary<string, RedPointNode>();//全部子节点
    public event Action<bool> StateChanged;//节点状态改变
    public RedPointNode Parent
    {
        get
        {
            return parent;
        }
    }
    public bool IsShow//节点是否显示
    {
        get
        {
            if (selfShow)//节点自身显示
            {
                return true;
            }
            foreach (RedPointNode child in children.Values)//子节点如果有显示的,则自己也显示
            {
                if (child.IsShow)
                {
                    return true;
                }
            }
            return false;
        }
    }
    public RedPointNode(string name, RedPointNode parent)
    {
        this.nodeName = name;
        this.parent = parent;
    }
    public RedPointNode GetChild(string name)//通过路径名获取子节点
    {
        RedPointNode node;
        if (children.TryGetValue(name, out node))
        {
            return node;
        }
        return null;
    }
    public RedPointNode GetOrAddChild(string name)//获取或者创建子节点
    {
        RedPointNode child;
        if (children.TryGetValue(name, out child))//获取子节点
        {
            return child;
        }
        child = new RedPointNode(name, this);//没有获取到则进行创建
        children.Add(name, child);
        return child;
    }
    public bool SetSelfShow(bool show)//设置自身是否显示
    {
        if (selfShow == show)
        {
            return false;
        }
        selfShow = show;
        return true;
    }
    public void AddListener(Action<bool> callback)//添加监听
    {
        if (callback == null)
        {
            return;
        }
        StateChanged -= callback;
        StateChanged += callback;
        callback.Invoke(IsShow);
    }
    public void RemoveListener(Action<bool> callback)//移除监听
    {
        StateChanged -= callback;
    }
    public void Notify()
    {
        if (StateChanged != null)
        {
            StateChanged.Invoke(IsShow);
        }
    }
}
