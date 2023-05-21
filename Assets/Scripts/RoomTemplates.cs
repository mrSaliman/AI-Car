using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTemplates : MonoBehaviour
{
    public GameObject[] bottomRoads;
    public GameObject[] topRoads;
    public GameObject[] leftRoads;
    public GameObject[] rightRoads;
    public GameObject car;
    public GameObject player;
    public GameObject enemy;
    public bool isTraining;

    public List<GameObject> roads;
    public Vector3 tileScale;

    [SerializeField] private int numberOfCars;
    [SerializeField] private float spacing;

    [HideInInspector] public bool IsGenerationDone;
    [HideInInspector] public bool CarsAreSet;
    private int roadsCount = 0;
    private TrackCheckpoints trackCheckpoints;
    private MapLoader mapLoader;


    private void Start()
    {
        GameObject.FindGameObjectWithTag("LoadSceneCamera").GetComponent<Camera>().enabled = true;
        GameObject.Find("Main Camera").GetComponent<Camera>().enabled = false;

        trackCheckpoints = GetComponent<TrackCheckpoints>();
        mapLoader = transform.parent.GetComponent<MapLoader>();
        numberOfCars = mapLoader.enemies + 1;
        IsGenerationDone = false;
        CarsAreSet = false;
        InvokeRepeating(nameof(EndSpawning), 0f, 0.1f);
    }

    private void EndSpawning()
    {
        if (roadsCount == roads.Count)
        {
            IsGenerationDone = true;
            DeleteBorders();
            StartCoroutine(SpawnCars());
            SetCameras();
            CancelInvoke();
        }
        else
            roadsCount = roads.Count;
    }

    private void DeleteBorders()
    {
        foreach (var road in roads)
        {
            var borders = road.GetComponentsInChildren<Transform>();
            foreach (var border in borders)
                if (border.CompareTag("Border"))
                {
                    var borderCollisionCount = border.GetComponent<BorderCollisionCount>();
                    if (borderCollisionCount != null && borderCollisionCount.CollisionCount > 4)
                        Destroy(border.gameObject);
                }
        }
    }

    public float GetCarRotation()
    {
        var rightWay = GetRightWay();
        var change = (rightWay[1].transform.position - rightWay[0].transform.position);
        
        if (change.z != 0)
        {
            if (change.z > 0) return 0;
            else return 180;
        }
        else
        {
            if (change.x > 0) return 90;
            else return 270;
        }
    }

    public List<Transform> GetRightWay()
    {
        var endTile = roads[^1].transform;
        var rightWay = new List<Transform>() { endTile };
        GameObject check;
        do
        {
            check = rightWay[^1].GetComponent<AddRoom>().previousRoad;
            if (check != null) rightWay.Add(check.transform);
        } while (check != null);
        rightWay.Reverse();
        return rightWay;
    }

    private IEnumerator SpawnCars()
    {
        Transform newParent = transform.Find("Cars");

        float rotation = GetCarRotation();

        while (!trackCheckpoints.CheckpointsAreSet)
            yield return null;


        float distance = (numberOfCars - 1) * spacing;
        float startSpawn = -distance / 2f;
        int playerPosition = Random.Range(0, numberOfCars);

        for (int p = 0; p < numberOfCars; p += 1)
        {
            float x = 0f;
            float z = 0f;
            if (rotation < 1f || (rotation > 179f && rotation < 181f))
                x = startSpawn + p * spacing;
            else 
                z = startSpawn + p * spacing;

            GameObject newCar;
            if (isTraining)
            {
                newCar = Instantiate(car, newParent);
            }
            else if (p == playerPosition)
            {
                newCar = Instantiate(player, newParent);
            }
            else
            {
                newCar = Instantiate(enemy, newParent);
            }
            newCar.transform.SetLocalPositionAndRotation(new Vector3(x, 0.2f, z), Quaternion.Euler(0, rotation, 0));
        }

        CarsAreSet = true;

        mapLoader.PauseGame(10f);
        while (mapLoader.deltaTime > 0)
            yield return null;

        mapLoader.ResumeGame();
    }

    private void SetCameras()
    {
        GameObject.FindGameObjectWithTag("LoadSceneCamera").GetComponent<Camera>().enabled = false;
        GameObject.Find("Main Camera").GetComponent<Camera>().enabled = true;
    }
}