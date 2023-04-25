using TMPro;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    private void LateUpdate()
    {
        GameObject car = GameObject.FindGameObjectWithTag("Car");
        if (car != null)
            gameObject.GetComponent<TMP_Text>().text = $"{(int)GetCarSpeed(car)} KM/H";
    }

    public float GetCarSpeed(GameObject car)
    {
        Rigidbody rb = car.GetComponent<Rigidbody>();
        return rb.velocity.magnitude * 3.6f; 
    }
}
