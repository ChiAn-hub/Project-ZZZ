using UnityEngine;

public class BehaviorTree
{
    private BTNode rootNode;
    public BehaviorTree(BTNode rootNode)
    {
        this.rootNode = rootNode;
    }
    public BTNodeState Update()
    {
        if (rootNode == null)
        {
            return BTNodeState.Failure;
        }

        return rootNode.Tick();//从根节点开始执行行为树
    }
}
