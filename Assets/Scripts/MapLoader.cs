using UnityEngine;

public class MapLoader : MonoBehaviour
{
    [Range(0, 1)]
    public int gameMode = 0;
    public int enemies = 0;
    [SerializeField] GameObject easyMap;
    [SerializeField] GameObject hardMap;
    [SerializeField] CounterText counterText;

    private bool timeStopped;
    [HideInInspector] public float deltaTime;

    public void Restart()
    {
        switch(gameMode)
        {
            case 0:
                if (transform.childCount > 0)
                    Destroy(transform.GetChild(0).gameObject);
                Instantiate(easyMap, transform);
                break;
            case 1:
                if (transform.childCount > 0)
                    Destroy(transform.GetChild(0).gameObject);
                Instantiate(hardMap, transform);
                break;
        }
    }

    public void PauseGame(float time)
    {
        Time.timeScale = 0;
        deltaTime = time;
        timeStopped = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        timeStopped = false;
        counterText.Set("");
    }

    private void Update()
    {
        if (timeStopped)
        {
            deltaTime -= Time.unscaledDeltaTime;
            counterText.Set(Mathf.RoundToInt(deltaTime).ToString());
        }
    }
}
