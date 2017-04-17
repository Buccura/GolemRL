using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class npc_Pathfinding : MonoBehaviour {
   
    public int x,y;

    public List<Node> currentPath = null;
    List<Node> unvisitedBase;
    public BoardManager bm;
    public GameObject player;

    void Start()
    {
        unvisitedBase = new List<Node>();
        bm = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BoardManager>();
        Debug.Log(bm.boardHeight);
        foreach (Vector2 v in bm.pathfindingGrid.Keys)
        {
            unvisitedBase.Add(bm.pathfindingGrid[v]);
        }
    }


    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down,out hit))
        {
            if (hit.collider.gameObject.GetComponent<env_floorTile>())
            {
                x = hit.collider.gameObject.GetComponent<env_floorTile>().x;
                y = hit.collider.gameObject.GetComponent<env_floorTile>().y;
            }
        }


        if (Input.GetKeyDown(KeyCode.P))
        {
            GeneratePathTo(0, 0);
        }

        if (currentPath != null)
        {
            int currNode = 0;
            while (currNode < currentPath.Count - 1)
            {
                Vector3 start = new Vector3(x, y, 1f) * 2f;
                Vector3 end = new Vector3(currentPath[currNode + 1].x, currentPath[currNode + 1].y, 1f) * 2f;

                Debug.DrawLine(start, end, Color.red);
                currNode++;
            }
        }
    }

    public void GeneratePathTo(int tX, int tY)
    {
        // Clear out the old path
       currentPath = null;
         
        Dictionary<Node,float> dist = new Dictionary<Node, float>();
        Dictionary<Node,Node> prev = new Dictionary<Node, Node>();

        // Sets up the "Q" -- the list of nodes we have not visted yet
        List<Node> unvisited = new List<Node>();
        
        Node source = new Node();
        Node target = new Node();
        source.x = x; source.y = y;
        target.x = tX; target.y = tY;
      

        dist[source] = 0f;
        prev[source] = null;

        unvisited = unvisitedBase;
        foreach (Node v in unvisited)
        {
            if (v != source)
            {
                dist[v] = Mathf.Infinity;
                prev[v] = null;
            }
        }




     
        /*     while (unvisited.Count > 0)
        {
            Node u = null;
            foreach (Node possibleU in unvisited)
            {
                if (u == null || dist[possibleU] < dist[u])
                {
                    u = possibleU;
                }
            }

            if (u == target)
            {
                break;
            }

            unvisited.Remove(u);

          foreach (Node v in u.neighbours)
            {
                float alt = dist[u] + CostToEnterTile(x, y, v.x, v.y);

                if (alt < dist[v])
                {
                    dist[v] = alt;
                    prev[v] = u;
                }
            }
        }*/

        if (prev[target] == null)
        {
            return;
        }

       // List<Node> currentPath = new List<Node>();
        Node curNode = target;

        while (curNode != null)
        {
            currentPath.Add(curNode);
            curNode = prev[curNode];
        }

        currentPath.Reverse();
    }

    float CostToEnterTile(int sX,int sY, int tX, int tY)
    {
        float cost = 0;
        cost = bm.gridCost[tX, tY];

        if (sX != tX && sY != tY)
        {
            cost += 0.001f;
        }

        return cost;
    }


}
