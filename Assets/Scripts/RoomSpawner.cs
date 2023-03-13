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
        ChangeTileScale();
        Invoke("Spawn", 0.1f);
    }

    private void Spawn()
    {
        if(spawned == false)
        {
            switch (openingDirection)
            {
                case 1:
                    rand = Random.Range(0, templates.bottomRooms.Length);
                    Instantiate(templates.bottomRooms[rand], transform.position, templates.bottomRooms[rand].transform.rotation);
                    break;
                case 2:
                    rand = Random.Range(0, templates.topRooms.Length);
                    Instantiate(templates.topRooms[rand], transform.position, templates.topRooms[rand].transform.rotation);
                    break;
                case 3:
                    rand = Random.Range(0, templates.leftRooms.Length);
                    Instantiate(templates.leftRooms[rand], transform.position, templates.leftRooms[rand].transform.rotation);
                    break;
                case 4:
                    rand = Random.Range(0, templates.rightRooms.Length);
                    Instantiate(templates.rightRooms[rand], transform.position, templates.rightRooms[rand].transform.rotation);
                    break;
            }
            spawned = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SpawnPoint"))
        {
            if(other.GetComponent<RoomSpawner>().spawned == false && spawned == false)
            {
                Instantiate(templates.closedRoom, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }

    private void ChangeTileScale()
    {
        for(int i = 0; i < templates.leftRooms.Length; i++)
            templates.leftRooms[i].transform.localScale = templates.tileScale;
        for(int i = 0; i < templates.rightRooms.Length; i++)
            templates.rightRooms[i].transform.localScale = templates.tileScale;
        for(int i = 0; i < templates.bottomRooms.Length; i++)
            templates.bottomRooms[i].transform.localScale = templates.tileScale;
        for (int i = 0; i < templates.topRooms.Length; i++)
            templates.topRooms[i].transform.localScale = templates.tileScale;
    }
}
