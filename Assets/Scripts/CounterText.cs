using TMPro;
using UnityEngine;

public class CounterText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI counterText;

    public void Set(string text)
    {
        counterText.text = text;
    }
}
