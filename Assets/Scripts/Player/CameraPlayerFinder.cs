using Cinemachine;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraPlayerFinder : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;
    [SerializeField]
    private GameStateSynchronizer gameStateSynchronizer;
    
    private void Start()
    {
        Assert.IsNotNull(virtualCamera);
        Assert.IsNotNull(gameStateSynchronizer);
        gameStateSynchronizer.SubscribeToGameStateChange(SetCameraTarget);
    }

    private void SetCameraTarget(int previousValue, int newValue)
    {
        if (newValue == (int)GameState.Gameplay)
        {
            PlayerHandler[] playerHandlers = FindObjectsOfType<PlayerHandler>();

            foreach (var p in playerHandlers)
            {
                if (p.Elympics.Player == p.PredictableFor)
                {
                    virtualCamera.Follow = p.transform;
                }
            }
        }
    }
}
