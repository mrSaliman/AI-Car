using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class CarCollider : MonoBehaviour
{
    private int score = 0;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(score);
        if(other.CompareTag("Check Point"))
        {
            Destroy(other.gameObject);
            score++;
        }
        if (other.CompareTag("Border"))
        {
            score -= 5;
        }
    }
}
