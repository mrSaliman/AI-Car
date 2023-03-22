using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTemplates : MonoBehaviour
{
    public GameObject[] bottomRooms;
    public GameObject[] topRooms;
    public GameObject[] leftRooms;
    public GameObject[] rightRooms;

    public GameObject closedRoom;
    public GameObject car;
    public List<GameObject> rooms;
    public Vector3 tileScale;
    public float waitTime;
    private bool spawFinish;
    public GameObject finish;


    private void Update()
    {
        if(waitTime <= 0 && spawFinish == false)
        {
            Instantiate(finish, rooms[rooms.Count-1].transform.position, Quaternion.identity);
            spawFinish = true;
            waitTime = 0;
        } 
        else
        {
            waitTime -= Time.deltaTime;
        }
    }
}
