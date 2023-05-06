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
    private int roadsCount = 0;

    private void Start()
    {
        InvokeRepeating(nameof(EndSpawning), 0f, 1f);
    }

    private void EndSpawning()
    {
        if (roadsCount == roads.Count)
        {
            DeleteBorders();
            GenerateRightWay();
            SetCenterCheckPointRotation();
            SetCenterCheckPoints();
            SetEndCheckPoint();
            SpawnCars();
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
            {
                if (border.CompareTag("Border"))
                {
                    var borderCollisionCount = border.GetComponent<BorderCollisionCount>();
                    if (borderCollisionCount != null && borderCollisionCount.CollisionCount > 4)
                    {
                        Destroy(border.gameObject);
                    }
                }
            }
        }

    }

    private void GenerateRightWay()
    {
        var rightWay = GetRightWay();

        // Cache the child transform components for each road element
        var roadComponents = roads.Select(e => e.GetComponent<Transform>()).ToList();

        // Use a hash set for faster containment checks
        var rightWaySet = new HashSet<Transform>(rightWay);

        // Use parallel processing to delete "Forward CheckPoint" components
        foreach (var roadComponent in roadComponents) 
        {
            if (!rightWaySet.Contains(roadComponent))
            {
                foreach (var component in roadComponent.GetComponentsInChildren<Transform>().Where(k => k.CompareTag("Forward CheckPoint")))
                    Destroy(component.gameObject);
                
            }
        }

        for (int i = 1; i < rightWay.Count - 1; i++)
        {
            List<int> whiteListOfCheckPoints = new() { 0 };
            var checkPoints = rightWay[i].GetComponentsInChildren<Transform>().Where(e => e.CompareTag("Forward CheckPoint")).ToList();
            float minDistanceForPrev = float.MaxValue;
            float minDistanceForNext = float.MaxValue;
            int checkPointIdWithMinDistanceForPrev = 0;
            int checkPointIdWithMinDistanceForNext = 0;

            for (int j = 0; j < checkPoints.Count; j++)
            {
                float distance = Vector3.Distance(checkPoints[j].position, rightWay[i - 1].transform.position);
                if (distance < minDistanceForPrev)
                {
                    minDistanceForPrev = distance;
                    checkPointIdWithMinDistanceForPrev = checkPoints[j].GetComponent<CheckPointDirection>().Direction;
                }

                distance = Vector3.Distance(checkPoints[j].position, rightWay[i + 1].transform.position);
                if (distance < minDistanceForNext)
                {
                    minDistanceForNext = distance;
                    checkPointIdWithMinDistanceForNext = checkPoints[j].GetComponent<CheckPointDirection>().Direction;
                }
            }
            whiteListOfCheckPoints.Add(checkPointIdWithMinDistanceForNext);
            whiteListOfCheckPoints.Add(checkPointIdWithMinDistanceForPrev);

            checkPoints.Where(e => !whiteListOfCheckPoints.Contains(e.GetComponent<CheckPointDirection>().Direction)).ToList()
                .ForEach(e => Destroy(e.gameObject));
        }

        DeleteFirstAndLastCheckPoints(rightWay.Count - 1, rightWay.Count - 2, rightWay);
    }

    private void DeleteFirstAndLastCheckPoints(int actul, int next, List<Transform> rightWay)
    {
        float minDistance = float.MaxValue;
        int checkPointWithMinDistance = 0;
        var checkPoints = rightWay[actul].GetComponentsInChildren<Transform>().Where(e => e.CompareTag("Forward CheckPoint")).ToList();

        for (int i = 0; i < checkPoints.Count; i++)
        {
            float distance = Vector3.Distance(checkPoints[i].position, rightWay[next].transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                checkPointWithMinDistance = checkPoints[i].GetComponent<CheckPointDirection>().Direction;
            }
        }

        checkPoints.Where(e => e.GetComponent<CheckPointDirection>().Direction != checkPointWithMinDistance && e.GetComponent<CheckPointDirection>().Direction != 0).ToList()
            .ForEach(e => Destroy(e.gameObject));
    }

    public float GetCarRotation()
    {
        float rotation = 0;
        var checkPoints = roads[0]
            .GetComponentsInChildren<Transform>()
            .Where(e => e.CompareTag("Forward CheckPoint"))
            .Select(e => e.GetComponent<CheckPointDirection>().Direction)
            .ToList();

        if (checkPoints.Contains(1))
            rotation += 90;
        else if (checkPoints.Contains(2))
            rotation += 270;
        else if (checkPoints.Contains(3))
            rotation += 180;
        return rotation;
    }

    private List<Transform> GetRightWay()
    {
        var endTile = roads[^1].transform;
        var rightWay = new List<Transform>() { endTile };
        GameObject check;
        do
        {
            check = rightWay[^1].GetComponent<AddRoom>().previousRoad;
            if (check != null) rightWay.Add(check.transform);
        } while (check != null);
        return rightWay;
    }

    private void SetCenterCheckPoints()
    {
        var rightWay = GetRightWay();
        var centerCheckPoints = new List<GameObject>();

        for (int i = 0; i < rightWay.Count - 1; i++)
        {
            var checkPoints = rightWay[i].GetComponentsInChildren<Transform>().Where(e => e.CompareTag("Forward CheckPoint")).ToList();
            for (int j = 0; j < checkPoints.Count; j++)
            {
                if (checkPoints[j].GetComponent<CheckPointDirection>().Direction == 0)
                {
                    centerCheckPoints.Add(checkPoints[j].gameObject);
                }
            }
        }

        for (int i = 0; i < centerCheckPoints.Count - 2; i++)
        {
            string centralCheckPointTag = GetRotationTag(centerCheckPoints[i].transform.position, centerCheckPoints[i + 1].transform.position, centerCheckPoints[i + 2].transform.position);
            centerCheckPoints[i + 1].tag = centralCheckPointTag;
        }
    }


    private void SetEndCheckPoint()
    {
        var rightWay = GetRightWay();
        var finishTile = rightWay[0];
        var prevTile = rightWay[1];
        GameObject endCheckPoint = null;
        float maxDistance = 0f;
        var checkPoints = finishTile.GetComponentsInChildren<Transform>().Where(e => e.gameObject.layer == 6).ToList();
        for (int i = 0; i < checkPoints.Count; i++)
        {
            var distance = Vector3.Distance(checkPoints[i].transform.position, prevTile.transform.position);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                endCheckPoint = checkPoints[i].gameObject;
            }
        }
        endCheckPoint.tag = "End CheckPoint";
        endCheckPoint.AddComponent<CarFinished>();
    }

    private string GetRotationTag(Vector3 a, Vector3 b, Vector3 c)
    {
        var ab = a - b;
        var ac = a - c;
        float angle = Vector3.SignedAngle(ab, ac, b);
        if (angle < -40.0f)
            return "Right CheckPoint";
        else if (angle > 40.0f)
            return "Left CheckPoint";
        else
            return "Forward CheckPoint";
    }

    private void SetCenterCheckPointRotation()
    {
        var rightWay = GetRightWay();
        for (int i = 1; i < rightWay.Count - 1; i++)
        {
            var centerCheckPoints = rightWay[i].GetComponentsInChildren<Transform>().Where(e => e.CompareTag("Forward CheckPoint")).ToList();
            if (centerCheckPoints.Count >= 7)
            {
                Debug.Log(rightWay[i]);
                var actualCenterCheckPoint = centerCheckPoints.FirstOrDefault(e => e.GetComponent<CheckPointDirection>().Direction == 0);

                var prevCenterCheckPoint = rightWay[i - 1].GetComponentsInChildren<Transform>()
                    .Where(e => e.CompareTag("Forward CheckPoint")).ToList()
                    .FirstOrDefault(e => e.GetComponent<CheckPointDirection>().Direction == 0);

                var nextCenterCheckPoint = rightWay[i + 1].GetComponentsInChildren<Transform>()
                    .Where(e => e.CompareTag("Forward CheckPoint")).ToList()
                    .FirstOrDefault(e => e.GetComponent<CheckPointDirection>().Direction == 0);

                actualCenterCheckPoint.rotation = Quaternion.Euler(0, GetCenterCheckPointRotation(prevCenterCheckPoint.position, nextCenterCheckPoint.position), 0);
            }
        }
    }

    private float GetCenterCheckPointRotation(Vector3 prev, Vector3 next)
    {
        if (prev.x == next.x)
        {
            return 90f;
        }
        else if (prev.z == next.z)
        {
            return 0f;
        }


        else if ((next.x - prev.x > 0 && next.z - prev.z > 0) || (next.x - prev.x < 0 && next.z - prev.z < 0))
        {
            return 135f;
        }
        else
        {
            return 45f;
        }
    }

    private void SpawnCars()
    {
        //yield return new WaitForSeconds(0.1f);

        float rotation = GetCarRotation();

        for (var p = -4; p < 5; p += 2)
        {
            float x = 0f;
            float z = 0f;
            if (rotation < 1f || (rotation > 179f && rotation < 181f))
                x = p;
            else 
                z = p;

            GameObject newCar = Instantiate(car);
            //newCar.name = "Car " + p;
            //newCar.GetComponent<Rigidbody>().position = new Vector3(x, 0.2f, z);
            //newCar.GetComponent<Rigidbody>().rotation = Quaternion.Euler(0, rotation, 0);
            newCar.transform.SetPositionAndRotation(new Vector3(x, 0.2f, z), Quaternion.Euler(0, rotation, 0));
            //Debug.Log("Set pos for " + newCar.name + " pos " + newCar.transform.position);

            //yield return new WaitForFixedUpdate();
        }
    }

    private void SetCameras()
    {
        GameObject.FindGameObjectWithTag("LoadSceneCamera").GetComponent<Camera>().enabled = false;
    }
}