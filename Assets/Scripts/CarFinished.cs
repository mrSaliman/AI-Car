using System.Collections.Generic;
using UnityEngine;

public class CarFinished : MonoBehaviour
{
    private List<CarController> carsFinished;
    [HideInInspector] public MapLoader mapLoader;

    private void Start()
    {
        carsFinished = new List<CarController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3)
        {
            carsFinished.Add(other.GetComponent<CarController>());
            other.gameObject.SetActive(false);
            if (carsFinished[^1].isPlayer)
            {
                mapLoader.Restart();
            }
        }
    }
}