using System.Collections.Generic;
using UnityEngine;

public class AddRoom : MonoBehaviour
{
    private RoomTemplates templates;
    public GameObject previousRoad;

    private void Start()
    {
        templates = GameObject.FindGameObjectWithTag("Roads").GetComponent<RoomTemplates>();
        templates.roads.Add(gameObject);
    }
}