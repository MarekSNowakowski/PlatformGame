using Cinemachine;
using UnityEngine;

public class CameraPlayerFinder : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;
    
    private void Start()
    {
        PlayerHandler[] playerHandlers = FindObjectsOfType<PlayerHandler>();

        foreach (var p in playerHandlers)
        {
            if (p.Elympics.Player == p.PredictableFor)
            {
                Debug.Log(p.gameObject.name);
                virtualCamera.Follow = p.transform;
            }
        }
    }
}
