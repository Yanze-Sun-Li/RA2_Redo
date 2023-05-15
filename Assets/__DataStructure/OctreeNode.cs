using System.Collections.Generic;
using UnityEngine;

public struct OctreeObject {
    public Bounds bounds;
    public GameObject gameObject;
    public OctreeObject(GameObject _gameObject) {
        bounds = _gameObject.GetComponent<Collider>().bounds;
        gameObject = _gameObject;
    }
}

public class OctreeNode
{
    Bounds nodeBounds;
    Bounds[] childrenBounds;
    public OctreeNode[] children = null;
    float minSize;
    List<GameObject> containedGameObject = new List<GameObject>();

    public OctreeNode(Bounds bounds, float minNodeSize) {
        nodeBounds = bounds;
        minSize = minNodeSize;
    }

    public void Draw() {
        Gizmos.color = new Color(0, 1, 0);
        Gizmos.DrawWireCube(nodeBounds.center, nodeBounds.size);
    }
}
