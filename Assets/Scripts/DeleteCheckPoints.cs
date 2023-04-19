using UnityEngine;

public class DeleteCheckPoints : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
            Destroy(other.gameObject);
    }
}
