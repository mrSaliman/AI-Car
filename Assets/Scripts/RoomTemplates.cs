using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomTemplates : MonoBehaviour
{
    public GameObject[] bottomRoads;
    public GameObject[] topRoads;
    public GameObject[] leftRoads;
    public GameObject[] rightRoads;
    public GameObject finish;
    public GameObject car;

    public List<GameObject> roads;
    public Vector3 tileScale;
    private bool spawFinish = false;
    private int countOfRoads = 0;

    private void Start()
    {
        InvokeRepeating("EndSpawning", 0f, 1f);
    }

    private void EndSpawning()
    {
        if (spawFinish == false && countOfRoads == roads.Count)
        {
            DeleteBorders();
            GenerateRightWay();
            Instantiate(finish, roads[roads.Count - 1].transform.position, Quaternion.identity);
            Invoke("SpawnCar", 0.1f);
            spawFinish = true;
        }
        else
            countOfRoads = roads.Count;
            
    }


    private void DeleteBorders()
    {
        for(int i = 0; i < roads.Count; i++)
        {
            Transform[] borders = roads[i].GetComponentsInChildren<Transform>();
            for (int j = 0; j < borders.Length; j++)
            {
                if (borders[j].CompareTag("Border") && borders[j].gameObject.GetComponent<BorderCollisionCount>().CollisionCount > 0)
                {
                    Destroy(borders[j].gameObject);
                }
            }
        } 
    }


    private void GenerateRightWay()
    {
        List<GameObject> rightWay = new List<GameObject>() { roads[roads.Count - 1] };
        GameObject tmp;
        do
        {
           tmp = rightWay[rightWay.Count - 1].GetComponent<AddRoom>().previousRoad;
           rightWay.Add(tmp);
        } while (tmp != null);
        
        for(int i = 0; i < roads.Count; i++)
        {
            if (!rightWay.Contains(roads[i]))
            {
                Transform[] borders = roads[i].GetComponentsInChildren<Transform>();
                for (int j = 0; j < borders.Length; j++)
                {
                    if (borders[j].CompareTag("Check Point"))
                    {
                        Destroy(borders[j].gameObject);
                    }
                }
            }
        }

        for(int i = 1; i < rightWay.Count-2; i++)
        {
            if (rightWay[i].GetComponentsInChildren<Transform>().Where(e => e.CompareTag("Check Point")).Count() > 3)
            {
                List<int> whiteListOfCheckPoints = new List<int>() { 0 };
                float minDistanceForPrev = float.MaxValue, minDistanceForNext = float.MaxValue;
                int checkPointIdWithMinDistanceForPrev = 0, checkPointIdWithMinDistanceForNext = 0;
                var checkPoints = rightWay[i].GetComponentsInChildren<Transform>().Where(e => e.CompareTag("Check Point")).ToList();
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
                        minDistanceForNext= distance;
                        checkPointIdWithMinDistanceForNext= checkPoints[j].GetComponent<CheckPointDirection>().Direction;
                    }
                }
                whiteListOfCheckPoints.Add(checkPointIdWithMinDistanceForNext);
                whiteListOfCheckPoints.Add(checkPointIdWithMinDistanceForPrev);
               
                for(int j = 0; j < checkPoints.Count; j++)
                {
                    if (!whiteListOfCheckPoints.Contains(checkPoints[j].GetComponent<CheckPointDirection>().Direction))
                        Destroy(checkPoints[j].gameObject);
                }
            }
        }

        List<int> whiteList = new List<int>() { 0 };
        float minDistanceForFirst = float.MaxValue;
        int checkPointIdWithMinDistanceForFirst = 0;
        var checkPointsForFirstRoad = rightWay[rightWay.Count-2].GetComponentsInChildren<Transform>().Where(e => e.CompareTag("Check Point")).ToList();
        for(int i = 0; i < checkPointsForFirstRoad.Count; i++)
        {
            float distanceForFirst = Vector3.Distance(checkPointsForFirstRoad[i].position, rightWay[rightWay.Count - 3].transform.position);
            if (distanceForFirst < minDistanceForFirst)
            {
                minDistanceForFirst = distanceForFirst;
                checkPointIdWithMinDistanceForFirst = checkPointsForFirstRoad[i].GetComponent<CheckPointDirection>().Direction;
            }   
        }
        whiteList.Add(checkPointIdWithMinDistanceForFirst);
        for (int i = 0; i < checkPointsForFirstRoad.Count; i++)
        {
            if (!whiteList.Contains(checkPointsForFirstRoad[i].GetComponent<CheckPointDirection>().Direction))
                Destroy(checkPointsForFirstRoad[i].gameObject);
        }
    }


    private float GetCarRotation()
    {
        float rotation = 0;
        var checkPoints = roads[0]
            .GetComponentsInChildren<Transform>()
            .Where(e => e.CompareTag("Check Point"))
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

    private void SpawnCar() => Instantiate(car, new Vector3(0, 0.2f, 0), Quaternion.Euler(0, GetCarRotation(), 0));
}