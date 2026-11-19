using System.Collections.Generic;
using UnityEngine;

public class AStarGrid : MonoBehaviour
{
    public static AStarGrid Instance { get; private set; }

    private Vector2 gridWorldSize = new Vector2(120, 90);
    private float nodeRadius = 0.5f;//一个格子的半径
    private float groundCheckHeight = 3f;//地面检测起点高度
    private float groundCheckDistance = 6f;//向下检测地面的距离
    private float obstacleCheckHeight = 2f;//障碍物检测高度

    [SerializeField]
    private LayerMask groundLayer;//地面层级
    [SerializeField]
    private LayerMask obstacleLayer;//障碍物层级
    private bool showGrid = true;//是否显示网格
    private AStarNode[,] grid;//存放所有的网格节点

    private float nodeDiameter;//节点的直径
    private int gridCountX;//X轴上格子的数量
    private int gridCountY;//Y轴上格子的数量

    private Vector3 bottomLeft;//网格左下角世界坐标

    private void Awake()
    {
        Instance = this;
        CreateGrid();
    }

    private void CreateGrid()//创建网格
    {
        if (nodeRadius <= 0)
        {
            return;
        }

        nodeDiameter = nodeRadius * 2;//计算每个格子的边长(直径)
        gridCountX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);//计算该世界坐标中X轴上有多少个格子
        gridCountY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);//计算该世界坐标中Z轴上有多少个格子

        grid = new AStarNode[gridCountX, gridCountY];//创建二维数组网格

        //左下角世界坐标等于中心世界坐标减去x轴和Z轴的一半长度乘以x轴的方向向量和Z轴的方向向量
        bottomLeft = transform.position - gridWorldSize.x * 0.5f * Vector3.right - gridWorldSize.y * 0.5f * Vector3.forward;

        for (int x = 0; x < gridCountX; x++)
        {
            for (int y = 0; y < gridCountY; y++)
            {
                CreateNode(x, y);
            }
        }
    }

    private void CreateNode(int x, int y)//创建节点方法
    {
        //计算每个节点的中心世界坐标
        Vector3 nodePos = bottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);

        Vector3 rayStart = nodePos + Vector3.up * groundCheckHeight;//计算射线发射的起始点位置
        RaycastHit groundHit;//保存射线发射碰撞结果
        bool hasHitGround = Physics.Raycast(rayStart, Vector3.down, out groundHit, groundCheckDistance, groundLayer, QueryTriggerInteraction.Ignore);

        Vector3 hitPos = nodePos;//创建坐标用于保存射线发射的碰撞点位置,如果没有碰撞到地面,则坐标更新为节点位置
        if (hasHitGround)//如果有碰撞到地面
        {
            hitPos = groundHit.point;//坐标更新为射线发射的结束点位置
        }

        Vector3 obstaclePos = hitPos + Vector3.up * obstacleCheckHeight * 0.5f;//计算出障碍物检测的位置,该位置位于障碍物检测高度的中心
        Vector3 obstacleSize = new Vector3(nodeRadius * 0.8f, obstacleCheckHeight * 0.5f, nodeRadius * 0.8f);//以障碍物监测点为中心,需要的检测的大小,范围
        //创建检测盒检测是否有障碍物
        bool hasObstacle = Physics.CheckBox(obstaclePos, obstacleSize, Quaternion.identity, obstacleLayer, QueryTriggerInteraction.Ignore);

        bool walkable = hasHitGround && !hasObstacle;//判断是否可走

        grid[x, y] = new AStarNode(walkable, hitPos, x, y);

    }

    public AStarNode GetNodeFromWorldPosition(Vector3 worldPos)//通过世界坐标获取对应的节点位置
    {
        if (grid == null)
        {
            return null;
        }

        float percentX = (worldPos.x - bottomLeft.x) / gridWorldSize.x;//计算该世界坐标在网格中X轴上的百分比位置
        float percentY = (worldPos.z - bottomLeft.z) / gridWorldSize.y;//计算该世界坐标在网格中Z轴上的百分比位置

        percentX = Mathf.Clamp01(percentX);//确保百分比位置在0-1之间
        percentY = Mathf.Clamp01(percentY);//确保百分比位置在0-1之间

        int x = Mathf.RoundToInt((gridCountX - 1) * percentX);//计算该世界坐标在网格中的X轴索引
        int y = Mathf.RoundToInt((gridCountY - 1) * percentY);//计算该世界坐标在网格中的Z轴索引

        return grid[x, y];
    }

    public List<AStarNode> GetNearNodes(AStarNode node)//获取该节点的相邻节点
    {
        List<AStarNode> nearNodes = new List<AStarNode>();


        for (int offSetX = -1; offSetX <= 1; offSetX++)
        {
            for (int offSetY = -1; offSetY <= 1; offSetY++)
            {
                if (offSetX == 0 && offSetY == 0)
                {
                    continue;
                }
                int x = offSetX + node.GridX;//计算该节点的相邻节点的X轴索引
                int y = offSetY + node.GridY;//计算该节点的相邻节点的Y轴索引

                if (x < 0 || x >= gridCountX || y < 0 || y >= gridCountY)//超出地图范围
                {
                    continue;
                }

                AStarNode nearNode = grid[x, y];//获取该节点的相邻对应节点
                if (!nearNode.Walkable)//不可行走
                {
                    continue;
                }
                if (offSetX != 0 && offSetY != 0)
                {
                    AStarNode horizontalNode = grid[x, node.GridY];//获取该节点的横向对应节点
                    AStarNode verticalNode = grid[node.GridX, y];//获取该节点的纵向对应节点

                    if (!horizontalNode.Walkable || !verticalNode.Walkable)//如果斜方向两侧存在墙,则不能走斜线。
                    {
                        continue;
                    }
                }
                nearNodes.Add(grid[x, y]);
            }
        }
        return nearNodes;
    }

    private void OnDrawGizmosSelected()//绘制网格,AI辅助的
    {
        Gizmos.color = Color.white;

        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 0.1f, gridWorldSize.y));

        if (!showGrid || grid == null)
        {
            return;
        }

        foreach (AStarNode node in grid)
        {
            if (node.Walkable)
            {
                Gizmos.color = new Color(0f, 1f, 0f, 0.35f);
            }
            else
            {
                Gizmos.color = new Color(1f, 0f, 0f, 0.35f);
            }

            Vector3 drawPosition =
                node.WorldPos + Vector3.up * 0.05f;

            Vector3 drawSize = new Vector3(
                nodeDiameter * 0.9f,
                0.05f,
                nodeDiameter * 0.9f);

            Gizmos.DrawCube(drawPosition, drawSize);
        }
    }

}
