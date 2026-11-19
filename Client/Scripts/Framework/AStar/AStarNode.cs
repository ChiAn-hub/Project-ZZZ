using UnityEngine;

public class AStarNode
{
    public bool Walkable { get; set; }//是否可以行走
    public Vector3 WorldPos { get; private set; }//节点的世界坐标
    public int GridX { get; private set; }//节点在数组中的X
    public int GridY { get; private set; }//节点在数组中的Y

    public int GCost { get; set; }//起点到当前节点的消耗
    public int HCost { get; set; }//当前节点到终点的预计消耗
    public AStarNode Parent { get; set; }//寻路时的父节点

    public int FCost
    {
        get
        {
            return GCost + HCost;//起点到终点的总消耗
        }
    }

    public AStarNode(bool walkable, Vector3 worldPos, int gridX, int gridY)
    {
        Walkable = walkable;
        WorldPos = worldPos;
        GridX = gridX;
        GridY = gridY;
    }

}
