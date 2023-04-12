using UnityEngine;

public class Slot : MonoBehaviour
{
    public bool isOccupied = false;
    public bool isBlocked = false;

    public GameObject item = null;
    public Color itemColor = Color.black;

    public int positionX = 0;
    public int positionY = 0;

    public void SetArrayPosition(int positionX, int positionY)
    {
        this.positionX = positionX; 
        this.positionY = positionY;
    }
}
