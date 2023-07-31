using Elympics;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class DustHandler : ElympicsMonoBehaviour, IUpdatable, IObservable
{
    private ParticleSystem particleSystem;
    private ElympicsBool playDust = new ElympicsBool();
    
    private void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    public void CreateDust()
    {
        playDust.Value = true;
    }

    public void ElympicsUpdate()
    {
        if (playDust.Value)
        {
            particleSystem.Play();
            playDust.Value = false;
        }
    }
}
