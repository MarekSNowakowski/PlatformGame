using System;
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

    private bool isDestroyed;

    public void GameOver(bool win)
    {
        resultText.text = win ? youWinText : youLoseText;
        Time.timeScale = 0;
        if(!isDestroyed && gameObject)
            gameObject.SetActive(true);
    }

    public void GameOver()
    {
        resultText.text = gameOverText;
        Time.timeScale = 0;
        if(!isDestroyed && gameObject)
            gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        isDestroyed = true;
        Time.timeScale = 1;
    }
}
