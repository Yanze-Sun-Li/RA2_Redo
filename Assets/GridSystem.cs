using System.Collections;
using System.Collections.Generic;
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

    public Grid(float x1, float y1, float x2, float y2, float width, float height)
    {
        this.x1 = x1;
        this.y1 = y1;
        this.x2 = x2;
        this.y2 = y2;
        this.width = width;
        this.height = height;
        objects = new List<GameObject>();
    }
    public bool IsPositionInGrid(GameObject gameObject)
    {
        Vector3 position = gameObject.transform.position;
        return position.x >= x1 && position.x <= x2 && position.z >= y1 && position.z <= y2;
    }
    public float calculateArea()
    {
        return width * height;
    }
    public void AddObject(GameObject obj)
    {
        if (!objects.Contains(obj))
        {
            objects.Add(obj);
        }
    }
    public void RemoveObject(GameObject obj)
    {
        if (objects.Contains(obj))
        {
            objects.Remove(obj);
        }
    }

    public void MoveObjectToGrid(GameObject obj, Grid targetGrid)
    {
        RemoveObject(obj);
        targetGrid.AddObject(obj);
    }

}


public class GridSystem : MonoBehaviour
{
    public Grid[,] grids;

    public GameObject terrainParent;
    public float gridWidth;
    public float gridHeight;

    // Start is called before the first frame update
    void Start()
    {
        CalculateGrids();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 计算网格系统
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

                grids[row, col] = new Grid(x1, y1, x2, y2, gridWidth, gridHeight);
            }
        }
    }

    public void AddObjectToGrid(GameObject obj)
    {
        foreach (Grid grid in grids)
        {
            if (grid.IsPositionInGrid(obj))
            {
                // 检查物体是否已经存在于某个网格中
                if (!grid.objects.Contains(obj))
                {
                    grid.MoveObjectToGrid(obj, grid);
                }
                break;
            }
        }
    }

    // 获取指定网格周边网格内的物体
    public List<GameObject> GetNeighboringObjects(Grid grid, int range)
    {
        List<GameObject> neighboringObjects = new List<GameObject>();

        // 获取网格的行索引和列索引
        int rowIndex = Mathf.RoundToInt((grid.y1 - grids[0, 0].y1) / gridHeight);
        int colIndex = Mathf.RoundToInt((grid.x1 - grids[0, 0].x1) / gridWidth);

        // 计算周边网格的行范围和列范围
        int startRow = Mathf.Max(0, rowIndex - range);
        int endRow = Mathf.Min(grids.GetLength(0) - 1, rowIndex + range);
        int startCol = Mathf.Max(0, colIndex - range);
        int endCol = Mathf.Min(grids.GetLength(1) - 1, colIndex + range);

        // 遍历周边网格并收集物体
        for (int row = startRow; row <= endRow; row++)
        {
            for (int col = startCol; col <= endCol; col++)
            {
                neighboringObjects.AddRange(grids[row, col].objects);
            }
        }

        return neighboringObjects;
    }

    /// <summary>
    /// 查找在一定的范围之内存在的其他网格物体。（用行和列的位置信息来定位一个网格，）
    /// </summary>
    /// <param name="rowIndex">行坐标</param>
    /// <param name="colIndex">列坐标</param>
    /// <param name="competObject">指定进行位置信息对比的物体</param>
    /// <param name="range">搜索圈的范围</param>
    public void Surrounding(int rowIndex, int colIndex, GameObject competObject, int range) {
        Grid grid = grids[rowIndex, colIndex];
         // 设置范围大小
        List<GameObject> neighboringObjects = GetNeighboringObjects(grid, range);

        foreach (GameObject neighboringObject in neighboringObjects)
        {
            // 计算网格内物体与周边网格内物体之间的距离
            float distance = Vector3.Distance(neighboringObject.transform.position, competObject.transform.position);
            // 在这里进行处理逻辑


        }
    }

    // 在Scene视图中绘制网格系统的边界
    private void OnDrawGizmos()
    {
        if (grids != null)
        {
            foreach (Grid grid in grids)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(new Vector3(grid.x1, 0, grid.y1), new Vector3(grid.x2, 0, grid.y1));
                Gizmos.DrawLine(new Vector3(grid.x2, 0, grid.y1), new Vector3(grid.x2, 0, grid.y2));
                Gizmos.DrawLine(new Vector3(grid.x2, 0, grid.y2), new Vector3(grid.x1, 0, grid.y2));
                Gizmos.DrawLine(new Vector3(grid.x1, 0, grid.y2), new Vector3(grid.x1, 0, grid.y1));
            }
        }
    }
}
