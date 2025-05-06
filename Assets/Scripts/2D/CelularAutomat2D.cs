using System.Collections.Generic;
using UnityEngine;

public class CelularAutomat2D : MonoBehaviour
{
    public GameObject Grid1;
    public GameObject Grid2;

    public Vector2Int[] cells;


    /*
        Any live cell with fewer than two live neighbours dies, as if by underpopulation.
        Any live cell with two or three live neighbours lives on to the next generation.
        Any live cell with more than three live neighbours dies, as if by overpopulation.
        Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
     */

}
