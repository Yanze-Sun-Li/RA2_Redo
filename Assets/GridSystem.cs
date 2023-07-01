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
    public Object[] objects;

    public Grid(float x1, float y1, float x2, float y2, float width, float height)
    {
        this.x1 = x1;
        this.y1 = y1;
        this.x2 = x2;
        this.y2 = y2;
        this.width = width;
        this.height = height;
        objects = new Object[0];
    }

    public float calculateArea()
    {
        return width * height;
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
