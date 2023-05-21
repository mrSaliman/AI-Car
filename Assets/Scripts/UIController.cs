using UnityEngine;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    public MapLoader mapLoader;
    public Button startButton;
    public DropdownField difficulity;
    public SliderInt enemies;
    private VisualElement root;

    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;

        startButton = root.Q<Button>("StartButton");
        difficulity = root.Q<DropdownField>("DropdownField");
        enemies = root.Q<SliderInt>("Enemies");

        startButton.clicked += StartButtonPressed;
    }

    void StartButtonPressed()
    {
        mapLoader.gameMode = difficulity.index;
        mapLoader.enemies = enemies.value;
        mapLoader.Restart();
        root.visible = false;
    }
}
