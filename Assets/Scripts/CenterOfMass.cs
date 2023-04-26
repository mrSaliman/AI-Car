using UnityEngine;

public class CenterOfMass : MonoBehaviour
{
    public Transform CenterOfMassTransform;

    void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = Vector3.Scale(CenterOfMassTransform.localPosition, transform.localScale);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(GetComponent<Rigidbody>().worldCenterOfMass, 0.1f);
    }
}
