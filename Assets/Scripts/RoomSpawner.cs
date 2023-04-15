using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

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
        templates = GameObject.FindGameObjectWithTag("Roads").GetComponent<RoomTemplates>();
        SetTileScale();   
        Invoke("Spawn", 0.05f);
    }

    private void Spawn()
    {
        if (spawned == false)
        {
            switch (openingDirection)
            {
                case 1:
                    spawnRoom(templates.bottomRoads, 1);
                    break;
                case 2:
                    spawnRoom(templates.topRoads, 2);
                    break;
                case 3:
                    spawnRoom(templates.leftRoads, 3);
                    break;
                case 4:
                    spawnRoom(templates.rightRoads, 4);
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

    private void SetTileScale()
    {
        for (int i = 0; i < templates.topRoads.Length; i++)
            templates.topRoads[i].transform.localScale = templates.tileScale;
        for (int i = 0; i < templates.bottomRoads.Length; i++)
            templates.bottomRoads[i].transform.localScale = templates.tileScale;
        for (int i = 0; i < templates.rightRoads.Length; i++)
            templates.rightRoads[i].transform.localScale = templates.tileScale;
        for (int i = 0; i < templates.leftRoads.Length; i++)
            templates.leftRoads[i].transform.localScale = templates.tileScale;
    }


    private void spawnRoom(GameObject[] roads, int direction)
    {
        rand = Random.Range(0, roads.Length);
        GameObject road = roads[rand];
        road.GetComponent<AddRoom>().previousRoad = gameObject.transform.parent.gameObject;
        Instantiate(road, transform.position, road.transform.rotation);
    }
}