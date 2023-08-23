using Elympics;
using TMPro;
using UnityEngine;

public class GameplayTimer : ElympicsMonoBehaviour, IUpdatable, IInitializable, IObservable
{
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private int gameplayTime = 60 * 5;
    [SerializeField] private GameStateSynchronizer gameStateSynchronizer;
    [SerializeField] private ResultManager resultManager;
    
    private readonly ElympicsBool countdownStarted = new ElympicsBool(false);
    private readonly ElympicsFloat gameplayTimer = new ElympicsFloat();

    public void Initialize()
    {
        gameplayTimer.Value = gameplayTime;
        gameStateSynchronizer.SubscribeToGameStateChange(OnGameStateChange);
        countdownStarted.ValueChanged += HandleTimerVisibility;
    }
    
    public void ElympicsUpdate()
    {
        if (!countdownStarted.Value)
            return;

        gameplayTimer.Value -= Elympics.TickDuration;

        if (gameplayTimer.Value > 0f)
        {
            float timer = Mathf.Ceil(gameplayTimer.Value);
            float minutes = Mathf.Floor(timer / 60);
            float seconds = timer%60;
            timerText.text = $"{minutes}:{seconds:00}";
        }
        else
        {
            // Game end
            gameStateSynchronizer.FinishGame();
            resultManager.GameOver(false);
        }
    }

    public int GetTimeLeft()
    {
        return (int) Mathf.Ceil(gameplayTimer.Value);
    }

    public void OnSafeZoneShrinkingStart()
    {
        timerText.color = Color.red;
    }

    private void OnGameStateChange(int value, int newValue)
    {
        if (newValue == (int)GameState.Gameplay)
        {
            countdownStarted.Value = true;
        }
        else if (newValue == (int)GameState.GameEnded)
        {
            countdownStarted.Value = false;
        }
    }
    
    private void HandleTimerVisibility(bool v1, bool v2)
    {
        if (v2)
        {
            timerText.enabled = true;
        }
        else
        {
            timerText.enabled = false;
        }
    }
}
