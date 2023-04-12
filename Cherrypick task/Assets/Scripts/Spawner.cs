using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GridCreator grid;

    public GameObject itemToSpawn;

    private Vector2Int currentPosition;
    private List<Color> itemColors = new List<Color>();

    private bool isDragging = false;
    private Slot lastDraggedSlot = null;

    private void Start()    
    {
        itemColors.Add(Color.red);
        itemColors.Add(Color.blue);
        itemColors.Add(Color.green);

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
    
    private Vector2Int CheckClockwiseForNewPosition(int gridPositionX, int gridPositionY)
    {
        int x = gridPositionX;
        int y = gridPositionY;
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

            int upperRightAndLowerLeftTurns = gridPositionX - gridPositionY;
            int lowerRightAndUpperLeftTurns = gridPositionX + gridPositionY;
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

    public void OnMouseDown()
    {
        isDragging = true;
        grid.gridArray[currentPosition.x, currentPosition.y].isOccupied = false;
    }

    public void OnMouseUp()
    {
        isDragging = false;

        Vector2Int newPosition = CheckClockwiseForNewPosition(lastDraggedSlot.positionX, lastDraggedSlot.positionY);
        if(newPosition.x != -500)
        {
            SetPosition(newPosition);
        }
    }

    private void OnMouseDrag()
    {
        if(isDragging)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePosition.x, mousePosition.y, 0f);

            int spawnerLayerMask = 1 << LayerMask.NameToLayer("Spawner");
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, ~spawnerLayerMask);

            if (hit.collider != null && hit.collider.tag == "Slot" && hit.collider.GetComponent<Slot>() != lastDraggedSlot)
            {
                lastDraggedSlot = hit.collider.GetComponent<Slot>();
            }
        }
    }

    private Color GetRandomColorRGB()
    {
        return itemColors[Random.Range(0, 3)];
    }

    public IEnumerator SpawnItems()
    {
        Vector2Int spawnPosition = CheckClockwise();
        while (spawnPosition.x != -500)
        {
            Vector3 gridSlotWorldPosition = grid.gridArray[spawnPosition.x, spawnPosition.y].transform.position;
            Vector3 gridSlotWithSpawnerWorldPosition = grid.gridArray[currentPosition.x, currentPosition.y].transform.position;

            GameObject item = Instantiate(itemToSpawn, gridSlotWithSpawnerWorldPosition, Quaternion.identity);
            var randomColor = GetRandomColorRGB();
            item.GetComponent<SpriteRenderer>().material.color = randomColor;

            grid.gridArray[spawnPosition.x, spawnPosition.y].isOccupied = true;
            grid.gridArray[spawnPosition.x, spawnPosition.y].item = item;
            grid.gridArray[spawnPosition.x, spawnPosition.y].itemColor = randomColor;

            StartCoroutine(MoveItem(item.transform, gridSlotWithSpawnerWorldPosition, gridSlotWorldPosition));

            spawnPosition = CheckClockwise();
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    private IEnumerator MoveItem(Transform itemTransform, Vector3 startPosition, Vector3 endPosition)
    {
        float startTime = Time.time;
        float endTime = startTime + 0.5f;

        while (Time.time < endTime)
        {
            float t = (Time.time - startTime) / 0.5f;
            itemTransform.position = Vector2.Lerp(startPosition, endPosition, t);
            yield return null;
        }

        itemTransform.position = new Vector3(endPosition.x, endPosition.y, 0);
    }


}
