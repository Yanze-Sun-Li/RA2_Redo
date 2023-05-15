
using UnityEngine;

public class Octree
{
    public OctreeNode rootNode;
    public Octree(GameObject[] worldObject, float minNodeSize) {
        Bounds bounds = new Bounds();
        foreach (GameObject item in worldObject)
        {
            //This is really not efficient, but for the first step to run.
            //Remember to remove it later.
            bounds.Encapsulate(item.GetComponent<Collider>().bounds);
        }

        float maxSize = Mathf.Max(new float[] { bounds.size.x, bounds.size.y, bounds.size.z });
        Vector3 sizeVector = new Vector3(maxSize, maxSize, maxSize) * .5f;
        bounds.SetMinMax(bounds.center - sizeVector, bounds.center + sizeVector);

        rootNode = new OctreeNode(bounds,minNodeSize);
    }
}
