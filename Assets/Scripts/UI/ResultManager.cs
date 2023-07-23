using TMPro;
using UnityEngine;

public class ResultManager : MonoBehaviour
{
    [SerializeField]
    private string youWinText;
    [SerializeField]
    private string youLoseText;
    [SerializeField]
    private string gameOverText;
    [SerializeField]
    private TextMeshProUGUI resultText;

    public void GameOver(bool win)
    {
        resultText.text = win ? youWinText : youLoseText;
        gameObject?.SetActive(true);
        Time.timeScale = 0;
    }

    public void GameOver()
    {
        resultText.text = gameOverText;
        gameObject?.SetActive(true);
        Time.timeScale = 0;
    }
}
