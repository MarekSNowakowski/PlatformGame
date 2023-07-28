using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ErrorPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI errorMessageText;

    public void Display(string errorMessage, bool matchmakingError)
    {
        Debug.LogError(errorMessage);

        gameObject.SetActive(true);
        errorMessageText.text = errorMessage;
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene(0);
    }
}
