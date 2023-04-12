using System.Collections;
using System;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GridCreator grid;
    public GameObject itemToSpawn;

    private Vector2Int currentPosition;

    private void Start()
    {
        int x = grid.gridArray.GetLength(0) % 2 == 0 ? grid.gridArray.GetLength(0) / 2 - 1 : grid.gridArray.GetLength(0) / 2;
        
        int y = grid.gridArray.GetLength(1) % 2 == 0 ? grid.gridArray.GetLength(1) / 2 - 1 : grid.gridArray.GetLength(1) / 2;

        currentPosition.x = x;
        currentPosition.y = y;

        Vector2Int startingPosition = CheckClockwise();
        
        if(startingPosition.x == -500)
        {
            Debug.LogWarning("No aviable space for Spawner");
            return;
        }

        SetPosition(startingPosition);
        StartCoroutine(SpawnItems());
    }

    private Vector2Int CheckClockwise()
    {
        int x = currentPosition.x;
        int y = currentPosition.y;
        int directionX = 0;
        int directionY = 1;
        int width = grid.gridArray.GetLength(0);
        int height = grid.gridArray.GetLength(1);
        int maxEdgeLenght = (int)(Mathf.Pow(Mathf.Max(width, height), 2) * 3.08f + 1);

        for (int i = 0; i < maxEdgeLenght; i++)
        {
            if((x >= 0 && y >= 0) && (x < width && y < height) && !grid.gridArray[x,y].isBlocked && !grid.gridArray[x, y].isOccupied)
            {
                return new Vector2Int(x, y);
            }
            x = x + directionX;
            y = y + directionY;

            int upperRightAndLowerLeftTurns = currentPosition.x - currentPosition.y;
            int lowerRightAndUpperLeftTurns = currentPosition.x + currentPosition.y;
            if ((x - y == upperRightAndLowerLeftTurns) || (x + y == lowerRightAndUpperLeftTurns && x - y > upperRightAndLowerLeftTurns) || (x + y == lowerRightAndUpperLeftTurns + 1 && x - y < upperRightAndLowerLeftTurns))
            {
                int t = directionY;
                directionY = -directionX;
                directionX = t;
            }
        }
        return new Vector2Int(-500,-500);
    }

    private void SetPosition(Vector2Int newPosition)
    {
        grid.gridArray[currentPosition.x, currentPosition.y].isOccupied = false;
        currentPosition = newPosition;
        
        transform.position = grid.gridArray[newPosition.x, newPosition.y].transform.position;
        grid.gridArray[newPosition.x, newPosition.y].isOccupied = true;
    }

    private IEnumerator SpawnItems()
    {
        while (CheckClockwise().x != -500)
        {
            Vector2Int spawnPosition = CheckClockwise();
            Instantiate(itemToSpawn, grid.gridArray[spawnPosition.x, spawnPosition.y].transform.position, Quaternion.identity);
            grid.gridArray[spawnPosition.x, spawnPosition.y].isOccupied = true;
            yield return new WaitForSeconds(1f);
        }
        yield return null;
    }
}
