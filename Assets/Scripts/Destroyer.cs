using UnityEngine;

public class Destroyer : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (!(other.CompareTag("Car") || other.CompareTag("Finish") || other.CompareTag("Road") || other.CompareTag("Check Point")))
            Destroy(other.gameObject);
    }
}
