using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Pool;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    private float[] _noteMultipliers = new float[12];

    public int[] CurrentScale;
    
    Constants.Scale _selectedScale;
    Constants.Scale SelectedScale
    {
        get => _selectedScale;
        set
        {
            Constants.Scale index = value;
            
            if ((int)index >= Constants.Scales.Count)
                index = 0;
            else if ((int)index < 0)
                index = Constants.Scales.ElementAt(Constants.Scales.Count - 1).Key;
            
            _selectedScale = index;
            CurrentScale = Constants.Scales[_selectedScale];
        }
    }
    
    bool _inTransition;
    public float TransitionTime = 2.0f;

    [Header("AudioSource Settings")]
    [SerializeField] AudioClip _audioClip;
    [SerializeField] AudioMixerGroup _mixerGroup;
    
    [Header("AudioSource Pool")]
    public ObjectPool<AudioSource> AudioSourcePool;
    List<AudioSource> _activeAudioSources = new List<AudioSource>();
    [SerializeField] int MaxAudioSources = 1000;
    
    [SerializeField] AudioSource _audioSourcePrefab;
    
    [Header("Spectrum Data")]
    [SerializeField] int _numFftBands = 16;
    float[] _fftBandRangeFrequencies;
    [SerializeField] int _vectorSize = 1024;
    [SerializeField] FFTWindow _fftWindowType = FFTWindow.BlackmanHarris;
    
    public AudioChannelData Left; 
    public AudioChannelData Right; 
    
    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        AudioSourcePool = new ObjectPool<AudioSource>(
            CreatePooledAudioSource,
            OnGetAudioSource,
            OnReleaseAudioSource,
            OnDestroyAudioSource,
            false,
            MaxAudioSources,
            MaxAudioSources);
        
        InitNoteMultipliers();
        InitFftBandRangeFrequencies();

        Left = new AudioChannelData(_vectorSize, 0, _fftWindowType, _fftBandRangeFrequencies);
        Right = new AudioChannelData(_vectorSize, 1, _fftWindowType, _fftBandRangeFrequencies);
    }
    
    void Start()
    {
        SelectedScale = Constants.Scale.Dorian;
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
            SelectedScale++;
        
        if (Input.GetKeyDown(KeyCode.S))
            SelectedScale--;

        if (Input.GetKeyDown(KeyCode.M) && !_inTransition)
        {
            _inTransition = true;
            foreach (AudioSource source in _activeAudioSources)
            {
                StartCoroutine(TransitionPlaybackMode(source));
            }
        }
    }

    /********************* INITS *********************/
    void InitNoteMultipliers()
    {
        float twelfthRoot = Mathf.Pow(2.0f, 1.0f / 12.0f);

        _noteMultipliers = _noteMultipliers
            .Select((_, i) => Mathf.Pow(twelfthRoot, i))
            .ToArray();
    }

    void InitFftBandRangeFrequencies()
    {
        float freqMin = 20.0f;
        float freqMax = AudioSettings.outputSampleRate * 0.5f;
        
        // Ratio for each step in logarithmic scaling
        float ratio = Mathf.Pow(freqMax / freqMin, 1.0f / _numFftBands);

        _fftBandRangeFrequencies = new float[_numFftBands + 1];
        _fftBandRangeFrequencies[0] = freqMin;

        _fftBandRangeFrequencies = _fftBandRangeFrequencies
            .Select(
                (freq, i) => i == 0
                    ? freq
                    : _fftBandRangeFrequencies[i - 1] * ratio)
            .ToArray();
    }

    /********************* POOL METHODS *********************/
    AudioSource CreatePooledAudioSource()
    {
        AudioSource instance = Instantiate(_audioSourcePrefab, Vector3.zero, Quaternion.identity);
        instance.gameObject.SetActive(false);

        return instance;
    }
    
    void OnGetAudioSource(AudioSource instance)
    {
        instance.gameObject.SetActive(true);
        instance.transform.SetParent(transform, true);
        _activeAudioSources.Add(instance);
    }

    void OnReleaseAudioSource(AudioSource instance)
    {
        instance.gameObject.SetActive(false);
    }

    void OnDestroyAudioSource(AudioSource instance)
    {
        Destroy(instance.gameObject);
    }

    void ReleaseAudioSource(AudioSource instance)
    {
        AudioSourcePool.Release(instance);
        _activeAudioSources.Remove(instance);
    }
    
    /********************* METHODS *********************/
    public IEnumerator PlayNote(NotePlayer tilePlayer, int note, float multiplier, Transform tileTransform)
    {
        tilePlayer.SourceAssigned = true;
        
        tilePlayer.transform.position = tileTransform.position;
        
        tilePlayer.Source.pitch = _noteMultipliers[CurrentScale[note % CurrentScale.Length]] * multiplier;
        tilePlayer.Source.PlayOneShot(tilePlayer.Source.clip);
        
        yield return new WaitWhile(() => tilePlayer.Source.isPlaying || _inTransition);
        
        tilePlayer.SourceAssigned = false;
        ReleaseAudioSource(tilePlayer.Source);
    }
    
    IEnumerator TransitionPlaybackMode(AudioSource source)
    {
        float initialValue = source.spatialBlend;
        float destinationValue = 1.0f - source.spatialBlend;
        
        float elapsedTime = 0.0f;
    
        while (elapsedTime <= TransitionTime)
        {
            source.spatialBlend = Mathf.Lerp(initialValue, destinationValue, elapsedTime / TransitionTime);
            
            elapsedTime += Time.deltaTime;
            
            yield return null;
        }
    
        source.spatialBlend = Mathf.Round(source.spatialBlend);
    
        _inTransition = false;
    }
}
