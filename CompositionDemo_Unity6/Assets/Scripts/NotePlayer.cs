using Unity.VisualScripting;
using UnityEngine;
using Update = UnityEngine.PlayerLoop.Update;

public class NotePlayer : MonoBehaviour
{
    public int NoteIndex;
    public float Multiplier;

    public AudioSource Source;
    public bool SourceAssigned;
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
        if (SourceAssigned)
        {
            Source.GetOutputData(_samples, 0);
            
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
            
            Debug.Log(intensity);
            
            _material.SetColor(_glowColorID, intensity * _emissionColor);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Source = AudioManager.Instance.AudioSourcePool.Get();
        
        StartCoroutine(AudioManager.Instance.PlayNote(this, NoteIndex, Multiplier, transform));

        NoteTileParticleSystem particleSystem = SpawnerParticleSystem.Instance.GetParticleSystem();
        if (particleSystem != null)
        {
            particleSystem.transform.position = transform.position;
            ParticleSystem.MainModule settings = particleSystem.GetComponent<ParticleSystem>().main;
            settings.startColor = _material.GetColor(_baseColorID);
        }
    }
}
