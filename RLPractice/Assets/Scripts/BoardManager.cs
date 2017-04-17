using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

	public enum tileType
    {
        wall,floor,blank,
    }

    public enum propType
    {
        none,ignore, dood, badguy,item,goal,
    }

    public class room
    {
        public int x, y, width, height, roomID;
        public bool up, down, left, right, connectionMade;
        public room(int nX, int nY,int w, int h, int i)
        {
            up = down = left = right = connectionMade = false;
            x = nX; y = nY; width = w; height = h; roomID = i;
        }

        public void circRoom(tileType[,] g) // This will cut out corners to make the room a different shape than just square or rectangle
        {
            g[x + (width / 2)-1, y + (height / 2)] = tileType.blank;
            g[x + (width / 2)-1, y - (height / 2)+1] = tileType.blank;
            g[x - (width / 2), y + (height / 2)] = tileType.blank;
            g[x - (width / 2), y - (height / 2) + 1] = tileType.blank;
        }

        public float getDist(int x2,int y2)
        {
            return Mathf.Sqrt(Mathf.Pow(x - x2, 2) + Mathf.Pow(y - y2, 2));
        }


    }

    public class floorTile
    {
        public int x, y;
        public floorTile(int nX, int nY)
        {
            x = nX; y = nY;
        }

    }

    public GameObject plane;

    public int boardHeight;
    public int boardWidth;
    public tileType[,] grid;  // Grid for the terrain
    public propType[,] pGrid; // Grid for the props
    public float[,] gridCost; // grid containing the cost data
    public GameObject wallPiece;
    public GameObject floorPiece;
    public GameObject[] doodads; // Holds all the doodad props
    public GameObject[] badGuys; // Holds all the types of badguys
    public GameObject[] items; // Holds all the types of items
    public GameObject goal; // Holds the goal object

    [Range(0.0f,100.0f)]
    public float doodadRatio;

    [Range(0.0f, 100.0f)]
    public float badGuyRatio;

    public int numberOfRooms;
    private int maxRooms;

    public int roomWidthMin;
    public int roomWidthMax;
    public int roomHeightMin;
    public int roomHeightMax;
    public int corridorMin;
    public int corridorMax;
    List<GameObject> walls;

    MeshFilter[] meshFilters;

    List<room> rooms;
    List<floorTile> floors;

    public string seed;
    public bool useRandomSeed;
    System.Random pseudoRandom;

    public GameObject wallsParent;
    public bool dungeonDone;

    public int playerX, playerY;

    public Dictionary<Vector2,Node> pathfindingGrid;
    

    void Start()
    {
        if(useRandomSeed)
        {
            seed = (Random.Range(0, 999999999f) + Time.time).ToString();
        }
        
        pseudoRandom = new System.Random(seed.GetHashCode());


        maxRooms = numberOfRooms;
        grid = new tileType[boardHeight, boardWidth];
        pGrid = new propType[boardHeight, boardWidth];
        gridCost = new float[boardHeight, boardWidth];
        rooms = new List<room>();
        floors = new List<floorTile>();
        Debug.Log("Prepairing the board...");
        resetBoard();
        pGrid[boardWidth / 2, boardHeight / 2] = propType.ignore;
        Debug.Log("Building dungeon...");
        buildRoom(boardWidth / 2, boardHeight/2, pseudoRandom.Next(roomWidthMin, roomWidthMax + 1), pseudoRandom.Next(roomHeightMin, roomHeightMax + 1));
        Debug.Log("Tweaking the room shapes...");
        tweakRoomShapes();
        Debug.Log("Making random connections...");
        randomConnections();
        Debug.Log("Building walls...");
        buildWalls();
        Debug.Log("Spawning goal");
        spawnGoal();
        Debug.Log("Spawning doodads...");
        spawnDoodads();
     //   Debug.Log("Creating pathfinding database...");
     //   generatePathFindingGrid();
     //   generateCostGrid();
        Debug.Log("Spawning badguys...");
        spawnBadGuys();
        Debug.Log("Drawing dungeon...");
        drawBoard();
        Debug.Log("Dungeon drawn.");
        dungeonDone = true;
        
        

    }

    void resetBoard()
    {
        for(int x = 0; x<boardWidth;x++)
        {
            for(int y = 0; y<boardHeight;y++)
            {
                grid[x,y] = tileType.blank;
                pGrid[x, y] = propType.none;
            }
        }
    }

    // This is used to draw out the board using the information from the grid array
    void drawBoard() 
    {
        // This will go through every tile in the grid and create a piece based on what it's labeled as.
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++) 
            {
                if(grid[x,y] == tileType.wall) 
                {
                    Vector3 pos = new Vector3((x - (boardWidth/2)), 1, (y - (boardHeight/2)));
                    GameObject wall;
                    pos *= 2;
                    wall = Instantiate(wallPiece, pos, transform.rotation);
                    wall.transform.parent = wallsParent.transform;
                }
                if (grid[x, y] == tileType.floor)
                {
                    Vector3 pos = new Vector3((x - (boardWidth / 2)), 0, (y - (boardHeight / 2)));
                    pos *= 2;
                    GameObject floor;
                    floor = Instantiate(floorPiece, pos, transform.rotation);
                    floor.transform.parent = plane.transform;
                    floor.GetComponent<env_floorTile>().x = x;
                    floor.GetComponent<env_floorTile>().y = y;
                }
                if (pGrid[x, y] == propType.dood && doodads.Length != 0) // This wil create a randomly chosen doodad
                {
                    Vector3 pos = new Vector3((x - (boardWidth / 2)), 0, (y - (boardHeight / 2)));
                    pos *= 2;
                    GameObject dood;
                    dood = Instantiate(doodads[pseudoRandom.Next(0,doodads.Length)], pos, transform.rotation);
                    dood.transform.parent = plane.transform;
                }
                if (pGrid[x, y] == propType.badguy && badGuys.Length != 0) // This wil create a randomly chosen doodad
                {
                    Vector3 pos = new Vector3((x - (boardWidth / 2)), 3, (y - (boardHeight / 2)));
                    pos *= 2;
                    GameObject bad;
                    bad = Instantiate(badGuys[pseudoRandom.Next(0, badGuys.Length)], pos, transform.rotation);
                    bad.transform.parent = plane.transform;
//                    bad.GetComponent<npc_Pathfinding>().startX = x;
  //                  bad.GetComponent<npc_Pathfinding>().startY = y;
                }
                if (pGrid[x,y] == propType.goal)
                {
                    Vector3 pos = new Vector3((x - (boardWidth / 2)), 0, (y - (boardHeight / 2)));
                    pos *= 2;
                    GameObject goalPos;
                    goalPos = Instantiate(goal, pos, transform.rotation);
                    goalPos.transform.parent = plane.transform;
                }

            }
        }
    } // End of drawBoard

    // For building a room
    void buildRoom(int xC, int yC, int rW, int rH)
    {
        if (numberOfRooms > 0 && regionCheck(xC,yC, rW, rH) )
        {
            int roomWidth = rW;
            int roomHeight = rH;
            // Builds the actual room
            room newRoom = new room(xC, yC, rW, rH, (maxRooms - numberOfRooms) );
            
            
            numberOfRooms--;
            // Creates a room based on the chosen height and width using xC and yC as the center of the room.
            for (int y = (yC + roomHeight/2); y > (yC - roomHeight/2); y--)
            {
                for (int x = (xC - roomWidth/2); x < (xC + roomWidth/2); x++)
                {
                    
                    grid[x, y] = tileType.floor;

                   
                    floorTile newFloor = new floorTile(x, y);
                    floors.Add(newFloor);

                    int[] addCords = new int[2];
                    addCords[0] = x;
                    addCords[1] = y;
                    


                }
            }

            // For building rooms connected to this one
            int conRooms = 1;
            int numAttempts = 0; // This is meant to prevent infinite loops
            if (numberOfRooms >= 4) { conRooms = 4; }
            else { conRooms = numberOfRooms; }

            for(int i = 0; i < conRooms; i++)
            {
                
                bool roomChosen = false; // This flag is used to inform if the room has been chosen
                int nextRoom = 0; // The number designation for the nest room. 0=Up, 1=Right,2=Down,3=Left  

                
                int nextWidth = pseudoRandom.Next(roomWidthMin, roomWidthMax + 1);
                int nextHeight = pseudoRandom.Next(roomHeightMin, roomHeightMax + 1);
                int cordLength = pseudoRandom.Next(corridorMin, corridorMax + 1);

                nextRoom = pseudoRandom.Next(0, 4);



                int nX;
                int nY;
                while (!roomChosen)
                {
                    
                    if (numberOfRooms > 0)
                    {

                        switch (nextRoom)
                        {
                            case 0: // Up
                                
                                nX = xC;
                                nY = yC + (roomHeight / 2) + cordLength + (nextHeight / 2);
                                if (isRoomClear(nX, nY, nextWidth, nextHeight))
                                {
                                    for (int z = 0; z <= cordLength; z++) // This creates the hallway that connects the rooms
                                    {
                                        
                                        grid[xC, yC + (rH / 2) + z] = tileType.floor;
                                        pGrid[xC, yC + (rH / 2) + z] = propType.ignore;
                                        grid[xC-1, yC + (rH / 2) + z] = tileType.floor;
                                        grid[xC+1, yC + (rH / 2) + z] = tileType.floor;
                                        floorTile newFloor = new floorTile(xC, yC + (rH / 2) + z);
                                        floors.Add(newFloor);
                                        newFloor = new floorTile(xC-1, yC + (rH / 2) + z);
                                        floors.Add(newFloor);
                                        newFloor = new floorTile(xC + 1, yC + (rH / 2) + z);
                                        floors.Add(newFloor);

                                    }
                                    newRoom.up = true;
                                    roomChosen = true;
                                    
                                    buildRoom(nX, nY, nextWidth, nextHeight);
                                }
                                else if(grid[xC,yC + cordLength + 1] == tileType.floor)
                                {
                                    for (int z = 0; z <= cordLength; z++)
                                    {
                                        grid[xC, yC + (rH / 2) + z] = tileType.floor;
                                        pGrid[xC, yC + (rH / 2) + z] = propType.ignore;
                                        grid[xC - 1, yC + (rH / 2) + z] = tileType.floor;
                                        grid[xC + 1, yC + (rH / 2) + z] = tileType.floor;
                                        floorTile newFloor = new floorTile(xC, yC + (rH / 2) + z);
                                        floors.Add(newFloor);
                                        newFloor = new floorTile(xC - 1, yC + (rH / 2) + z);
                                        floors.Add(newFloor);
                                        newFloor = new floorTile(xC + 1, yC + (rH / 2) + z);
                                        floors.Add(newFloor);
                                    }
                                    newRoom.up = true;

                                }


                                break;
                            case 1: // Right
                                
                                nX = xC + (roomWidth / 2) + cordLength + (nextWidth / 2);
                                nY = yC;

                                if (isRoomClear(nX, nY, nextWidth, nextHeight))
                                {
                                    for (int z = 0; z <= cordLength; z++)
                                    {
                                        grid[xC + (rW / 2) + z, yC] = tileType.floor;
                                        pGrid[xC + (rW / 2) + z, yC] = propType.ignore;
                                        grid[xC + (rW / 2) + z, yC+1] = tileType.floor;
                                        grid[xC + (rW / 2) + z, yC-1] = tileType.floor;
                                        floorTile newFloor = new floorTile(xC + (rW / 2) + z, yC);
                                        floors.Add(newFloor);
                                        newFloor = new floorTile(xC + (rW / 2) + z, yC+1);
                                        floors.Add(newFloor);
                                        newFloor = new floorTile(xC + (rW / 2) + z, yC - 1);
                                        floors.Add(newFloor);
                                    }
                                    newRoom.right = true;
                                    roomChosen = true;
                                    buildRoom(nX, nY, nextWidth, nextHeight);
                                }
                                else if (grid[xC + cordLength + 1, yC ] == tileType.floor)
                                {
                                    for (int z = 0; z <= cordLength; z++)
                                    {
                                        grid[xC + (rW / 2) + z, yC] = tileType.floor;
                                        pGrid[xC + (rW / 2) + z, yC] = propType.ignore;
                                        grid[xC + (rW / 2) + z, yC + 1] = tileType.floor;
                                        grid[xC + (rW / 2) + z, yC - 1] = tileType.floor;
                                        floorTile newFloor = new floorTile(xC + (rW / 2) + z, yC);
                                        floors.Add(newFloor);
                                        newFloor = new floorTile(xC + (rW / 2) + z, yC+1);
                                        floors.Add(newFloor);
                                        newFloor = new floorTile(xC + (rW / 2) + z, yC - 1);
                                        floors.Add(newFloor);

                                    }
                                    newRoom.right = true;

                                }

                                    break;
                            case 2: // Down
                                
                                nX = xC;
                                nY = yC - (roomHeight / 2) - cordLength - (nextHeight / 2);

                                if (isRoomClear(nX, nY, nextWidth, nextHeight))
                                {
                                    roomChosen = true;
                                    for (int z = 0; z <= cordLength + 1; z++)
                                    {
                                        grid[xC, yC - (rH / 2) - z] = tileType.floor;
                                        pGrid[xC, yC - (rH / 2) - z] = propType.ignore;
                                        grid[xC+1, yC - (rH / 2) - z] = tileType.floor;
                                        grid[xC-1, yC - (rH / 2) - z] = tileType.floor;
                                        floorTile newFloor = new floorTile(xC, yC - (rH / 2) - z);
                                        floors.Add(newFloor);
                                        newFloor = new floorTile(xC-1, yC - (rH / 2) - z);
                                        floors.Add(newFloor);
                                        newFloor = new floorTile(xC+1, yC - (rH / 2) - z);
                                        floors.Add(newFloor);
                                    }
                                    newRoom.down = true;
                                    buildRoom(nX, nY, nextWidth, nextHeight);
                                }
                                else if (grid[xC, yC - cordLength - 1] == tileType.floor)
                                {
                                    for (int z = 0; z <= cordLength + 1; z++)
                                    {
                                        grid[xC, yC - (rH / 2) - z] = tileType.floor;
                                        pGrid[xC, yC - (rH / 2) - z] = propType.ignore;
                                        grid[xC + 1, yC - (rH / 2) - z] = tileType.floor;
                                        grid[xC - 1, yC - (rH / 2) - z] = tileType.floor;
                                        floorTile newFloor = new floorTile(xC, yC - (rH / 2) - z);
                                        floors.Add(newFloor);
                                        newFloor = new floorTile(xC - 1, yC - (rH / 2) - z);
                                        floors.Add(newFloor);
                                        newFloor = new floorTile(xC + 1, yC - (rH / 2) - z);
                                        floors.Add(newFloor);
                                    }
                                    newRoom.down = true;

                                }

                                    break;
                            case 3: // Left
                                
                                nX = xC - (roomWidth / 2) - cordLength - (nextWidth / 2);
                                nY = yC;

                                if (isRoomClear(nX, nY, nextWidth, nextHeight))
                                {
                                    roomChosen = true;
                                    for (int z = 0; z <= cordLength; z++)
                                    {
                                        grid[xC - (rW / 2) - z, yC] = tileType.floor;
                                        pGrid[xC - (rW / 2) - z, yC] = propType.ignore;
                                        grid[xC - (rW / 2) - z, yC+1] = tileType.floor;
                                        grid[xC - (rW / 2) - z, yC-1] = tileType.floor;
                                        floorTile newFloor = new floorTile(xC - (rW / 2) - z, yC);
                                        floors.Add(newFloor);
                                        newFloor = new floorTile(xC - (rW / 2) - z, yC+1);
                                        floors.Add(newFloor);
                                        newFloor = new floorTile(xC - (rW / 2) - z, yC - 1);
                                        floors.Add(newFloor);
                                    }
                                    newRoom.left = true;
                                    buildRoom(nX, nY, nextWidth, nextHeight);
                                }
                                else if (grid[xC + cordLength + 1, yC] == tileType.floor)
                                {
                                    for (int z = 0; z <= cordLength; z++)
                                    {
                                        grid[xC - (rW / 2) - z, yC] = tileType.floor;
                                        pGrid[xC - (rW / 2) - z, yC] = propType.ignore;
                                        grid[xC - (rW / 2) - z, yC + 1] = tileType.floor;
                                        grid[xC - (rW / 2) - z, yC - 1] = tileType.floor;
                                        floorTile newFloor = new floorTile(xC - (rW / 2) - z, yC);
                                        floors.Add(newFloor);
                                        newFloor = new floorTile(xC - (rW / 2) - z, yC + 1);
                                        floors.Add(newFloor);
                                        newFloor = new floorTile(xC - (rW / 2) - z, yC - 1);
                                        floors.Add(newFloor);
                                    }
                                    newRoom.left = true;

                                }

                                break;
                        }
                    }
                    else
                    {
                        roomChosen = true;
                    }

                    if(!roomChosen) // If a room is not chosen, it will attempt a different direction
                    {
                        nextRoom = pseudoRandom.Next(0, 4);
                        numAttempts++;
                          if(nextRoom > 3)
                          {
                              nextRoom = 0;
                          }
                    }
                    if(numAttempts > 3) { break; }

                }
            }
            rooms.Add(newRoom);
        }
        else
        {
            numberOfRooms++;
        }



    } // end of "Build Room"

    // Check to make sure the room being made is within the grid
    bool regionCheck(int x, int y,int width, int height)
    {
        if(x >= (boardWidth - roomWidthMax)) {  return false; } // This makes sure it's X coordinate is within the X- border
        if(x <= (roomWidthMax)) { return false;  } // This makes sure it's X coordinate is within the X+ border
        if (y <= (roomHeightMax)) { return false;  } // "  " Y " " Y+ border
        if (y >= (boardHeight - roomHeightMax)) { return false;  } // " " Y " " Y- border

        for(int xR = x - (width/2); xR<x+(width/2)+1;xR++)
        {
            for(int yR = y-(height/2); yR<y+(height/2)+1;yR++)
            {
                if(grid[xR,yR] == tileType.wall)
                {
                    return false;
                }
            }
        }

        return true;
    } // end of regionCheck
    
    // Makes sure the room is clear and not taken by any floor tiles
    bool isRoomClear(int xC,int yC,int rW,int rH)
    {
        
        for (int y = (yC + rH / 2)+1; y > (yC - rH / 2)-1; y--)
        {
            for (int x = (xC - rW / 2)-1; x < (xC + rW / 2)+1; x++)
            {
                if(grid[x,y] == tileType.floor) {return false; }
            }
        }

        return true;
    } // end of isRoomClear

    // This creates walls around every floor tile that is blank
    void buildWalls()
    {
        int i = 0;
        int x, y;

        while(i < floors.Count)
        {
            x = floors[i].x;
            y = floors[i].y;

           for(int xF = x-1; xF <= x+1; xF++)
            {
                for(int yF = y-1; yF <= y+1;yF++)
                {
                    if(grid[xF,yF] == tileType.blank)
                    {
                        grid[xF, yF] = tileType.wall;
                    }
                }
            }
            i++;
        }           
    } // End of buildWalls


    // Randomly, this will make a room a different shape.
    void tweakRoomShapes()
    {
        foreach(room r in rooms)
        {
            if(pseudoRandom.Next(0,100) == 0)
            {
                r.circRoom(grid);
            }
        }
    }

    // This creates additional connections between rooms in an attempt to make the dungeon less linear
    void randomConnections()
    {
        int i = 0;
        while(i < rooms.Count)
        {
            int j = 0;
            while(j < rooms.Count)
            {
                if (rooms[i].getDist(rooms[j].x, rooms[j].y) <= 20 && !rooms[j].connectionMade)
                {
                    rooms[j].connectionMade = true;
                    // Builds a corridor upward or downward
                    if( Mathf.Abs(rooms[i].x - rooms[j].x) <= 3)
                    {

                        if (rooms[i].y <= rooms[j].y && rooms[j].down == false)
                        {
                            for (int y = rooms[i].y; y <= rooms[j].y; y++)
                            {
                                grid[rooms[i].x, y] = tileType.floor;
                                pGrid[rooms[i].x, y] = propType.ignore;
                                grid[rooms[i].x, y-1] = tileType.floor;
                                grid[rooms[i].x, y+1] = tileType.floor;
                                floorTile newFloor = new floorTile(rooms[i].x, y);
                                floors.Add(newFloor);
                                newFloor = new floorTile(rooms[i].x, y + 1);
                                floors.Add(newFloor);
                                newFloor = new floorTile(rooms[i].x, y - 1);
                                floors.Add(newFloor);
                            }
                            rooms[j].down = true;
                        }
                        else if(rooms[j].up == false)
                        {

                            for (int y = rooms[i].y; y > rooms[j].y; y--)
                            {

                                grid[rooms[i].x, y] = tileType.floor;
                                pGrid[rooms[i].x, y] = propType.ignore;
                                grid[rooms[i].x, y - 1] = tileType.floor;
                                grid[rooms[i].x, y + 1] = tileType.floor;
                                floorTile newFloor = new floorTile(rooms[i].x, y);
                                floors.Add(newFloor);
                                newFloor = new floorTile(rooms[i].x, y - 1);
                                floors.Add(newFloor);
                                newFloor = new floorTile(rooms[i].x, y + 1);
                                floors.Add(newFloor);
                            }
                            rooms[j].up = true;
                        }
                        

                    }
                    // Builds a corridor left or right
                    if (Mathf.Abs(rooms[i].y - rooms[j].y) <= 3)
                    {
                        
                        if (rooms[i].x <= rooms[j].x && rooms[j].right == false)
                        {
                            for (int x = rooms[i].x; x <= rooms[j].x; x++)
                            {

                                grid[x, rooms[i].y] = tileType.floor;
                                pGrid[x, rooms[i].y] = propType.ignore;
                                grid[x-1, rooms[i].y] = tileType.floor;
                                grid[x+1, rooms[i].y] = tileType.floor;
                                floorTile newFloor = new floorTile(x, rooms[i].y);
                                floors.Add(newFloor);
                                newFloor = new floorTile(x + 1, rooms[i].y);
                                floors.Add(newFloor);
                                newFloor = new floorTile(x - 1, rooms[i].y);
                                floors.Add(newFloor);
                            }
                            rooms[j].right = true;
                        }
                        else if(rooms[j].left == false)
                        {
                            for (int x = rooms[i].x; x > rooms[j].x; x--)
                            {

                                grid[x, rooms[i].y] = tileType.floor;
                                pGrid[x, rooms[i].y] = propType.ignore;
                                grid[x - 1, rooms[i].y] = tileType.floor;
                                grid[x + 1, rooms[i].y] = tileType.floor;
                                floorTile newFloor = new floorTile(x, rooms[i].y);
                                floors.Add(newFloor);
                                newFloor = new floorTile(x - 1, rooms[i].y);
                                floors.Add(newFloor);
                                newFloor = new floorTile(x + 1, rooms[i].y);
                                floors.Add(newFloor);
                            }
                            rooms[j].left = true;
                        }
                    }
                }
                j++;
            }


            i++;
        }

    } // End of random connections


    void spawnDoodads()
    {
        foreach(room r in rooms)
        {
            for(int x = r.x - (r.width/2);x <= r.x + (r.width/2);x++)
            {
                for(int y = r.y - (r.height/2);y <= r.y + (r.height/2);y++)
                {
                    if(grid[x,y] == tileType.floor && pGrid[x,y] == propType.none)
                    {
                        if(pseudoRandom.Next(0,100) <= doodadRatio)
                        {
                            pGrid[x, y] = propType.dood;
                            for(int xD = x-1; xD <= x+1;xD++) // This will randomly spawn doodads around the one just created
                            {
                                for (int yD = y - 1; yD <= y + 1; yD++)
                                {
                                    if(grid[xD,yD] == tileType.floor && pGrid[xD,yD] == propType.none)
                                    {
                                        if(pseudoRandom.Next(0,100) <= 50)
                                        {
                                            pGrid[xD, yD] = propType.dood;
                                        }
                                    }
                                }

                            }
                        }
                    }
                }
            }
        }

    } // End of spawnDoodads

    void spawnBadGuys()
    {
        foreach (room r in rooms)
        {
            for (int x = r.x - (r.width / 2); x <= r.x + (r.width / 2); x++)
            {
                for (int y = r.y - (r.height / 2); y <= r.y + (r.height / 2); y++)
                {
                    if (grid[x, y] == tileType.floor && pGrid[x, y] == propType.none)
                    {
                        if (pseudoRandom.Next(0, 100) <= badGuyRatio)
                        {
                            pGrid[x, y] = propType.badguy;
                        }
                    }
                }
            }
        }
    }

    void spawnGoal()
    {
        room endRoom;
        int rndRoom;
        if(maxRooms >= 4)
        {
            rndRoom = pseudoRandom.Next(maxRooms - 4, maxRooms - 1);
        }
        else
        {
            rndRoom = pseudoRandom.Next(1, maxRooms - 1);
        }
        
        endRoom = rooms[maxRooms - (1 + rndRoom)];
        int x, y;
        x = endRoom.x; y = endRoom.y;
        pGrid[x, y] = propType.goal;

    }

    void generatePathFindingGrid()
    {
        pathfindingGrid = new Dictionary<Vector2, Node>();



        // Initialize each spot in the array
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                Node newNode = new Node();
                Vector2 cord = new Vector2(x, y);
                pathfindingGrid.Add(cord, newNode);
            }
        }
        
        // Build it's neighbours
        foreach (Vector2 v in pathfindingGrid.Keys)
        {
            if (grid[(int)v.x, (int)v.y] == tileType.floor)
            {
                if (grid[(int)v.x -1, (int)v.y] == tileType.floor)
                {

                    Vector2 key = new Vector2();
                    key.x = v.x - 1;
                    key.y = v.y;
                    pathfindingGrid[v].neighbours.Add(pathfindingGrid[key]);

                }
                if (grid[(int)v.x + 1, (int)v.y] == tileType.floor)
                {
                    Vector2 key = new Vector2();
                    key.x = v.x + 1;
                    key.y = v.y;
                    pathfindingGrid[v].neighbours.Add(pathfindingGrid[key]);

                }
                if (grid[(int)v.x - 1, (int)v.y-1] == tileType.floor)
                {
                    Vector2 key = new Vector2();
                    key.x = v.x;
                    key.y = v.y - 1;
                    pathfindingGrid[v].neighbours.Add(pathfindingGrid[key]);
                }
                if (grid[(int)v.x - 1, (int)v.y+1] == tileType.floor)
                {
                    Vector2 key = new Vector2();
                    key.x = v.x;
                    key.y = v.y + 1;
                    pathfindingGrid[v].neighbours.Add(pathfindingGrid[key]);
                }
            }
        }
    }

    public void generateCostGrid()
    {
        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                gridCost[x, y] = Mathf.Infinity;
                if (grid[x, y] == tileType.floor && pGrid[x, y] == propType.none)
                {
                    gridCost[x, y] = 0f;
                }
            }

        }
    }

}
