using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class NotePlayer : MonoBehaviour
{
    public int NoteIndex;
    public float Multiplier;

    [SerializeField] ParticleSystem _particleSystem;
    Transform _particleSystemContainer;
    Material _material;

    void Start()
    {
        _particleSystemContainer = GameObject.Find("Particle System Container").GetComponent<Transform>();
        _material = GetComponent<Renderer>().material;
    }

    void OnTriggerEnter(Collider other)
    {
        AudioManager.Instance.PlayNote(NoteIndex, Multiplier);

        ParticleSystem particleSystem = Instantiate(_particleSystem, other.transform.position, Quaternion.identity, _particleSystemContainer);
        ParticleSystem.MainModule mainModule = particleSystem.main;
        mainModule.startColor = _material.color;
    }
}
