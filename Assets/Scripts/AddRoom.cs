using UnityEngine;

public class AddRoom : MonoBehaviour
{
    private RoomTemplates templates;
    public GameObject previousRoad;

    private void Start()
    {
        templates = transform.parent.GetComponent<RoomTemplates>();
        templates.roads.Add(gameObject);
    }
}