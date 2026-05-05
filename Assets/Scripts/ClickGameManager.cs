using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClickGameManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button _gameClickButton;
    [SerializeField] private TMP_Text _currentScoreText;

    private int _currentScore = 0;

    public int CurrentScore
    {
        get { return _currentScore; }
    }

    private void Reset()
    {
        _gameClickButton = GameObject.Find("ButtonGameClick")?.GetComponent<Button>();
        _currentScoreText = GameObject.Find("TextCurrentScore")?.GetComponent<TMP_Text>();
    }

    private void Start()
    {
        _gameClickButton.onClick.AddListener(AddPoint);
        UpdateScoreText();
    }

    private void AddPoint()
    {
        _currentScore++;
        UpdateScoreText();

        Debug.Log("Puntaje actual: " + _currentScore);
    }

    private void UpdateScoreText()
    {
        _currentScoreText.text = "Puntaje actual: " + _currentScore;
    }

    public void ResetScore()
    {
        _currentScore = 0;
        UpdateScoreText();
    }
}