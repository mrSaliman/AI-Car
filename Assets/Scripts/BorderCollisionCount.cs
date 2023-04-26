using UnityEngine;

public class BorderCollisionCount : MonoBehaviour
{
    public int CollisionCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Border"))
            CollisionCount++;
    }
}