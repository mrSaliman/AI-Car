using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    RoomTemplates roomTemplates;
    public void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Car"))
        {   
            //Debug.Log(other.gameObject);
            Destroy(other.gameObject);
        }
    }
}
