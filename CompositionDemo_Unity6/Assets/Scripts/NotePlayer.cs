using System.Collections.Generic;
using UnityEngine;

public class NotePlayer : MonoBehaviour
{
    public int NoteIndex;
    public float Multiplier;

    public List<AudioSource> Sources = new List<AudioSource>();
    private float[] _samples = new float[1024];
    
    Material _material;
    Color _emissionColor;
    int _baseColorID;
    int _glowColorID;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        _material = new Material(renderer.material);

        renderer.material = _material;
        
        _baseColorID = Shader.PropertyToID("_BaseColor");
        _glowColorID = Shader.PropertyToID("_GlowColor");
        
        _emissionColor = _material.GetColor(_glowColorID);
    }

    void Update()
    {
        if (Sources.Count > 0)
        {
            // The ^ operator is used to specify "From End"
            AudioSource lastSource = Sources[^1];
            if (lastSource.isPlaying)
            {
                lastSource.GetOutputData(_samples, 0);
                
                float rms = Utilities.GetRootMeanSquared(_samples);
                float intensity = Utilities.Map(
                    rms,
                    -80.0f, 0.0f,
                    0.01f, 10.0f,
                    clamp: true
                );
                intensity = Mathf.Pow(2, intensity);
                intensity = Utilities.Map(
                    intensity,
                    0.0f, 1024.0f,
                    0.0f, 100.0f
                );
                
                _material.SetColor(_glowColorID, intensity * _emissionColor);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        AudioSource source = AudioManager.Instance.AudioSourcePool.Get();
        Sources.Add(source);

        StartCoroutine(AudioManager.Instance.PlayNote(this, Sources.Count - 1, NoteIndex, Multiplier, other));

        NoteTileParticleSystem particleSystem = SpawnerParticleSystem.Instance.GetParticleSystem();
        if (particleSystem != null)
        {
            particleSystem.transform.position = transform.position;
            ParticleSystem.MainModule settings = particleSystem.GetComponent<ParticleSystem>().main;
            settings.startColor = _material.GetColor(_baseColorID);
        }
    }

    public void RemoveSource(int index)
    {
        if (index >= 0 && index < Sources.Count)
        {
            AudioSource source = Sources[index];

            if (source != null)
            {
                AudioManager.Instance.ReleaseAudioSource(source);
                Sources.RemoveAt(index);
            }
        }
    }
}
