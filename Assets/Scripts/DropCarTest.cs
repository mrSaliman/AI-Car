using UnityEngine;
using UnityEngine.SceneManagement;

public class DropCarTest : MonoBehaviour
{
    void Update()
    {
        if (gameObject.transform.position.y < -1)
            SceneManager.LoadScene("MapGeneration");
    }
}