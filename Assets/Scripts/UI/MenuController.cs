using System.Threading;
using Elympics;
using Elympics.Models.Authentication;
using JetBrains.Annotations;
using Plugins.Elympics.Plugins.ParrelSync;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button rejoinButton;
    [SerializeField] private TMP_InputField halfRemotePlayerId;
    [SerializeField] private GameObject editorOnlyPanel;

    private const string PlayOnlineText = "Play Online";
    private const string CancelMatchmakingText = "Cancel matchmaking";

    private const string RejoinOnlineText = "Rejoin Online";
    private const string CancelRejoinText = "Cancel rejoin";

    private TextMeshProUGUI _playButtonText;
    private TextMeshProUGUI _rejoinButtonText;
    private CancellationTokenSource _cts;
    private string _closestRegion;

    private void Start()
    {
        #if UNITY_EDITOR
            editorOnlyPanel.SetActive(true);
        #endif
        
        if (ElympicsLobbyClient.Instance.IsAuthenticated)
            HandleAuthenticated(ElympicsLobbyClient.Instance.AuthData);
        else
            ElympicsLobbyClient.Instance.AuthenticationSucceeded += HandleAuthenticated;
        ElympicsLobbyClient.Instance.Matchmaker.MatchmakingCancelledGuid += _ => ResetState();
        ElympicsLobbyClient.Instance.Matchmaker.MatchmakingFailed += _ => ResetState();
        ChooseRegion();

        _playButtonText = playButton.GetComponentInChildren<TextMeshProUGUI>();
        _rejoinButtonText = rejoinButton.GetComponentInChildren<TextMeshProUGUI>();
        ResetState();

        if (!ElympicsClonesManager.IsClone())
            return;
        halfRemotePlayerId.text = ElympicsGameConfig.GetHalfRemotePlayerIndex(0).ToString();
        halfRemotePlayerId.placeholder.GetComponent<TextMeshProUGUI>().enabled = true;
    }

    private void ResetState()
    {
        _cts?.Cancel();
        _playButtonText.text = PlayOnlineText;
        _rejoinButtonText.text = RejoinOnlineText;
        _cts = null;
    }

    private void HandleAuthenticated(AuthData authData)
    {
        if (_closestRegion != null)
            playButton.interactable = true;

        ElympicsLobbyClient.Instance.HasAnyUnfinishedMatch(isUnfinishedMatchAvailable => rejoinButton.interactable = isUnfinishedMatchAvailable, Debug.LogError);
    }

    private async void ChooseRegion()
    {
        _closestRegion = (await ElympicsCloudPing.ChooseClosestRegion(ElympicsRegions.AllAvailableRegions)).Region;
        if (string.IsNullOrEmpty(_closestRegion))
            _closestRegion = ElympicsRegions.Warsaw;
        Debug.Log($"Selected region: {ElympicsRegions.Warsaw}");
        if (ElympicsLobbyClient.Instance.IsAuthenticated && playButton)
            playButton.interactable = true;
    }
    
    public void OnPlayOnlineClicked()
    {
        if (_cts != null)
        {
            ResetState();
            return;
        }

        _cts = new CancellationTokenSource();
        _playButtonText.text = CancelMatchmakingText;
        ElympicsLobbyClient.Instance.PlayOnlineInRegion(_closestRegion, cancellationToken: _cts.Token);
    }

    public void OnRejoinOnlineClicked()
    {
        if (_cts != null)
        {
            ResetState();
            return;
        }

        _cts = new CancellationTokenSource();
        _rejoinButtonText.text = CancelRejoinText;
        ElympicsLobbyClient.Instance.RejoinLastOnlineMatch(ct: _cts.Token);
    }

#if UNITY_EDITOR
    [UsedImplicitly]
    public void OnPlayLocalClicked()
    {
        ElympicsLobbyClient.Instance.PlayOffline();
    }
    [UsedImplicitly]
    public void OnPlayHalfRemoteClicked()
    {
        var playerId = int.Parse(halfRemotePlayerId.text);
        ElympicsLobbyClient.Instance.PlayHalfRemote(playerId);
    }

    [UsedImplicitly]
    public void OnStartHalfRemoteServer()
    {
        ElympicsLobbyClient.Instance.StartHalfRemoteServer();
    }
#endif
}
