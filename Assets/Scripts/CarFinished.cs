using UnityEngine;
using UnityEngine.SceneManagement;

public class CarFinished : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        /*if (other.CompareTag("Car"))
            SceneManager.LoadScene("MapGeneration");*/
    }
}