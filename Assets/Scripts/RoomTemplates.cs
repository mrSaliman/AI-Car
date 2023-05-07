using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class RoomTemplates : MonoBehaviour
{
    public GameObject[] bottomRoads;
    public GameObject[] topRoads;
    public GameObject[] leftRoads;
    public GameObject[] rightRoads;
    public GameObject car;

    public List<GameObject> roads;
    public Vector3 tileScale;

    [HideInInspector] public bool IsGenerationDone;
    [HideInInspector] public bool CarsAreSet;
    private int roadsCount = 0;
    private TrackCheckpoints trackCheckpoints;


    private void Start()
    {
        trackCheckpoints = GetComponent<TrackCheckpoints>();
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
        Debug.Log("Spawning Cars...");

        for (var p = -4; p < 5; p += 2)
        {
            float x = 0f;
            float z = 0f;
            if (rotation < 1f || (rotation > 179f && rotation < 181f))
                x = p;
            else 
                z = p;

            GameObject newCar = Instantiate(car, newParent);
            //newCar.name = "Car " + p;
            //newCar.GetComponent<Rigidbody>().position = new Vector3(x, 0.2f, z);
            //newCar.GetComponent<Rigidbody>().rotation = Quaternion.Euler(0, rotation, 0);
            newCar.transform.SetPositionAndRotation(new Vector3(x, 0.2f, z) + transform.position, Quaternion.Euler(0, rotation, 0));
            //Debug.Log("Set pos for " + newCar.name + " pos " + newCar.transform.position);
        }

        CarsAreSet = true;
    }

    private void SetCameras()
    {
        GameObject.FindGameObjectWithTag("LoadSceneCamera").GetComponent<Camera>().enabled = false;
    }
}