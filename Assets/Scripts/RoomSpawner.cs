using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class RoomSpawner : MonoBehaviour
{
    public int openingDirection;
    // 1 -> need bottom door
    // 2 -> need top door
    // 3 -> need left door
    // 4 -> need right door

    private RoomTemplates templates;
    private int rand;
    private bool spawned = false;

    private void Start()
    {
        templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
        Invoke("Spawn", 0.33f);
    }

    private void Spawn()
    {
        if (spawned == false)
        {
            switch (openingDirection)
            {
                case 1:
                    rand = Random.Range(0, templates.bottomRooms.Length);
                    GameObject bottomRoom = templates.bottomRooms[rand];
                    bottomRoom.transform.localScale = templates.tileScale;
                    Instantiate(bottomRoom, transform.position, bottomRoom.transform.rotation);
                    break;
                case 2:
                    rand = Random.Range(0, templates.topRooms.Length);
                    GameObject topRoom = templates.topRooms[rand];
                    topRoom.transform.localScale = templates.tileScale;
                    Instantiate(topRoom, transform.position, topRoom.transform.rotation);
                    break;
                case 3:
                    rand = Random.Range(0, templates.leftRooms.Length);
                    GameObject leftRoom = templates.leftRooms[rand];
                    leftRoom.transform.localScale = templates.tileScale;
                    Instantiate(leftRoom, transform.position, leftRoom.transform.rotation);
                    break;
                case 4:
                    rand = Random.Range(0, templates.rightRooms.Length);
                    GameObject rightRoom = templates.rightRooms[rand];
                    Instantiate(rightRoom, transform.position, rightRoom.transform.rotation);
                    break;
            }
            spawned = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SpawnPoint"))
            Destroy(gameObject);
    }
}