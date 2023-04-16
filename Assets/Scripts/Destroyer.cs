using UnityEngine;

public class Destroyer : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SpawnPoint"))
            Destroy(other.gameObject);
    }
}
