using System;
using UnityEngine;

public class NoteTileParticleSystem : MonoBehaviour
{
    public delegate void OnDisableCallback(NoteTileParticleSystem instance);
    public OnDisableCallback Disable;
    
    private void OnParticleSystemStopped()
    {
        Disable?.Invoke(this);
    }

    // private void Update()
    // {
    //     if (!GetComponent<ParticleSystem>().IsAlive())
    //         OnParticleSystemStopped();
    // }
}
