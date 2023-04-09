using System.Collections.Generic;
using UnityEngine;

public class RoomTemplates : MonoBehaviour
{
    public GameObject[] bottomRooms;
    public GameObject[] topRooms;
    public GameObject[] leftRooms;
    public GameObject[] rightRooms;
    public GameObject finish;
    public GameObject car;

    public List<GameObject> rooms;
    public Vector3 tileScale;
    private bool spawFinish = false;
    private float roomSpawnTime = 5;


    private void Update()
    {
        if (roomSpawnTime <= 0 && spawFinish == false)
        {
            Instantiate(finish, rooms[rooms.Count - 1].transform.position, Quaternion.identity);
            spawFinish = true;
        }
        else
            roomSpawnTime -= Time.deltaTime;
    }
}