using UnityEngine;
using System.IO;

public class GridCreator : MonoBehaviour
{
    public Slot slotPrefab;
    public string gridSizeFilePath;

    [HideInInspector] public Slot[,] GridArray;

    void Start()
    {
        int[] gridSize = LoadGridSize(gridSizeFilePath);
        GridArray = new Slot[gridSize[0], gridSize[1]];

        Vector3 centerPosition = new Vector3(-(gridSize[0] - 1) / 2f, -(gridSize[1] - 1) / 2f, 0);

        for (int y = 0; y < gridSize[1]; y++)
        {
            for (int x = 0; x < gridSize[0]; x++)
            {
                Slot slot = Instantiate(slotPrefab, new Vector3(x, y, 0) + centerPosition, Quaternion.identity);
                slot.transform.parent = gameObject.transform;
                slot.SetArrayPosition(x, y);

                GridArray[x, y] = slot;

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
                    slot.isOccupied = true;
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
}