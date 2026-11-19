using System.Collections.Generic;
using UnityEngine;



public class AStarPathFinding : Singleton<AStarPathFinding>
{
    private const int HorizonCost = 10;//水平移动的消耗
    private const int DiagonalCost = 14;//斜向移动的消耗
    private const int VerticalCost = 10;//竖直移动的消耗

    private int GetDistance(AStarNode firstNode, AStarNode secondNode)//获取两个节点之间的距离消耗
    {
        int distanceX = Mathf.Abs(firstNode.GridX - secondNode.GridX);//获取两个节点之间的X轴距离
        int distanceY = Mathf.Abs(firstNode.GridY - secondNode.GridY);//获得两个节点之间的Z轴距离

        if (distanceX > distanceY)//如果X轴距离大于Z轴距离
        {
            return DiagonalCost * distanceY + HorizonCost * (distanceX - distanceY);//返回斜向移动的消耗加上剩余X轴的消耗
        }
        else
        {
            return DiagonalCost * distanceX + VerticalCost * (distanceY - distanceX);//返回斜向移动的消耗加上剩余Z轴的消耗
        }
    }

    private AStarNode GetLowestFCostNode(List<AStarNode> openList)//获取消耗最低的节点
    {
        AStarNode lowestNode = openList[0];
        int lastIndex = openList.Count - 1;

        openList[0] = openList[lastIndex];
        openList.RemoveAt(lastIndex);

        if (openList.Count > 0)
        {
            HeapSwap(openList, openList.Count, 0);
        }

        return lowestNode;
    }

    private List<Vector3> CreatPath(AStarNode startNode, AStarNode targetNode)//根据节点创建路径
    {
        List<Vector3> path = new List<Vector3>();
        AStarNode curNode = targetNode;
        while (curNode != startNode)//循环直到起始节点
        {
            path.Add(curNode.WorldPos);//将当前节点的坐标添加到路径中
            curNode = curNode.Parent;//将当前节点的父节点设置为当前节点
        }
        path.Reverse();//反转路径,将起点放置在开头
        return path;
    }

    public List<Vector3> FindPath(Vector3 startWorldPos, Vector3 endWorldPos)//寻找路径方法
    {
        List<Vector3> emptyList = new List<Vector3>();

        AStarNode startNode = AStarGrid.Instance.GetNodeFromWorldPosition(startWorldPos);
        AStarNode targetNode = AStarGrid.Instance.GetNodeFromWorldPosition(endWorldPos);

        if (startNode == null || targetNode == null)//如果起点位置或终点位置不存在
        {
            return emptyList;
        }

        if (!startNode.Walkable || !targetNode.Walkable)//如果起点位置或终点位置都不可走
        {
            Debug.LogError("起点位置或终点位置都不可走");
            return emptyList;
        }

        List<AStarNode> openList = new List<AStarNode>();//开放列表
        HashSet<AStarNode> closeSet = new HashSet<AStarNode>();//关闭列表

        startNode.GCost = 0;//起点到当前节点的消耗
        startNode.HCost = GetDistance(startNode, targetNode);//当前节点到终点的预计消耗
        startNode.Parent = null;

        openList.Add(startNode);//添加到开放列表中
        HeapUp(openList, openList.Count - 1);

        while (openList.Count > 0)
        {
            AStarNode curNode = GetLowestFCostNode(openList);//获取开放列表中消耗最低的节点
            closeSet.Add(curNode);//添加到关闭列表中

            if (curNode == targetNode)
            {
                return CreatPath(startNode, targetNode);//创建路径并返回
            }

            List<AStarNode> nearNodes = AStarGrid.Instance.GetNearNodes(curNode);//获取当前节点的周围节点
            foreach (AStarNode nearNode in nearNodes)//遍历该节点的周围节点
            {
                if (closeSet.Contains(nearNode))//该节点已经在关闭列表中,跳过
                {
                    continue;
                }
                int newGCost = curNode.GCost + GetDistance(curNode, nearNode);//计算起点到当前节点的消耗和当前节点到下个节点的消耗
                int nearNodeIndex = openList.IndexOf(nearNode);
                if (newGCost < nearNode.GCost || nearNodeIndex == -1)//如果从当前节点到下个节点的消耗小于直接到下个节点的消耗,或者下个节点不在开放列表中
                {
                    nearNode.GCost = newGCost;//更新下个节点的实际消耗
                    nearNode.HCost = GetDistance(nearNode, targetNode);//更新下个节点的预计消耗
                    nearNode.Parent = curNode;//设置下个节点的父节点为当前节点
                    if (nearNodeIndex == -1)//如果下个节点不在开放列表中
                    {
                        openList.Add(nearNode);//添加到开放列表中
                        HeapUp(openList, openList.Count - 1);//新节点从列表末尾向上堆化
                    }
                    else//如果下个节点在开放列表中,并且消耗更小
                    {
                        HeapUp(openList, nearNodeIndex);
                    }
                }
            }
        }
        return emptyList;
    }

    private bool IsLowerCost(AStarNode firstNode, AStarNode secondNode)
    {
        if (firstNode.FCost < secondNode.FCost)
        {
            return true;
        }
        if (firstNode.FCost == secondNode.FCost && firstNode.HCost < secondNode.HCost)
        {
            return true;
        }
        return false;
    }

    private void HeapSwap(List<AStarNode> list, int listCount, int index)//堆排序交换
    {
        int minIndex = index;//最小值的节点索引
        int left = 2 * index + 1;//左子节点
        int right = 2 * index + 2;//右子节点
        if (left < listCount && IsLowerCost(list[left], list[minIndex]))
        {
            minIndex = left;
        }
        if (right < listCount && IsLowerCost(list[right], list[minIndex]))
        {
            minIndex = right;
        }
        if (minIndex != index)//最小值索引不是当前索引
        {
            AStarNode temp = list[index];//获取当前节点的值
            list[index] = list[minIndex];//将最小值节点的值设置为父节点
            list[minIndex] = temp;//将父节点的值设置为子节点
            HeapSwap(list, listCount, minIndex);//将该节点下方的节点继续堆化
        }
    }

    private void HeapUp(List<AStarNode> openList, int index)//将新节点向上堆化
    {
        while (index > 0)
        {
            int parentIndex = (index - 1) / 2;
            if (IsLowerCost(openList[index], openList[parentIndex]))
            {
                AStarNode temp = openList[parentIndex];
                openList[parentIndex] = openList[index];
                openList[index] = temp;
                index = parentIndex;
            }
            else
            {
                break;
            }
        }
    }
}
