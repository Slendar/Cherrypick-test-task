using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class GridCreator : MonoBehaviour
{
    public Slot slotPrefab;
    public string gridSizeFilePath;

    [HideInInspector] public Slot[,] gridArray;

    void Awake()
    {
        int[] gridSize = LoadGridSize(gridSizeFilePath);
        gridArray = new Slot[gridSize[0], gridSize[1]];

        Vector3 centerPosition = new Vector3(-(gridSize[0] - 1) / 2f, -(gridSize[1] - 1) / 2f, 0);

        for (int y = 0; y < gridSize[1]; y++)
        {
            for (int x = 0; x < gridSize[0]; x++)
            {
                Slot slot = Instantiate(slotPrefab, new Vector3(x, y, 0) + centerPosition, Quaternion.identity);
                slot.transform.parent = gameObject.transform;
                slot.SetArrayPosition(x, y);

                gridArray[x, y] = slot;

                if ((x + y) % 2 == 0)
                {
                    slot.GetComponent<Renderer>().material.color = new Color (0.5f, 0.5f, 0.5f, 1);
                }
                else
                {
                    slot.GetComponent<Renderer>().material.color = new Color(0.7f, 0.7f, 0.7f, 1);
                }

                if (Random.value < 0.25f)
                {
                    slot.GetComponent<Renderer>().material.color = Color.black;
                    slot.isBlocked = true;
                }
            }
        }
    }

    int[] LoadGridSize(string filePath)
    {
        string fileContent = File.ReadAllText(filePath);

        GridSize gridSize = JsonUtility.FromJson<GridSize>(fileContent);

        return new int[] { gridSize.width, gridSize.height };
    }

    [System.Serializable]
    class GridSize
    {
        public int width;
        public int height;
    }

    public void ClearAdjacentSameColors()
    {
        List<GameObject> itemsToClear= new List<GameObject>();
        int width = gridArray.GetLength(0);
        int height = gridArray.GetLength(1);

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var currentGameObject = gridArray[x, y];
                if(currentGameObject == null || gridArray[x, y].isBlocked)
                {
                    continue;
                }

                if (x > 0 && gridArray[x - 1, y].itemColor == currentGameObject.itemColor)
                {
                    if (!itemsToClear.Contains(currentGameObject.item))
                    {
                        itemsToClear.Add(currentGameObject.item);
                        currentGameObject.isOccupied = false;
                    }
                }

                if (x < width - 1 && gridArray[x + 1, y].itemColor == currentGameObject.itemColor)
                {
                    if (!itemsToClear.Contains(currentGameObject.item))
                    {
                        itemsToClear.Add(currentGameObject.item);
                        currentGameObject.isOccupied = false;
                    }
                }

                if (y > 0 && gridArray[x, y - 1].itemColor == currentGameObject.itemColor)
                {
                    if (!itemsToClear.Contains(currentGameObject.item))
                    {
                        itemsToClear.Add(currentGameObject.item);
                        currentGameObject.isOccupied = false;
                    }
                }

                if (y < height - 1 && gridArray[x, y + 1].itemColor == currentGameObject.itemColor)
                {
                    if (!itemsToClear.Contains(currentGameObject.item))
                    {
                        itemsToClear.Add(currentGameObject.item);
                        currentGameObject.isOccupied = false;
                    }
                }
            }
        }
        foreach (var item in itemsToClear)
        {
            Destroy(item);
        }
        itemsToClear.Clear();
    }
}