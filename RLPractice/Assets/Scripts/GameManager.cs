using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public int numRooms;
    public bool[,] grid = new bool[10, 10];


    void Start()
    {
        grid[4, 4] = true;
    }


}
