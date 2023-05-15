using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatOctree : MonoBehaviour
{
    public GameObject[] worldObjects;
    Octree octree;

    // Start is called before the first frame update
    void Start()
    {
        octree = new Octree(worldObjects,1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = new Color(0, 1, 0);
            octree.rootNode.Draw();
            //Used for bounds debug.
            //Gizmos.DrawWireCube(octree.boundsRecord.center, octree.boundsRecord.size);
        }
    }
}
