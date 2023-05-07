using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public int openingDirection;
    private RoomTemplates templates;
    private int rand;
    private bool spawned = false;

    private void Start()
    {
        templates = transform.parent.parent.GetComponent<RoomTemplates>();
        SetTileScale();   
        Invoke(nameof(Spawn), 0.03f);
    }

    private void Spawn()
    {
        if (spawned == false)
        {
            switch (openingDirection)
            {
                case 1:
                    SpawnRoom(templates.bottomRoads);
                    break;
                case 2:
                    SpawnRoom(templates.topRoads);
                    break;
                case 3:
                    SpawnRoom(templates.leftRoads);
                    break;
                case 4:
                    SpawnRoom(templates.rightRoads);
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


    private void SpawnRoom(GameObject[] roads)
    {
        rand = Random.Range(0, roads.Length);
        GameObject road = roads[rand];
        road.GetComponent<AddRoom>().previousRoad = gameObject.transform.parent.gameObject;
        Instantiate(road, transform.position, road.transform.rotation, templates.gameObject.transform);
    }
}