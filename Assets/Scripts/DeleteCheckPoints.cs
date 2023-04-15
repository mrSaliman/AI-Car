using UnityEngine;

public class DeleteCheckPoints : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Check Point"))
            Destroy(other.gameObject);
    }
}
