using Elympics;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameStateSynchronizer : ElympicsMonoBehaviour, IInitializable
{
    [SerializeField] private GameStartCountdown gameStartCountdown;

    private readonly ElympicsInt gameState = new ElympicsInt((int)GameState.Initialization);

    public GameState GameState => (GameState)gameState.Value;

    public void Initialize()
    {
        Assert.IsNotNull(gameStartCountdown);
        gameStartCountdown.OnCountdownEnded += StartGame;
    }

    public void SubscribeToGameStateChange(ElympicsVar<int>.ValueChangedCallback action)
    {
        gameState.ValueChanged += action;
    }

    private void StartGame()
    {
        SetGameState(GameState.Gameplay);
    }

    public void FinishGame()
    {
        SetGameState(GameState.GameEnded);

        if (Elympics.IsServer)
        {
            Elympics.EndGame();
        }
    }

    private void SetGameState(GameState newState)
    {
        gameState.Value = (int)newState;
    }
}
