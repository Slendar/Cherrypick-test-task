using UnityEngine;

public class Slot : MonoBehaviour
{
    public bool isOccupied = false;
    public int positionX = 0;
    public int positionY = 0;

    public void SetArrayPosition(int positionX, int positionY)
    {
        this.positionX = positionX; 
        this.positionY = positionY;
    }
}
