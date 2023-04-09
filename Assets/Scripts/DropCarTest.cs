using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DropCarTest : MonoBehaviour
{
    void Update()
    {
        if (GameObject.FindGameObjectWithTag("Car").transform.position.y < -1)
            SceneManager.LoadScene("MapGeneration");
    }
}
