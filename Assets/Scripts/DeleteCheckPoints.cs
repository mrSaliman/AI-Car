using System.Collections.Generic;
using UnityEngine;

public class DeleteCheckPoints : MonoBehaviour
{
    private List<GameObject> deletedCheckPoints = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            other.gameObject.SetActive(false);
            deletedCheckPoints.Add(other.gameObject);
        }
    }

    public void Regenrate()
    {
        foreach (var checkPoint in  deletedCheckPoints)
            checkPoint.SetActive(true);
    }
}
