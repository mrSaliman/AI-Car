using TMPro;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    private void LateUpdate()
    {
        GameObject car = GameObject.FindGameObjectWithTag("Car");
        if (car != null)
            gameObject.GetComponent<TMP_Text>().text = GetCarSpeed(car);
    }

    private string GetCarSpeed(GameObject car)
    {
        Rigidbody rb = car.GetComponent<Rigidbody>();
        float speed = rb.velocity.magnitude * 3.6f;
        return $"{(int)speed} KM/H";
    }
}
