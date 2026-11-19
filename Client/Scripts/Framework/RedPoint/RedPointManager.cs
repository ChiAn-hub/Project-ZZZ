using System;

public class RedPointManager : Singleton<RedPointManager>
{
    private RedPointNode root = new RedPointNode("Root", null);

    private RedPointNode GetNode(string path, bool create)//获取对应的节点,并说明是否进行创建
    {
        if (string.IsNullOrEmpty(path))//无效路径名
        {
            return null;
        }
        string[] nodeNames = path.Split('/');//将字符串以/为标志进行切割,并保存到数组中
        RedPointNode curNode = root;
        foreach (string nodeName in nodeNames)//遍历节点名,查找目标节点
        {
            if (string.IsNullOrEmpty(nodeName))//无效节点名
            {
                continue;
            }
            RedPointNode nextNode = curNode.GetChild(nodeName);//查找当前节点的下一个节点
            if (nextNode == null)
            {
                if (!create)
                {
                    return null;
                }
                nextNode = curNode.GetOrAddChild(nodeName);
            }
            curNode = nextNode;
        }
        if (curNode == root)
        {
            return null;
        }
        return curNode;
    }

    private void NotifyParent(RedPointNode node)
    {
        RedPointNode curNode = node;
        while (curNode != null)
        {
            curNode.Notify();//调用每个节点所绑定的显示函数
            curNode = curNode.Parent;//继续通知父节点
        }
    }

    public bool GetRedPointState(string path)//获取对应节点状态
    {
        RedPointNode node = GetNode(path, false);//获取对应节点
        if (node == null)
        {
            return false;
        }
        return node.IsShow;
    }

    public void AddListener(string path, Action<bool> callback)//对应的节点添加监听
    {
        RedPointNode node = GetNode(path, true);//获取对应节点
        if (node == null)
        {
            return;
        }
        node.AddListener(callback);
    }

    public void RemoveListener(string path, Action<bool> callback)//对应的节点移除监听
    {
        RedPointNode node = GetNode(path, false);//获取对应节点
        if (node == null)
        {
            return;
        }
        node.RemoveListener(callback);
    }

    public void SetRedPoint(string path, bool show)//设置节点是否显示和创建
    {
        RedPointNode node = GetNode(path, true);//获取对应节点
        if (node == null)
        {
            return;
        }
        if (!node.SetSelfShow(show))//设置节点显示
        {
            return;//设置失败
        }
        NotifyParent(node);
    }

    public void Clear()
    {
        root = new RedPointNode("Root", null);
    }
}
