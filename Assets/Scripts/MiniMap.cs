using UnityEngine;

public class MiniMap : MonoBehaviour
{
    private void Update()
    {
        GameObject car = GameObject.FindGameObjectWithTag("Car");
        if (car != null)
        {
            Vector3 newPosition = car.transform.position;
            newPosition.y = transform.position.y;
            transform.position = newPosition;

            transform.rotation = Quaternion.Euler(90f, car.transform.eulerAngles.y, 0f);
        }
    }
}
