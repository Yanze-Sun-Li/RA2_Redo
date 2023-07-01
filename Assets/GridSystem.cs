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

    // ��������ϵͳ
    void CalculateGrids()
    {
        Renderer[] childRenderers = terrainParent.GetComponentsInChildren<Renderer>();

        // ��ȡ������ı߽�
        Bounds combinedBounds = new Bounds();
        foreach (Renderer renderer in childRenderers)
        {
            combinedBounds.Encapsulate(renderer.bounds);
        }

        // ������θ�����ı߽����ĵ�
        Vector3 terrainCenter = combinedBounds.center;

        // ������θ�����ı߽��Ⱥ͸߶�
        float terrainWidth = combinedBounds.size.x;
        float terrainHeight = combinedBounds.size.z;

        // ����ÿ������ϵͳ������������
        int rowCount = Mathf.CeilToInt(terrainHeight / gridHeight);
        int colCount = Mathf.CeilToInt(terrainWidth / gridWidth);

        // ��������Ŀ�Ⱥ͸߶�
        gridWidth = terrainWidth / colCount;
        gridHeight = terrainHeight / rowCount;

        // ��ʼ����������
        grids = new Grid[rowCount, colCount];

        // ����ÿ������ı߽�����ĵ㣬�������������
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


    // ��Scene��ͼ�л�������ϵͳ�ı߽�
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
