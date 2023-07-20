using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Elympics;

public class CameraPlayerFinder : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera camera;
    
    private void Start()
    {
        PlayerHandler[] playerHandlers = FindObjectsOfType<PlayerHandler>();

        foreach (var p in playerHandlers)
        {
            if (p.Elympics.Player == p.PredictableFor)
            {
                Debug.Log(p.gameObject.name);
                camera.Follow = p.transform;
            }
        }
    }
}
