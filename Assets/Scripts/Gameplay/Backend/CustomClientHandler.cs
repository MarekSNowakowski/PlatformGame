﻿using UnityEngine;
using System;
using Elympics;
using MatchTcpClients.Synchronizer;

public class CustomClientHandler : ElympicsMonoBehaviour, IClientHandlerGuid
{
    [SerializeField] private GameStateSynchronizer gameStateSynchronizer;
    [SerializeField] private ErrorPanel errorPanel;
    [SerializeField] private int timeoutSeconds = 5;

    private DateTime? timeoutThreshold;

    public void OnConnected(TimeSynchronizationData data)
    {
        if (Elympics.IsClient)
        {
            PersistentEffectsManager.Instance.PlayGameplayMusic();
        }
    }

    public void OnSynchronized(TimeSynchronizationData data)
    {
        timeoutThreshold = data.UnreliableLastReceivedPingDateTime + new TimeSpan(0, 0, timeoutSeconds);
    }

    private void Update()
    {
        if (timeoutThreshold != null && timeoutThreshold < DateTime.Now && gameStateSynchronizer.GameState != GameState.GameEnded)
        {
            Elympics.Disconnect();

            OnConnectingFailed();

            timeoutThreshold = null;
        }
    }

    public void OnConnectingFailed()
    {
        errorPanel.Display("Connection error!\n Check your Internet connection");
    }

    public void OnDisconnectedByServer()
    {
        if (gameStateSynchronizer.GameState != GameState.GameEnded)
        {
            errorPanel.Display("Disconnected by the server. \n One of players disconnected or an error occured");
        }
    }
    
    #region Unused
    public void OnStandaloneClientInit(InitialMatchPlayerDataGuid data) { }
    public void OnDisconnectedByClient() { }
    public void OnClientsOnServerInit(InitialMatchPlayerDatasGuid data) { }
    public void OnAuthenticated(Guid userId) { }
    public void OnAuthenticatedFailed(string errorMessage) { }
    public void OnMatchJoined(Guid matchId) { }
    public void OnMatchJoinedFailed(string errorMessage) { }

    public void OnMatchEnded(Guid matchId) { }
    #endregion
}