using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
            GetCenterCheckPoints();
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
        var rightWay = GetRightWay();
        
        roads.Where(e => !rightWay.Contains(e))
            .Select(e => e.GetComponentsInChildren<Transform>()).ToList()
            .ForEach(e => e.Where(k => k.CompareTag("Check Point")).ToList()
            .ForEach(k => Destroy(k.gameObject)));

        for(int i = 1; i < rightWay.Count-2; i++)
        {
            if (rightWay[i].GetComponentsInChildren<Transform>().Where(e => e.CompareTag("Check Point")).Count() > 3)
            {
                List<int> whiteListOfCheckPoints = new List<int>() { 0 };
                var checkPoints = rightWay[i].GetComponentsInChildren<Transform>().Where(e => e.CompareTag("Check Point")).ToList();
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
                        minDistanceForNext= distance;
                        checkPointIdWithMinDistanceForNext= checkPoints[j].GetComponent<CheckPointDirection>().Direction;
                    }
                }
                whiteListOfCheckPoints.Add(checkPointIdWithMinDistanceForNext);
                whiteListOfCheckPoints.Add(checkPointIdWithMinDistanceForPrev);

                checkPoints.Where(e => !whiteListOfCheckPoints.Contains(e.GetComponent<CheckPointDirection>().Direction)).ToList()
                    .ForEach(e => Destroy(e.gameObject));
            }
        }

        DeleteFirstAndLastCheckPoints(rightWay.Count - 2, rightWay.Count - 3, rightWay);
        DeleteFirstAndLastCheckPoints(0, 1, rightWay);
    }


    private void DeleteFirstAndLastCheckPoints(int actul, int next, List<GameObject> rightWay)
    {
        float minDistance = float.MaxValue;
        int checkPointWithMinDistance = 0;
        var checkPoints = rightWay[actul].GetComponentsInChildren<Transform>().Where(e => e.CompareTag("Check Point")).ToList();
        
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


    private List<GameObject> GetRightWay() 
    {
        var rightWay = new List<GameObject>() { roads[roads.Count - 1] };
        GameObject tmp;
        do
        {
            tmp = rightWay[rightWay.Count - 1].GetComponent<AddRoom>().previousRoad;
            rightWay.Add(tmp);
        } while (tmp != null);
        return rightWay;
    } 
    
    private void GetCenterCheckPoints()
    {
        var rightWay = GetRightWay();
        var centerCheckPoints = new List<GameObject>();

        for(int i = 0; i < rightWay.Count-1; i++)
        {
            var checkPoints = rightWay[i].GetComponentsInChildren<Transform>().Where(e => e.CompareTag("Check Point")).ToList();
            if(checkPoints.Count > 2)
            {
                for (int j = 0; j < checkPoints.Count; j++)
                {
                    if (checkPoints[j].GetComponent<CheckPointDirection>().Direction == 0)
                    {
                        centerCheckPoints.Add(checkPoints[j].gameObject);
                    }
                }
            }
        }
        
        for(int i = 0; i < centerCheckPoints.Count - 2; i++)
        {
            string a = GetRotationTag(centerCheckPoints[i].transform.position, centerCheckPoints[i + 1].transform.position, centerCheckPoints[i + 2].transform.position);
            centerCheckPoints[i+1].tag = a;
        }
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


    private void SpawnCar()
    {
        Instantiate(car, new Vector3(0, 0.2f, 0), Quaternion.Euler(0, GetCarRotation(), 0));
    }
}