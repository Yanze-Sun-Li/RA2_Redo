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

    public void AddObjectToGrid(GameObject obj)
    {
        foreach (Grid grid in grids)
        {
            if (grid.IsPositionInGrid(obj))
            {
                // ��������Ƿ��Ѿ�������ĳ��������
                if (!grid.objects.Contains(obj))
                {
                    grid.MoveObjectToGrid(obj, grid);
                }
                break;
            }
        }
    }

    // ��ȡָ�������ܱ������ڵ�����
    public List<GameObject> GetNeighboringObjects(Grid grid, int range)
    {
        List<GameObject> neighboringObjects = new List<GameObject>();

        // ��ȡ�������������������
        int rowIndex = Mathf.RoundToInt((grid.y1 - grids[0, 0].y1) / gridHeight);
        int colIndex = Mathf.RoundToInt((grid.x1 - grids[0, 0].x1) / gridWidth);

        // �����ܱ�������з�Χ���з�Χ
        int startRow = Mathf.Max(0, rowIndex - range);
        int endRow = Mathf.Min(grids.GetLength(0) - 1, rowIndex + range);
        int startCol = Mathf.Max(0, colIndex - range);
        int endCol = Mathf.Min(grids.GetLength(1) - 1, colIndex + range);

        // �����ܱ������ռ�����
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
    /// ������һ���ķ�Χ֮�ڴ��ڵ������������塣�����к��е�λ����Ϣ����λһ�����񣬣�
    /// </summary>
    /// <param name="rowIndex">������</param>
    /// <param name="colIndex">������</param>
    /// <param name="competObject">ָ������λ����Ϣ�Աȵ�����</param>
    /// <param name="range">����Ȧ�ķ�Χ</param>
    public void Surrounding(int rowIndex, int colIndex, GameObject competObject, int range) {
        Grid grid = grids[rowIndex, colIndex];
         // ���÷�Χ��С
        List<GameObject> neighboringObjects = GetNeighboringObjects(grid, range);

        foreach (GameObject neighboringObject in neighboringObjects)
        {
            // �����������������ܱ�����������֮��ľ���
            float distance = Vector3.Distance(neighboringObject.transform.position, competObject.transform.position);
            // ��������д����߼�


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
