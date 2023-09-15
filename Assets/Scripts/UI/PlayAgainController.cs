using System.Threading;
using Elympics;
using Elympics.Models.Authentication;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayAgainController : MonoBehaviour
{
    [SerializeField] private Button playButton;

    private const string PlayOnlineText = "Play again";
    private const string CancelMatchmakingText = "Cancel matchmaking";
    
    private TextMeshProUGUI _playButtonText;
    private CancellationTokenSource _cts;
    private string _closestRegion;

    private void Start()
    {
        if (ElympicsLobbyClient.Instance)
        {
            if (ElympicsLobbyClient.Instance.IsAuthenticated)
                HandleAuthenticated(ElympicsLobbyClient.Instance.AuthData);
            else
                ElympicsLobbyClient.Instance.AuthenticationSucceeded += HandleAuthenticated;
            ElympicsLobbyClient.Instance.Matchmaker.MatchmakingCancelledGuid += _ => ResetState();
            ElympicsLobbyClient.Instance.Matchmaker.MatchmakingFailed += _ => ResetState();
            ChooseRegion();

            _playButtonText = playButton.GetComponentInChildren<TextMeshProUGUI>();
            ResetState();
        }
        else
        {
            Debug.LogWarning("Elympics lobby client instance is missing");
            playButton.interactable = false;
        }
    }

    private void ResetState()
    {
        _cts?.Cancel();
        _playButtonText.text = PlayOnlineText;
        _cts = null;
    }

    private void HandleAuthenticated(AuthData authData)
    {
        if (_closestRegion != null)
            playButton.interactable = true;
    }

    private async void ChooseRegion()
    {
        _closestRegion = (await ElympicsCloudPing.ChooseClosestRegion(ElympicsRegions.AllAvailableRegions)).Region;
        if (string.IsNullOrEmpty(_closestRegion))
            _closestRegion = ElympicsRegions.Warsaw;
        Debug.Log($"Selected region: {ElympicsRegions.Warsaw}");
        if (ElympicsLobbyClient.Instance.IsAuthenticated)
            playButton.interactable = true;
    }

    public void OnPlayAgainClicked()
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
    
    public void QuitToMenuClicked()
    {
        SceneManager.LoadScene(0);
    }
}
