using Elympics;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class GameStartCountdown : ElympicsMonoBehaviour, IUpdatable, IInitializable, IObservable
{
    [SerializeField] private WaitingServerHandler serverHandler;

    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private int secondsToStartAfterConnection = 3;

    private readonly ElympicsBool countdownStarted = new ElympicsBool(false);
    private readonly ElympicsFloat timeToStart = new ElympicsFloat();

    public event System.Action OnCountdownEnded;

    private void Awake()
    {
        Assert.IsNotNull(serverHandler);
        Assert.IsNotNull(countdownText);
        Assert.IsFalse(secondsToStartAfterConnection < 0);
    }

    public void Initialize()
    {
        serverHandler.OnGameReady += () => { countdownStarted.Value = true; };
        countdownStarted.ValueChanged += StartCountdown;
    }

    private void StartCountdown(bool v1, bool v2)
    {
        if (v2)
        {
            countdownText.enabled = true;
            timeToStart.Value = secondsToStartAfterConnection;
        }
        else
        {
            countdownText.gameObject.SetActive(false);
        }
    }

    public void ElympicsUpdate()
    {
        if (!countdownStarted.Value)
            return;

        timeToStart.Value -= Elympics.TickDuration;

        if (timeToStart.Value > 0f)
        {
            countdownText.text = Mathf.Ceil(timeToStart.Value).ToString();
        }
        else
        {
            // Game start
            countdownStarted.Value = false;
            countdownText.gameObject.SetActive(false);
            OnCountdownEnded?.Invoke();
        }
    }
}
