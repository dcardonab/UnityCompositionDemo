/*
 * Project Name: Unity Composition Demo
 * Script Name: CameraBehavior.cs
 * Description: This script handles how particle systems will be removed from the pool.
 * Author: David Cardona
 * Date: July 17, 2024
 * License: MIT License
 */

using UnityEngine;

public class NoteTileParticleSystem : MonoBehaviour
{
    public delegate void OnDisableCallback(NoteTileParticleSystem instance);
    public OnDisableCallback Disable;
    
    private void OnParticleSystemStopped()
    {
        Disable?.Invoke(this);
    }
}
