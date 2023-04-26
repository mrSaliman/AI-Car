using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public GameObject miniMapCamera;
    private bool _bigMapIsShowed = false;
    private float _cameraYPos;

    private void Start()
    {
        _cameraYPos = miniMapCamera.transform.position.y;
    }


    private void LateUpdate()
    {
        Vector3 newPosition = gameObject.transform.position;
        newPosition.y = _bigMapIsShowed ? _cameraYPos + 500f : _cameraYPos + gameObject.GetComponent<Speedometer>().GetCarSpeed();
        miniMapCamera.transform.position = newPosition;
        miniMapCamera.transform.rotation = Quaternion.Euler(90f, gameObject.transform.eulerAngles.y, 0f);

        if (Input.GetKeyDown(KeyCode.M) && _bigMapIsShowed == false)
        {
            ScaleMap(new Vector3(0f, 0f, 0f), new Vector3(5f, 5f, 1f), miniMapCamera.transform.position);
            _bigMapIsShowed = true;
        }
        else if (Input.GetKeyDown(KeyCode.M) && _bigMapIsShowed == true)
        {
            ScaleMap(new Vector3(740f, -320f, 0f), new Vector3(2f, 2f, 1f), miniMapCamera.transform.position);
            _bigMapIsShowed = false;
        }
    }

    private void ScaleMap(Vector3 mapPosition, Vector3 mapScale, Vector3 cameraPosition)
    {
        GameObject map = GameObject.FindGameObjectWithTag("MiniMapRowImage");
        map.transform.localPosition = mapPosition;
        map.transform.localScale = mapScale;

        miniMapCamera.transform.position = cameraPosition;
    }
}