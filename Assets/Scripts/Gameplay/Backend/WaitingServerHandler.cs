using Elympics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class WaitingServerHandler : ElympicsMonoBehaviour, IServerHandlerGuid, IUpdatable
{
    [SerializeField] private float timeForPlayersToConnect = 30f;
    [SerializeField] private float timeForPlayersToRejoin = 15f;
    [SerializeField] private float connectingTimeoutCheckDelta = 5f;
    [SerializeField] private bool shouldGameEndAfterAnyDisconnect = false;
    [SerializeField] private RandomManager randomManager;

    private readonly HashSet<ElympicsPlayer> _playersConnected = new HashSet<ElympicsPlayer>();
    private int _playersNumber;
    private int _humanPlayersNumber;

    private bool _gameCancelled = false;
    private bool _isReadyLocally = false;
    private readonly ElympicsBool _gameReady = new ElympicsBool(false);

    public event Action OnGameReady;
    public bool IsGameReady => _gameReady.Value;

    private void Awake()
    {
        OnGameReady += () => Debug.Log("All players are ready");
    }

    public void OnServerInit(InitialMatchPlayerDatasGuid initialMatchPlayerDatas)
    {
        if (!IsEnabledAndActive)
            return;

        // Ensure initial synchronization variables are correct
        Assert.IsFalse(timeForPlayersToConnect < 0f || connectingTimeoutCheckDelta < 0f || connectingTimeoutCheckDelta > timeForPlayersToConnect);

        _playersNumber = initialMatchPlayerDatas.Count;
        _humanPlayersNumber = initialMatchPlayerDatas.Count(x => !x.IsBot);
        Debug.Log($"Game initialized with {_humanPlayersNumber} human players and {initialMatchPlayerDatas.Count - _humanPlayersNumber} bots");

        InitializeRandomnessSeed(initialMatchPlayerDatas);

        StartCoroutine(WaitForClientsToConnect());
    }

    private void InitializeRandomnessSeed(InitialMatchPlayerDatasGuid initialMatchPlayerDatas)
    {
        //byte[] data = initialMatchPlayerDatas[0].GameEngineData;
        //int seed = BitConverter.ToInt32(data);
        //randomManager.SetSeed(seed);

        // If you don't have External Game Backend which could provide seed on match creation, use code below instead
        randomManager.SetSeed(UnityEngine.Random.Range(int.MinValue, int.MaxValue));
    }

    private IEnumerator WaitForClientsToConnect()
    {
        DateTime waitForPlayersFinishTime = DateTime.Now + TimeSpan.FromSeconds(timeForPlayersToConnect);

        while (DateTime.Now < waitForPlayersFinishTime && !GameStateAlreadyDetermined)
        {
            Debug.Log("Waiting for all players to connect...");
            yield return new WaitForSeconds(connectingTimeoutCheckDelta);
        }

        if (GameStateAlreadyDetermined)
            yield break;

        EndGameForcefully("Not all players have connected, therefore the game cannot start and so it ends");
    }
    
    private IEnumerator WaitForPlayerToRejoin()
    {
        DateTime waitForPlayersFinishTime = DateTime.Now + TimeSpan.FromSeconds(timeForPlayersToRejoin);

        while (DateTime.Now < waitForPlayersFinishTime && !RejoiningDetermined)
        {
            Debug.Log("Waiting for player to rejoin...");
            yield return new WaitForSeconds(connectingTimeoutCheckDelta);
        }

        if (RejoiningDetermined)
            yield break;

        EndGameForcefully("Player disconnected and failed to rejoin, therefore the game ends");
    }

    public void OnPlayerDisconnected(ElympicsPlayer player)
    {
        if (!IsEnabledAndActive)
            return;

        Debug.Log($"Player {player} disconnected");
        _playersConnected.Remove(player);

        if (_gameCancelled)
            return;
        
        StartCoroutine(WaitForPlayerToRejoin());

        if (shouldGameEndAfterAnyDisconnect || NoHumanPlayersInGame)
            EndGameForcefully("Therefore, the game has ended");
    }


    public void OnPlayerConnected(ElympicsPlayer player)
    {
        if (!IsEnabledAndActive)
            return;

        Debug.Log($"Player {player} connected");
        _playersConnected.Add(player);

        if (NotAllPlayersConnected || GameStateAlreadyDetermined)
            return;

        BeginTheGame();
    }

    private bool GameStateAlreadyDetermined => _gameReady.Value || _gameCancelled;
    private bool RejoiningDetermined => _playersConnected.Count == _playersNumber || _gameCancelled;
    private bool NoHumanPlayersInGame => _playersConnected.Count == _playersNumber - _humanPlayersNumber;
    private bool NotAllPlayersConnected => _playersConnected.Count != _playersNumber;

    private void BeginTheGame()
    {
        _gameReady.Value = true;
    }

    private void EndGameForcefully(string message)
    {
        _gameCancelled = true;
        Debug.Log(message);
        Elympics.EndGame();
    }

    public void ElympicsUpdate()
    {
        if (_isReadyLocally)
            return;

        if (!_gameReady.Value)
            return;

        OnGameReady?.Invoke();
        _isReadyLocally = true;
    }
}
