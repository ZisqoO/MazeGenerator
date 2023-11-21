using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


// If you want to make it instant, just change the Start and GenerateMaze IEnumerators to Void functions and remove the yield returns
public class MazeGenerator : MonoBehaviour
{
    [SerializeField] private MazeCell mazeCellPrefab;
    [SerializeField] private int mazeWidth;
    [SerializeField] private int mazeDepth;
    private MazeCell[,] mazeGrid;
    IEnumerator Start()
    {
        mazeGrid = new MazeCell[mazeWidth, mazeDepth];
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int z = 0; z < mazeDepth; z++)
            {
                mazeGrid[x,z] = Instantiate(mazeCellPrefab, new Vector3(x, 0, z), Quaternion.identity);
            }
        }

        yield return GenerateMaze(null, mazeGrid[0,0]);
    }
    private IEnumerator GenerateMaze(MazeCell previousCell, MazeCell currentCell)
    {
        currentCell.Visit();
        ClearWalls(previousCell, currentCell);

        yield return new WaitForSeconds(0.05f);

        MazeCell nextCell;

        do
        {
             nextCell = GetNextUnvisitedCell(currentCell);

            if (nextCell != null)
            {
                yield return GenerateMaze(currentCell, nextCell);
            }
        } while (nextCell != null);

        
    }
    private MazeCell GetNextUnvisitedCell(MazeCell currenCell)
    {
        var unvisitedCells = GetUnvisitedCells(currenCell);
        return unvisitedCells.OrderBy(_ => Random.Range(1, 10)).FirstOrDefault();
    }
    private IEnumerable<MazeCell>GetUnvisitedCells(MazeCell currenCell)
    {
        int x = (int)currenCell.transform.position.x;
        int z = (int)currenCell.transform.position.z;

        if(x+1 < mazeWidth)
        {
            var cellToRight = mazeGrid[x+1,z];

            if(cellToRight.IsVisited == false)
            {
                yield return cellToRight;
            }
        }
        if(x-1 >= 0)
        {
            var cellToLeft = mazeGrid[x-1,z];
            if(cellToLeft.IsVisited == false)
            {
                yield return cellToLeft;
            }
        }
        if(z+1 < mazeDepth)
        {
            var cellToFront = mazeGrid[x,z+1];
            if(cellToFront.IsVisited == false)
            {
                yield return cellToFront;
            }
        }
        if(z-1 >= 0)
        {
            var cellToBack = mazeGrid[x,z-1];
            if (cellToBack.IsVisited == false)
            {
                yield return cellToBack;
            }
        }
    }
    private void ClearWalls(MazeCell previousCell, MazeCell currentCell)
    {
        if(previousCell == null) 
        {
            return;
        }
        if(previousCell.transform.position.x < currentCell.transform.position.x) // moved to right
        {
            previousCell.ClearRightWall();
            currentCell.ClearLeftWall();
            return;
        }
        if(previousCell.transform.position.x > currentCell.transform.position.x) // moved left
        {
            previousCell.ClearLeftWall();
            currentCell.ClearRightWall();
            return;
        }
        if(previousCell.transform.position.z < currentCell.transform.position.z)
        {
            previousCell.ClearFrontWall();
            currentCell.ClearBackWall();
            return;
        }
        if (previousCell.transform.position.z > currentCell.transform.position.z)
        {
            previousCell.ClearBackWall();
            currentCell.ClearFrontWall();
            return;
        }
    }
}
