using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Grid
{
    public float x1;
    public float y1;
    public float x2;
    public float y2;
    public float width;
    public float height;
    public List<GameObject> objects;
    public int index_x;
    public int index_y;

    public Grid(float x1, float y1, float x2, float y2, float width, float height, int index_x, int index_y)
    {
        this.x1 = x1;
        this.y1 = y1;
        this.x2 = x2;
        this.y2 = y2;
        this.width = width;
        this.height = height;
        this.index_x = index_x;
        this.index_y = index_y;
        objects = new List<GameObject>();
    }

    /// <summary>
    /// 判断一个游戏物体是否在网格中
    /// </summary>
    /// <param name="gameObject">要检查的游戏物体</param>
    /// <returns>如果游戏物体在网格中，返回true；否则返回false</returns>
    public bool IsPositionInGrid(Vector3 _position)
    {
        Vector3 position = _position;
        return position.x >= x1 && position.x <= x2 && position.z >= y1 && position.z <= y2;
    }

    /// <summary>
    /// 计算网格的面积（宽度乘以高度）
    /// </summary>
    /// <returns>网格的面积</returns>
    public float CalculateArea()
    {
        return width * height;
    }

    /// <summary>
    /// 添加游戏物体到网格中
    /// </summary>
    /// <param name="obj">要添加的游戏物体</param>
    public void AddObject(GameObject obj)
    {
        if (!objects.Contains(obj))
        {
            objects.Add(obj);
        }
    }

    /// <summary>
    /// 从网格的物体列表中中移除游戏物体
    /// </summary>
    /// <param name="obj">要移除的游戏物体</param>
    public void RemoveObject(GameObject obj)
    {
        if (objects.Contains(obj))
        {
            objects.Remove(obj);
        }
    }

    /// <summary>
    /// 将游戏物体从当前网格的物体列表中删除，并移动到目标网格的移动列表。
    /// </summary>
    /// <param name="obj">要移动的游戏物体</param>
    /// <param name="targetGrid">目标网格</param>
    public void MoveObjectToGrid(GameObject obj, Grid targetGrid)
    {
        RemoveObject(obj);
        targetGrid.AddObject(obj);
    }
}


public class GridSystem : MonoBehaviour
{
    public ControlSystem_Players controlSystem_Players;
    public Grid[,] grids;

    public GameObject terrainParent;
    public float gridWidth;
    public float gridHeight;

    // Start is called before the first frame update
    void Start()
    {
        CalculateGrids();

        if (controlSystem_Players == null)
        {
            controlSystem_Players = FindAnyObjectByType<ControlSystem_Players>();
            Debug.Log("controlSystem_Players is null, find it in scene. "  + controlSystem_Players.name);
        }

        foreach (PlayerController player in controlSystem_Players.players)
        {

            List<UnitControl> playerUnit = player.getPlayerUnits();
            if (playerUnit == null)
            {
                Debug.Log(player.name + " do not have exist unit.");
                continue;
            }
            foreach (UnitControl unit in playerUnit)
            {
                AddObjectToGrid(unit.gameObject);
            }
        }


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpgradeGrids();

    }


    //————————————————————————————
    // Note：未取用的逻辑：
    //     （未取用）功能实现的原理：
    //     1.每当一个单位行动的时候，都会将自己所在网格周围所有的网格发送到 - Fixed待更新网格列表中。
    //     2.帧读取所有的待更新网格
    //     3.Fixed列表里更新网格数据
    //____________________________
    /// <summary>
    /// 在Fixed帧中更新网格系统的每一个单位。
    ///     UpgradeGrids方法需要遍历所有网格，对每个网格检查其周围相邻网格中的游戏物体。
    ///     如果发现相邻网格中有距离足够近的游戏物体，则将它们添加到当前网格的游戏物体列表中。
    ///     如果当前网格中的游戏物体移动到了其他网格，则需要将其从当前网格中移除，并添加到目标网格的游戏物体列表中。
    /// </summary>
    public void UpgradeGrids()
    {
        int rowCount = grids.GetLength(0);
        int colCount = grids.GetLength(1);

        // 遍历所有网格
        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                Grid currentGrid = grids[row, col];

                List<GameObject> closeObjects = new List<GameObject>();
                if (currentGrid.objects.Count > 0)
                {
                    closeObjects = GetCloseNeighboringObjects(row, col, currentGrid.objects[0], 1, 5f);
                }

                // 将距离足够近的游戏物体添加到当前网格的游戏物体列表中
                foreach (GameObject closeObject in closeObjects)
                {
                    if (!currentGrid.objects.Contains(closeObject))
                    {
                        currentGrid.objects.Add(closeObject);
                    }
                }

                // 如果当前网格中的游戏物体移动到了其他网格，则需要将其从当前网格中移除，并添加到目标网格的游戏物体列表中
                for (int i = currentGrid.objects.Count - 1; i >= 0; i--)
                {
                    GameObject obj = currentGrid.objects[i];
                    Grid targetGrid = FindGridByPosition(obj.transform.position);
                    if (targetGrid != null && targetGrid != currentGrid)
                    {
                        currentGrid.RemoveObject(obj);
                        targetGrid.AddObject(obj);
                    }
                }
            }
        }
    }



    /// <summary>
    /// 计算网格系统，根据指定范围生成网格
    /// </summary>
    void CalculateGrids()
    {
        Renderer[] childRenderers = terrainParent.GetComponentsInChildren<Renderer>();

        // 获取子物体的边界
        Bounds combinedBounds = new Bounds();
        foreach (Renderer renderer in childRenderers)
        {
            combinedBounds.Encapsulate(renderer.bounds);
        }

        // 计算地形父物体的边界中心点
        Vector3 terrainCenter = combinedBounds.center;

        // 计算地形父物体的边界宽度和高度
        float terrainWidth = combinedBounds.size.x;
        float terrainHeight = combinedBounds.size.z;

        // 计算每个网格系统的行数和列数
        int rowCount = Mathf.CeilToInt(terrainHeight / gridHeight);
        int colCount = Mathf.CeilToInt(terrainWidth / gridWidth);

        // 调整网格的宽度和高度
        gridWidth = terrainWidth / colCount;
        gridHeight = terrainHeight / rowCount;

        // 初始化网格数组
        grids = new Grid[rowCount, colCount];

        // 计算每个网格的边界和中心点，并创建网格对象
        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                float x1 = terrainCenter.x - terrainWidth / 2 + col * gridWidth;
                float y1 = terrainCenter.z - terrainHeight / 2 + row * gridHeight;
                float x2 = x1 + gridWidth;
                float y2 = y1 + gridHeight;

                grids[row, col] = new Grid(x1, y1, x2, y2, gridWidth, gridHeight, row, col);

            }
        }
    }

    /// <summary>
    /// 将游戏物体添加到网格系统中的相应网格
    /// </summary>
    /// <param name="obj">要添加的游戏物体</param>
    public void AddObjectToGrid(GameObject obj)
    {
        UnitControl unitControl = obj.GetComponent<UnitControl>();

        if (unitControl != null && unitControl.GetGrid() != null) {
            unitControl.CleanGrid();
        }

        foreach (Grid grid in grids)
        {
            if (grid.IsPositionInGrid(obj.transform.position))
            {
                // 检查物体是否已经存在于某个网格中
                if (!grid.objects.Contains(obj))
                {
                    grid.AddObject(obj);
                    unitControl.SetGrid(grid);
                }
                break;
            }
        }
    }

    /// <summary>
    /// 获取指定网格周围的相邻网格
    /// </summary>
    /// <param name="grid">指定的网格</param>
    /// <param name="range">搜索范围</param>
    /// <returns>相邻网格的列表</returns>
    public List<Grid> GetNeighboringGrids(Grid grid, int range)
    {
        List<Grid> neighboringGrids = new List<Grid>();

        int rowIndex = grid.index_x;
        int colIndex = grid.index_y;

        // 计算周围网格的行范围和列范围，确保不超出索引范围
        int startRow = Mathf.Max(0, rowIndex - range);
        int endRow = Mathf.Min(grids.GetLength(0) - 1, rowIndex + range);
        int startCol = Mathf.Max(0, colIndex - range);
        int endCol = Mathf.Min(grids.GetLength(1) - 1, colIndex + range);

        // 遍历周围网格并收集相邻网格
        for (int row = startRow; row <= endRow; row++)
        {
            for (int col = startCol; col <= endCol; col++)
            {
                neighboringGrids.Add(grids[row, col]);
            }
        }

        return neighboringGrids;
    }



    /// <summary>
    /// 获取指定网格周围的相邻游戏物体
    /// </summary>
    /// <param name="grid">指定的网格</param>
    /// <param name="range">搜索范围</param>
    /// <returns>相邻游戏物体的列表</returns>
    public List<GameObject> GetNeighboringObjects(Grid grid, int range)
    {
        List<GameObject> neighboringObjects = new List<GameObject>();

        List<Grid> neighboringGrids = GetNeighboringGrids(grid, range);

        // 遍历相邻网格并收集物体
        foreach (Grid neighboringGrid in neighboringGrids)
        {
            neighboringObjects.AddRange(neighboringGrid.objects);
        }

        return neighboringObjects;
    }



    /// <summary>
    /// 寻找在相邻网格之中，距离在指定范围之内的其他物体。
    /// </summary>
    /// <param name="rowIndex">行坐标</param>
    /// <param name="colIndex">列坐标</param>
    /// <param name="competObject">用于位置比较的游戏物体</param>
    /// <param name="gridRange">网格的搜索范围</param>
    /// <param name="objectDistance">多远的距离才会被判定为“接近”?</param>
    /// <returns>所有足够接近目标的物体</returns>
    public List<GameObject> GetCloseNeighboringObjects(int rowIndex, int colIndex, GameObject competObject, int gridRange, float objectDistance)
    {
        Grid grid = grids[rowIndex, colIndex];
        List<GameObject> neighboringObjects = GetNeighboringObjects(grid, gridRange);

        List<GameObject> closeObjects = new List<GameObject>();

        foreach (GameObject neighboringObject in neighboringObjects)
        {
            float distance = Vector3.Distance(neighboringObject.transform.position, competObject.transform.position);

            if (distance < objectDistance)
            {
                closeObjects.Add(neighboringObject);
            }
        }

        return closeObjects;
    }

    // 在Scene视图中绘制网格系统的边界
    private void OnDrawGizmos()
    {
        if (grids != null)
        {
            foreach (Grid grid in grids)
            {
                if (grid.objects.Count > 0)
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.blue;
                }

                Gizmos.DrawLine(new Vector3(grid.x1, 0, grid.y1), new Vector3(grid.x2, 0, grid.y1));
                Gizmos.DrawLine(new Vector3(grid.x2, 0, grid.y1), new Vector3(grid.x2, 0, grid.y2));
                Gizmos.DrawLine(new Vector3(grid.x2, 0, grid.y2), new Vector3(grid.x1, 0, grid.y2));
                Gizmos.DrawLine(new Vector3(grid.x1, 0, grid.y2), new Vector3(grid.x1, 0, grid.y1));
            }
        }
    }

    public Grid FindGridByIndex(int rowIndex, int colIndex)
    {
        int rowCount = grids.GetLength(0);
        int colCount = grids.GetLength(1);

        if (rowIndex >= 0 && rowIndex < rowCount && colIndex >= 0 && colIndex < colCount)
        {
            return grids[rowIndex, colIndex];
        }

        return null;
    }

    public Grid FindGridByPosition(Vector3 position)
    {
        int rowCount = grids.GetLength(0);
        int colCount = grids.GetLength(1);

        for (int row = 0; row < rowCount; row++)
        {
            for (int col = 0; col < colCount; col++)
            {
                Grid grid = grids[row, col];

                if (grid.IsPositionInGrid(position))
                {
                    return grid;
                }
            }
        }

        return null;
    }   
}
