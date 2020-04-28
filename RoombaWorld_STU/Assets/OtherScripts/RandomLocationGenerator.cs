using UnityEngine;
using System.Collections.Generic;
using Pathfinding;

public class RandomLocationGenerator  {

    private static List<GraphNode> allNodes;
    private static List<GameObject> patrolPoints;
    
    static RandomLocationGenerator ()
    {
        // get all the nodes in the gridgraph and save them 
        // in allNodes list.
        allNodes = new List<GraphNode>();
        GridGraph gg = AstarPath.active.data.gridGraph;
        gg.GetNodes(nod => { allNodes.Add(nod); });

        // get all the patrol points
        patrolPoints = new List<GameObject>(GameObject.FindGameObjectsWithTag("PATROLPOINT"));
        
    } 
    // QUI DONA 2 FUNCIONS X TRIAR ELS PUNTS D LA ROOMBA 
    public static Vector3 RandomWalkableLocation ()
    {
        GraphNode node = allNodes[Random.Range(0, allNodes.Count)];
        while (!node.Walkable)
        {
            node = allNodes[Random.Range(0, allNodes.Count)];
        }
        // when here node is walkable
        // return its position as a vector 3
        return (Vector3)node.position;
    }

    public static Vector3 RandomPatrolLocation ()
    {
        return patrolPoints[Random.Range(0, patrolPoints.Count)].transform.position;
    }

}
