using System;
using TMPro;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    private void LateUpdate()
    {
        gameObject.GetComponentInChildren<TMP_Text>().text = $"{(int) GetCarSpeed()} KM/H";
    }

    public float GetCarSpeed()
    {
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        return rb.velocity.magnitude * 3.6f; 
    }
}
