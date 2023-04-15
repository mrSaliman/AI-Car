using System;
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
    private float roadSpawnTime = 5f;


    private void Update()
    {
        if (roadSpawnTime <= 0 && spawFinish == false)
        {
            DeleteBorders();
            GenerateRightWay();
            Instantiate(finish, roads[roads.Count - 1].transform.position, Quaternion.identity);
            spawFinish = true;
        }
        else
            roadSpawnTime -= Time.deltaTime;
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
    }
}